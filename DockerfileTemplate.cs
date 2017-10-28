namespace Dotnet.Dockerize
{
    public class DockerfileTemplate
    {
        private const string Template = 
@"
FROM {0}

RUN mkdir /projectBinaries
ADD ./publish/ /projectBinaries/

CMD /projectBinaries/{1}
";

        public static string Generate(DockerizeConfiguration config)
        {
            return string.Format(Template, config.BaseImage, config.GeneratedImageTagSuffix);
        }
    }
}