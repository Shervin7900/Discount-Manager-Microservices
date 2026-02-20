#!/bin/bash
set -e

ENV=${1:-dev}
ACTION=${2:-none}

echo "========================================"
echo "  Base API Build System (Linux/macOS)"
echo "  Target Environment: $ENV"
echo "========================================"

# Check for .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "[ERROR] .NET SDK not found. Please install .NET 10 SDK."
    exit 1
fi

# Check for Docker
if ! command -v docker &> /dev/null; then
    echo "[ERROR] Docker not found. Please install Docker."
    exit 1
fi

echo "[1/3] Restoring and Building .NET project..."
dotnet build BaseApi/BaseApi.csproj -c Release

echo "[2/3] Building Docker Image..."
docker build -t base-api:$ENV BaseApi/

if [ "$ACTION" == "up" ]; then
    echo "[3/3] Starting stack with Docker Compose..."
    docker-compose up -d
else
    echo "[3/3] Build complete. Use './builder.sh $ENV up' to start services."
fi

echo "========================================"
echo "  Done."
echo "========================================"
