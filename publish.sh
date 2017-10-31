#!/bin/bash

if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then
	dotnet pack -c Release --version-suffix $(printf %05d $TRAVIS_BUILD_NUMBER)
	dotnet nuget push ./bin/Release/Brthor.Dockerize.NET.*.nupkg -s https://www.myget.org/F/thor/api/v2/package -k $MYGET_KEY
fi
