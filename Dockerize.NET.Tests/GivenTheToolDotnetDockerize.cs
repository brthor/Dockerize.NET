using System;
using System.ComponentModel.DataAnnotations;
using Brthor.Dockerize;
using FluentAssertions;
using Microsoft.DotNet.Cli.Utils;
using Xunit;

namespace Dockerize.NET.Tests
{
    public class GivenTheToolDotnetDockerize
    {
        [Fact]
        public void ItCreatesARunnableDockerContainerFromAConsoleApplication()
        {
            var toolTest = new DotnetToolTest(typeof(DockerizeConfiguration).Assembly)
                .WithDisposableTestProject();

            var dockerTag = Guid.NewGuid().ToString();
            var toolExecutionResult = toolTest.ExecuteTool(new [] {"-t", dockerTag});
            toolExecutionResult.Should().Be(0);

            var dockerRunResult = Command.Create("docker", new[] {"run", "--name", dockerTag, "-it", dockerTag})
                .CaptureStdErr()
                .CaptureStdOut()
                .Execute();

            dockerRunResult.StdOut.Should().Contain("Hello World!");
            
            // Cleanup
            Command.Create("docker", new[] {"rm", "-f", dockerTag}).Execute();
            Command.Create("docker", new[] {"rmi", "-f", dockerTag}).Execute();
        }
    }
}
