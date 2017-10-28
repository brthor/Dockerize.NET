using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var projectFilePath = GetMSBuildProjPath(Environment.CurrentDirectory);
                var projectName = Path.GetFileNameWithoutExtension(projectFilePath);
                var config = new DockerizeConfiguration(projectName, tag.Value(), baseRid.Value(), baseImage.Value());

                return Run(config);
            });
            
            return commandLineApplication.Execute(args);

        }
        
        static int Run(DockerizeConfiguration config){

            var projectDirectory = Environment.CurrentDirectory;
            var muxer = new Muxer();
            var deferencedMuxerPath = DereferenceSymLinks(muxer.MuxerPath);
            var dockerizeBaseDir = Path.Combine(projectDirectory, "bin", "dockerize");
            var publishOutDirectory = Path.Combine(dockerizeBaseDir, "publish");
            var dockerfilePath = Path.Combine(dockerizeBaseDir, "Dockerfile");
            
            var publish = Command.Create(deferencedMuxerPath, new[] {"publish", "-o", publishOutDirectory, "-r", config.BaseRid});

            var publishResult = publish.WorkingDirectory(projectDirectory).ForwardStdErr().ForwardStdOut().Execute();

            if (publishResult.ExitCode != 0)
            {
                return publishResult.ExitCode;
            }

            var outputBinaryName =
                Path.GetFileNameWithoutExtension(Directory.EnumerateFiles(publishOutDirectory, "*.deps.json").Single()).Replace(".deps", "");

            var dockerfile = DockerfileTemplate.Generate(config, outputBinaryName);
            
            File.WriteAllText(dockerfilePath, dockerfile);

            var dockerBuild = Command.Create("docker", new[] {"build", "-t", config.GeneratedImageTag, "."}).WorkingDirectory(dockerizeBaseDir).ForwardStdErr()
                .ForwardStdOut();

            var dockerResult = dockerBuild.Execute();

            return dockerResult.ExitCode;
        }

        private static string DereferenceSymLinks(string path)
        {
            var oldPath = path;
            var newPath = DereferenceSymLinkSingle(oldPath);

            while (oldPath != newPath && !string.IsNullOrWhiteSpace(newPath))
            {
                oldPath = newPath;
                newPath = DereferenceSymLinkSingle(oldPath);
            }

            return oldPath;
        }

        private static string DereferenceSymLinkSingle(string oldPath)
        {
            return Command.Create("readlink", new[] {oldPath}).CaptureStdOut().CaptureStdErr().Execute().StdOut
                .Trim();
        }
        
        // https://github.com/dotnet/cli/blob/444d75c0cd482f44af392d4fce8bfc081b25d2b4/src/Microsoft.DotNet.Cli.Utils/CommandResolution/ProjectFactory.cs#L71
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
        }
    }
}
