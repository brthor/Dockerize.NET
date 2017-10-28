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

        public DockerizeConfiguration(string projectName, string tag = null, string baseRid = "linux-x64",
            string baseImage = "microsoft/dotnet:2.0-runtime")
        {
            GeneratedImageTag = tag ?? projectName;
            BaseRid = baseRid;
            BaseImage = baseImage;
        }
    }
}