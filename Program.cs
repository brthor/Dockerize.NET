using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.CommandLineUtils;

namespace Brthor.Dockerize
{
    class Program
    {
        static int Main(string[] args)
        {
            var commandLineApplication =
                new CommandLineApplication(throwOnUnexpectedArg: false);
            
            var project = commandLineApplication.Option(
                "-p |--project <project>",
                "The path to the project to dockerize. Only required if the multiple projects exist " +
                "in a directory or the project is not in the current working directory.",
                CommandOptionType.SingleValue);

            var tag = commandLineApplication.Option(
                "-t |--tag <tag>",
                "The desired tag name of the created image. Will be directly passed to " +
                "docker build -t, see docker build --help for more info. Defaults to the project name.",
                CommandOptionType.SingleValue);
            
            var baseRid = commandLineApplication.Option(
                "-r |--runtime <RID>",
                "The RID of the specified Base Docker image. Defaults to \"linux-x64\".",
                CommandOptionType.SingleValue);
            
            var baseImage = commandLineApplication.Option(
                "-i |--image <image>",
                "The base docker image used for the generated docker file. If you change this from the default, be sure to" +
                "update BaseRid if appropriate. Defaults to \"microsoft/dotnet:2.0-runtime\".",
                CommandOptionType.SingleValue);
            
            commandLineApplication.HelpOption("-? | -h | --help");
            commandLineApplication.OnExecute(() =>
            {
                var projectName = GetProjectName(Environment.CurrentDirectory, project);
                var config = new DockerizeConfiguration(projectName, tag.Value(), baseRid.Value(), baseImage.Value());

                return Run(config);
            });
            
            return commandLineApplication.Execute(args);
        }

        private static string GetProjectName(string currentDirectory, CommandOption project)
        {
            string projectFilePath;
            if (project.HasValue())
            {
                projectFilePath = project.Value();

                if (!File.Exists(projectFilePath))
                {
                    throw new GracefulException(string.Format(
                        "The project file {0} does not exist.",
                        projectFilePath));
                }
            }
            else
            {
                projectFilePath = GetMSBuildProjPath(currentDirectory);
            }

            return Path.GetFileNameWithoutExtension(projectFilePath);
        }

        static int Run(DockerizeConfiguration config){

            var projectDirectory = Environment.CurrentDirectory;
            var dockerizeBaseDir = Path.Combine(projectDirectory, "bin", "dockerize");
            var publishOutDirectory = Path.Combine(dockerizeBaseDir, "publish");
            var dockerfilePath = Path.Combine(dockerizeBaseDir, "Dockerfile");
            
            var publish = Command.Create("dotnet", new[] {"publish", "-o", publishOutDirectory, "-r", config.BaseRid});

            var publishResult = publish.WorkingDirectory(projectDirectory).ForwardStdErr().ForwardStdOut().Execute();

            if (publishResult.ExitCode != 0)
            {
                return publishResult.ExitCode;
            }

            var publishOutputDepsJsons = Directory.EnumerateFiles(publishOutDirectory, "*.deps.json").ToList();
            if (publishOutputDepsJsons.Count > 1)
            {
                throw new GracefulException(string.Format(
                    "Multiple output programs were found in {0}. Please try removing bin obj directories and retrying.",
                    publishOutDirectory));
            }
            
            if (!publishOutputDepsJsons.Any())
            {
                throw new GracefulException(string.Format(
                    "No output programs were found in {0}. Please examine 'dotnet publish' results and try again.",
                    publishOutDirectory));
            }

            var outputBinaryName =
                Path.GetFileNameWithoutExtension(publishOutputDepsJsons.Single()).Replace(".deps", "");

            var dockerfile = DockerfileTemplate.Generate(config, outputBinaryName);
            
            File.WriteAllText(dockerfilePath, dockerfile);

            var dockerBuild = Command.Create("docker", new[] {"build", "-t", config.GeneratedImageTag, "."}).WorkingDirectory(dockerizeBaseDir).ForwardStdErr()
                .ForwardStdOut();

            var dockerResult = dockerBuild.Execute();

            return dockerResult.ExitCode;
        }
        
        // https://github.com/dotnet/cli/blob/444d75c0cd482f44af392d4fce8bfc081b25d2b4/src/Microsoft.DotNet.Cli.Utils/CommandResolution/ProjectFactory.cs#L71
        // ReSharper disable once InconsistentNaming
        private static string GetMSBuildProjPath(string projectDirectory)
        {
            IEnumerable<string> projectFiles = Directory
                .GetFiles(projectDirectory, "*.*proj")
                .Where(d => !d.EndsWith(".xproj")).ToList();

            if (!projectFiles.Any())
            {
                throw new GracefulException(string.Format(
                    "No project files were found in {0}.",
                    projectDirectory));
            }
            else if (projectFiles.Count() > 1)
            {
                throw new GracefulException(string.Format(
                    "Multiple project files were found in {0}.",
                    projectDirectory));
            }

            return projectFiles.First();
//            var type = Type.GetType("Microsoft.DotNet.Cli.Utils.ProjectFactory");
//            var projectFactory = Activator.CreateInstance(type, new EnvironmentProvider());
//
//            var method = projectFactory.GetType()
//                .GetMethod("GetMSBuildProjPath", BindingFlags.Instance | BindingFlags.NonPublic);
//
//            return (string) method.Invoke(projectFactory, new object[] {});
        }
    }
}
