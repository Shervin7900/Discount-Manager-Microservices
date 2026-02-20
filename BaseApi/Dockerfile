# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy only the project file and restore to cache dependencies
COPY ["BaseApi.csproj", "./"]
RUN dotnet restore "./BaseApi.csproj"

# Copy the rest of the source and build
COPY . .
RUN dotnet build "BaseApi.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "BaseApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Run as non-root user for security
USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BaseApi.dll"]
