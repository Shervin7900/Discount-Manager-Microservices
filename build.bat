@echo off
setlocal enabledelayedexpansion

set ENV=%1
if "%ENV%"=="" set ENV=dev
set ACTION=%2

echo ========================================
echo   Base API Build System (Windows)
echo   Target Environment: %ENV%
echo ========================================

REM Check for .NET SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK not found. Please install .NET 10 SDK.
    exit /b 1
)

REM Check for Docker
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Docker not found. Please install Docker Desktop.
    exit /b 1
)

echo [1/3] Restoring and Building .NET project...
dotnet build BaseApi/BaseApi.csproj -c Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo [2/3] Building Docker Image...
docker build -t base-api:%ENV% BaseApi/
if %errorlevel% neq 0 exit /b %errorlevel%

if "%ACTION%"=="up" (
    echo [3/3] Starting stack with Docker Compose...
    docker-compose up -d
) else (
    echo [3/3] Build complete. Use 'build.bat %ENV% up' to start services.
)

echo ========================================
echo   Done.
echo ========================================
