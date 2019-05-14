#!/bin/bash

branch=$1
buildCounter=$2
majorAlfaPackage=$3
nugetApiKey=$4
if [[ -z $MajorAlfaPackage ]]; then
    MajorAlfaPackage="1.0"
fi

if [[ $branch = "V"* ]]; then
  [[ $branch =~ ^V[[:digit:]]+[.][[:digit:]]+$ ]] && branch="$branch".0
  version=${branch/V/}.$buildCounter 
else
  version=$majorAlfaPackage.0.$buildCounter-$branch
fi

echo "##teamcity[buildNumber '$version']"

docker build . --build-arg VERSION=$version --build-arg NUGET_APIKEY=$nugetApiKey && \
docker build -f ./SonarQube.Dockerfile .
