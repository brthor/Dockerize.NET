using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.DotNet.Cli.Utils;

namespace Dockerize.NET.Tests
{
    public class DotnetToolTest
    {
        private string _testProjectPath;
        private readonly Assembly _toolAssembly;
        
        public DotnetToolTest(Assembly toolAssembly)
        {
            _toolAssembly = toolAssembly;
        }

        public DotnetToolTest WithDisposableTestProject()
        {
            var projectName = Guid.NewGuid().ToString();
            _testProjectPath = Path.Combine(
                Path.GetDirectoryName(Path.GetTempFileName()),
                projectName,
                projectName + ".csproj");

            Directory.CreateDirectory(Path.GetDirectoryName(_testProjectPath));

            Command.CreateDotNet("new", new[] {"console"})
                .CaptureStdOut()
                .CaptureStdErr()
                .WorkingDirectory(Path.GetDirectoryName(_testProjectPath))
                .Execute();

            return this;
        }

        public int ExecuteTool(string[] args)
        {
            var muxer = new Muxer();
            var toolDepsFile = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.deps.json").Single();
            var toolRuntimeConfig = Directory.EnumerateFiles(AppContext.BaseDirectory, "*.runtimeconfig.json").Single();

            var dotnetArgs = new []
            {
                "exec",
                "--runtimeconfig",
                toolRuntimeConfig,
                "--depsfile",
                toolDepsFile,
                _toolAssembly.Location
            }.Concat(args).ToArray();


            var arguments = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(dotnetArgs);
            var process = Process.Start(new ProcessStartInfo()
            {
                Arguments = arguments,
                FileName = muxer.MuxerPath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(_testProjectPath)
            });
            
            process.WaitForExit();

            return process.ExitCode;
        }
    }
}