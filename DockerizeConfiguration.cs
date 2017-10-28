namespace Dotnet.Dockerize
{
    public class DockerizeConfiguration
    {
        /// <summary>
        /// The base docker image used for the generated docker file. If you change this from the default, be sure to
        /// update BaseRid if appropriate. Defaults to "microsoft/dotnet:2.0-runtime".
        /// </summary>
        public string BaseImage { get; set; } = "microsoft/dotnet:2.0-runtime";

        /// <summary>
        /// The RID of the specified Base Docker image. Defaults to "linux-x64".
        /// </summary>
        public string BaseRid { get; set; } = "linux-x64";

        /// <summary>
        /// defaults to empty.
        /// </summary>
        public string GeneratedImageTagPrefix { get; set; } = string.Empty;
        
        /// <summary>
        /// Defaults to project name.
        /// </summary>
        public string GeneratedImageTagSuffix { get; set; }

        
        public string GeneratedImageTag => $"{GeneratedImageTagPrefix}{GeneratedImageTagSuffix}";
    }
}