FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/infrastructure/EarthChat.Infrastructure.Gateway/EarthChat.Infrastructure.Gateway.csproj", "src/infrastructure/EarthChat.Infrastructure.Gateway/"]
RUN dotnet restore "src/infrastructure/EarthChat.Infrastructure.Gateway/EarthChat.Infrastructure.Gateway.csproj"
COPY . .
WORKDIR "/src/src/infrastructure/EarthChat.Infrastructure.Gateway"
RUN dotnet build "EarthChat.Infrastructure.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EarthChat.Infrastructure.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EarthChat.Infrastructure.Gateway.dll"]
