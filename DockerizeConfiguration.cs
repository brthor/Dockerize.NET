using System;

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

        public DockerizeConfiguration(string projectName, string tag, string baseRid, string baseImage)
        {
            GeneratedImageTag = tag ?? projectName;
            BaseRid = baseRid ?? "linux-x64";
            BaseImage = baseImage ?? "microsoft/dotnet:2.0-runtime";

            Console.WriteLine("Dockerize Config");
            Console.WriteLine($"Base Docker Image: {BaseImage}");
            Console.WriteLine($"Base Rid of Docker Image: {BaseRid}");
            Console.WriteLine($"Tag: {GeneratedImageTag}");
        }
    }
}