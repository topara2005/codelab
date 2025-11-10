# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY DemoApp/DemoApp.csproj DemoApp/

# Restore dependencies
RUN dotnet restore DemoApp/DemoApp.csproj

# Copy source code for the main project
COPY DemoApp/ DemoApp/

# Build the application
WORKDIR /src/DemoApp
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApp.dll"]
