# Dockerize.NET: Your .NET Core App to a docker image

[![Build Status](https://travis-ci.org/brthor/Dockerize.NET.svg?branch=master)](https://travis-ci.org/brthor/Dockerize.NET)
[![Myget Version Number](https://img.shields.io/myget/thor/v/Brthor.Dockerize.NET.svg?color=green)](https://www.myget.org/feed/thor/package/nuget/Brthor.Dockerize.NET)

`dotnet dockerize -t brthor/serviceWorker:dev`

This is a simple dotnet cli tool that enables you to easily package your dotnet app into a docker container. The above invocation creates a docker image with the tag `brthor/serviceWorker:dev` which you can then `docker push` to your registry or `docker run` locally.

The image simply runs the `static void Main(string[] args)` or other entrypoint of your app.

This has many uses:
 - Getting a service image ready for kubernetes environment
 - Testing your application in different OS's available in docker
 - Isolated environments for your app.
 
## Installation
 
 It's easy, add the following to your `*.csproj` file:
 ```XML
 <ItemGroup>
   <DotNetCliToolReference Include="Brthor.Dockerize.NET" Version="1.0.0-*" />
 </ItemGroup>
 ```

 Then `dotnet restore`, followed by `dotnet dockerize` to make your 

## Options

See `dotnet dockerize -h` for available options.

```
$ dotnet dockerize -h

Usage:  [options]

Options:
  -t |--tag <tag>      The desired tag name of the created image. Will be directly passed to docker build -t, see docker build --help for more info. Defaults to the project name.

  -r |--runtime <RID>  The RID of the specified Base Docker image. Defaults to "linux-x64".

  -i |--image <image>  The base docker image used for the generated docker file. If you change this from the default, be sure toupdate BaseRid if appropriate. Defaults to "microsoft/dotnet:2.0-runtime".

  -? | -h | --help     Show help information
```
