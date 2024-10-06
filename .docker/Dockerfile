FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /DockerSource

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY MeditationCountBot/*.csproj ./MeditationCountBot/
COPY MeditationCountBot.Tests/*.csproj ./MeditationCountBot.Tests/
WORKDIR /DockerSource
RUN dotnet restore --runtime linux-x64

# Copy everything else and build website
COPY MeditationCountBot/. ./MeditationCountBot/
WORKDIR /DockerSource/MeditationCountBot
RUN dotnet publish -c release --runtime linux-x64 --self-contained true -o /DockerOutput/Publish/Bot

# Final stage / image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /DockerOutput/Bot
COPY --from=build /DockerOutput/Publish/Bot ./
ENTRYPOINT ["dotnet", "MeditationCountBot.dll"]