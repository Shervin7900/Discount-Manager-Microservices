#!/bin/bash
set -e

ENV=${1:-dev}
ACTION=${2:-none}

echo "========================================"
echo "  Base API Build System (Linux/macOS)"
echo "  Target Environment: $ENV"
echo "========================================"

echo "[1/3] Restoring and Building .NET project..."
dotnet restore BaseApi/BaseApi.csproj
dotnet build BaseApi/BaseApi.csproj -c Release --no-restore

echo "[2/3] Building Docker Image ($ENV)..."
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
