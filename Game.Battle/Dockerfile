FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Game.Battle/Game.Battle.csproj", "Game.Battle/"]
COPY ["Game.SharedKernel/Game.SharedKernel.csproj", "Game.SharedKernel/"]
RUN dotnet restore "Game.Battle/Game.Battle.csproj"
COPY . .
WORKDIR "/src/Game.Battle"
RUN dotnet publish "Game.Battle.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Game.Battle.dll"]
