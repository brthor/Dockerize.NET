using System.Collections.Generic;
using Microsoft.DotNet.Cli.Utils;

// ReSharper disable once CheckNamespace
namespace Brthor.Dockerize
{
    public class DockerizeConfiguration
    {
        private string _generatedImageTag;

        /// <summary>
        /// The base docker image used for the generated docker file. If you change this from the default, be sure to
        /// update BaseRid if appropriate. Defaults to "microsoft/dotnet:2.0-runtime".
        /// </summary>
        public string BaseImage { get; }

        /// <summary>
        /// The RID of the specified Base Docker image. Defaults to "linux-x64".
        /// </summary>
        public string BaseRid { get; }

        public string GeneratedImageTag { get; }
        
        public string BuildConfiguration { get; }
        
        public string Username { get; }
        
        public List<string> ExposedPorts { get; }
        
        public bool DryRun { get; }

        public DockerizeConfiguration(
            string projectName, 
            string configuration, 
            string tag, 
            string baseRid, 
            string baseImage, 
            bool dryRun,
            string username, 
            List<string> exposedPorts)
        {
            GeneratedImageTag = tag ?? projectName;
            BaseRid = baseRid ?? "linux-x64";
            BaseImage = baseImage ?? "microsoft/dotnet:2.0-runtime";
            BuildConfiguration = configuration ?? "Release";
            
            DryRun = dryRun;
            Username = username;
            ExposedPorts = exposedPorts;
            
            Reporter.Output.WriteLine($"Dockerize.NET".Blue());
            Reporter.Output.WriteLine("Base Docker Image: ".White() +  $"{BaseImage}".Green());
            Reporter.Output.WriteLine("Base Rid of Docker Image: ".White() + $"{BaseRid}".Green());
            Reporter.Output.WriteLine("Tag: ".White() + $"{GeneratedImageTag}".Green());
            
            if (!string.IsNullOrWhiteSpace(Username))
                Reporter.Output.WriteLine("Username: ".White() + $"{Username}".Green());
        }
    }
}