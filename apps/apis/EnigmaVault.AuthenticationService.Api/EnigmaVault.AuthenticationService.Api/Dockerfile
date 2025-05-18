# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Отладка

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EnigmaVault.AuthenticationService.Api/EnigmaVault.AuthenticationService.Api.csproj", "EnigmaVault.AuthenticationService.Api/"]
RUN dotnet restore "./EnigmaVault.AuthenticationService.Api/EnigmaVault.AuthenticationService.Api.csproj"
COPY . .
WORKDIR "/src/EnigmaVault.AuthenticationService.Api"
RUN dotnet build "./EnigmaVault.AuthenticationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EnigmaVault.AuthenticationService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EnigmaVault.AuthenticationService.Api.dll"]

# Релиз