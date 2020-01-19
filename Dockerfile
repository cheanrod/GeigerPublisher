ARG DOTNET_RUNTIME=linux-x64
ARG IMAGE_TAG=3.1-buster-slim
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG DOTNET_RUNTIME=linux-x64

WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY GeigerPublisher/*.csproj ./GeigerPublisher/
COPY GeigerPublisher.Tests/*.csproj ./GeigerPublisher.Tests/
RUN dotnet restore

# copy everything else and build app
COPY GeigerPublisher/. ./GeigerPublisher/
WORKDIR /app/GeigerPublisher
RUN dotnet publish -r $DOTNET_RUNTIME -c Release -o out


FROM mcr.microsoft.com/dotnet/core/runtime:$IMAGE_TAG AS runtime
WORKDIR /app
COPY --from=build /app/GeigerPublisher/out ./
ENTRYPOINT ["dotnet", "GeigerPublisher.dll"]
