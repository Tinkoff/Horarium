FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
ARG VERSION
ARG NUGET_APIKEY

WORKDIR /sln

COPY Horarium/Horarium.sln ./
COPY Horarium/**/*.csproj ./

RUN for f in *.csproj; do \
        filename=$(basename $f) && \
        dirname=${filename%.*} && \
        mkdir $dirname && \
        mv $filename ./$dirname/; \
    done

RUN dotnet restore Horarium.sln

COPY Horarium ./

RUN dotnet build Horarium.sln -c Release

RUN dotnet test Horarium.Test/Horarium.Test.csproj --no-restore

RUN dotnet test teamcity Horarium.IntegrationTest/Horarium.IntegrationTest.csproj --no-restore 

RUN dotnet pack Horarium/Horarium.csproj -c Release --no-build  /p:PackageVersion=$VERSION
