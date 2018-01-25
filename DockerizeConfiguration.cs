using Microsoft.DotNet.Cli.Utils;

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

        public DockerizeConfiguration(string projectName, string configuration, string tag, string baseRid, string baseImage, 
            string username=null)
        {
            GeneratedImageTag = tag ?? projectName;
            BaseRid = baseRid ?? "linux-x64";
            BaseImage = baseImage ?? "microsoft/dotnet:2.0-runtime";
            BuildConfiguration = configuration ?? "Release";
            Username = username;
            
            Reporter.Output.WriteLine($"Dockerize.NET".Blue());
            Reporter.Output.WriteLine("Base Docker Image: ".White() +  $"{BaseImage}".Green());
            Reporter.Output.WriteLine("Base Rid of Docker Image: ".White() + $"{BaseRid}".Green());
            Reporter.Output.WriteLine("Tag: ".White() + $"{GeneratedImageTag}".Green());
            
            if (Username != null)
                Reporter.Output.WriteLine("Username: ".White() + $"{Username}".Green());
        }
    }
}