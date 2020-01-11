FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY GeigerPublisher/*.csproj ./GeigerPublisher/
COPY GeigerPublisher.Tests/*.csproj ./GeigerPublisher.Tests/
RUN dotnet restore

# copy everything else and build app
COPY GeigerPublisher/. ./GeigerPublisher/
WORKDIR /app/GeigerPublisher
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS runtime
WORKDIR /app
COPY --from=build /app/GeigerPublisher/out ./
ENTRYPOINT ["dotnet", "GeigerPublisher.dll"]
