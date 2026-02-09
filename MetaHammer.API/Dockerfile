# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["MetaHammer.API/MetaHammer.API.csproj", "MetaHammer.API/"]
COPY ["MetaHammer.Application/MetaHammer.Application.csproj", "MetaHammer.Application/"]
COPY ["MetaHammer.Domain/MetaHammer.Domain.csproj", "MetaHammer.Domain/"]
COPY ["MetaHammer.Persistence.Neo4j/MetaHammer.Persistence.Neo4j.csproj", "MetaHammer.Persistence.Neo4j/"]

RUN dotnet restore "MetaHammer.API/MetaHammer.API.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/MetaHammer.API"
RUN dotnet build "MetaHammer.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "MetaHammer.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MetaHammer.API.dll"]
