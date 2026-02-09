# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["MetaHammer.Client/MetaHammer.Client.csproj", "MetaHammer.Client/"]
COPY ["MetaHammer.Application/MetaHammer.Application.csproj", "MetaHammer.Application/"]
COPY ["MetaHammer.Domain/MetaHammer.Domain.csproj", "MetaHammer.Domain/"]

RUN dotnet restore "MetaHammer.Client/MetaHammer.Client.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/MetaHammer.Client"
RUN dotnet build "MetaHammer.Client.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "MetaHammer.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage (Nginx)
FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY MetaHammer.Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
