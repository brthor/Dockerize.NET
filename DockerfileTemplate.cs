namespace Brthor.Dockerize
{
    public static class DockerfileTemplate
    {
        public static string Generate(DockerizeConfiguration config, string outputBinaryName)
        {
            var addUser = config.Username == null 
                ? ""
                : $@"RUN groupadd -r {config.Username} && useradd --no-log-init -r -g {config.Username} {config.Username}
RUN chown {config.Username}:{config.Username} /projectBinaries
USER {config.Username}:{config.Username}";

            var chownOnAdd = config.Username == null
                ? ""
                : $"--chown={config.Username}:{config.Username} ";
            
            var dockerfileContent = $@"
FROM {config.BaseImage}

RUN mkdir /projectBinaries
{addUser}
ADD {chownOnAdd}./publish/ /projectBinaries/
WORKDIR /projectBinaries/

CMD /projectBinaries/{outputBinaryName}
";
            return dockerfileContent;
        }
    }
}