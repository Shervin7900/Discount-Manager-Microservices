@echo off
setlocal enabledelayedexpansion

set ENV=%1
if "%ENV%"=="" set ENV=dev
set ACTION=%2

echo ========================================
echo   Base API Build System (Windows)
echo   Target Environment: %ENV%
echo ========================================

echo [1/3] Restoring and Building .NET project...
dotnet restore BaseApi/BaseApi.csproj
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet build BaseApi/BaseApi.csproj -c Release --no-restore
if %errorlevel% neq 0 exit /b %errorlevel%

echo [2/3] Building Docker Image (%ENV%)...
docker build -t base-api:%ENV% BaseApi/
if %errorlevel% neq 0 exit /b %errorlevel%

if "%ACTION%"=="up" (
    echo [3/3] Starting stack with Docker Compose...
    REM We could use -f docker-compose.%ENV%.yml if they existed
    docker-compose up -d
) else (
    echo [3/3] Build complete. Use 'build.bat %ENV% up' to start services.
)

echo ========================================
echo   Done.
echo ========================================
