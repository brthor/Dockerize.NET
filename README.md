# Dockerize.NET: Your .NET Core App to a docker image

[![Build Status](https://travis-ci.org/brthor/Dockerize.NET.svg?branch=master)](https://travis-ci.org/brthor/Dockerize.NET)
[![Nuget Version Number](https://img.shields.io/nuget/v/Brthor.Dockerize.NET.svg)](https://www.nuget.org/packages/Brthor.Dockerize.NET)

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

 Then `dotnet restore`, followed by `dotnet dockerize` to make your docker image. The default tag is the project name.

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

## Example 

```bash
$ mkdir newApp
$ cd newApp
$ dotnet new console
The template "Console Application" was created successfully.

... etc output...

```

At this point you need to edit `newApp.csproj` and add the tool reference from above, and the whole file will look like:

```XML
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="Brthor.Dockerize.NET" Version="1.0.0-*" />
  </ItemGroup>
</Project>
```

Continuing on the command line:

```bash
$ dotnet dockerize
Dockerize Config
Base Docker Image: microsoft/dotnet:2.0-runtime
Base Rid of Docker Image: linux-x64
Tag: newApp

... etc output...

$ docker run -it newApp
Hello World!
```

## Contributing

There's a lot of room for improvement, especially with customization of the underlying dockerfile. 
Feel free to create pull requests for additional features.
