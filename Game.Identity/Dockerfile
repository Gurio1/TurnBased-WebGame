FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Game.Identity/Game.Identity.csproj", "Game.Identity/"]
COPY ["Game.SharedKernel/Game.SharedKernel.csproj", "Game.SharedKernel/"]
RUN dotnet restore "Game.Identity/Game.Identity.csproj"
COPY . .
WORKDIR "/src/Game.Identity"
RUN dotnet publish "Game.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Game.Identity.dll"]
