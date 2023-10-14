#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/assistant/Chat.SemanticServer/Chat.SemanticServer.csproj", "src/assistant/Chat.SemanticServer/"]
COPY ["src/Chat.Contracts/Chat.Contracts.csproj", "src/Chat.Contracts/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/assistant/Chat.SemanticServer/Chat.SemanticServer.csproj"
COPY . .
WORKDIR "/src/src/assistant/Chat.SemanticServer"
RUN dotnet build "Chat.SemanticServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Chat.SemanticServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chat.SemanticServer.dll"]