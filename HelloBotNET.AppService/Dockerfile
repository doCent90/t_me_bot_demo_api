FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HelloBotNET.AppService/HelloBotNET.AppService.csproj", "HelloBotNET.AppService/"]
RUN dotnet restore "./HelloBotNET.AppService/./HelloBotNET.AppService.csproj"
COPY . .
WORKDIR "/src/HelloBotNET.AppService"
RUN dotnet build "./HelloBotNET.AppService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HelloBotNET.AppService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
ENTRYPOINT ["dotnet", "HelloBotNET.AppService.dll"]