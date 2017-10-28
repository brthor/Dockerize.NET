using System;
using System.IO;
using System.Linq;
using Microsoft.DotNet.Cli.Utils;

namespace Brthor.Dockerize
{
    class Program
    {
        static int Main(string[] args)
        {
            var projectDirectory = Environment.CurrentDirectory;
            var muxer = new Muxer();
            var deferencedMuxerPath = DereferenceSymLinks(muxer.MuxerPath);
            var dockerizeBaseDir = Path.Combine(projectDirectory, "bin", "dockerize");
            var publishOutDirectory = Path.Combine(dockerizeBaseDir, "publish");
            var dockerfilePath = Path.Combine(dockerizeBaseDir, "Dockerfile");
            
            var config = new DockerizeConfiguration();

            var publish = Command.Create(deferencedMuxerPath, new[] {"publish", "-o", publishOutDirectory, "-r", config.BaseRid});

            var publishResult = publish.WorkingDirectory(projectDirectory).ForwardStdErr().ForwardStdOut().Execute();

            if (publishResult.ExitCode != 0)
            {
                return publishResult.ExitCode;
            }

            var projectName =
                Path.GetFileNameWithoutExtension(Directory.EnumerateFiles(publishOutDirectory, "*.deps.json").Single()).Replace(".deps", "");

            config.GeneratedImageTagSuffix = projectName;

            var dockerfile = DockerfileTemplate.Generate(config);
            
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
    }
}
