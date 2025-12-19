@echo off
setlocal

:: Quickstart script to build the solution and launch the console overlay sample
:: (includes the ProducerPanelController driving the StandingsTowerWidget and DriverInfoWidget).

where dotnet >nul 2>&1
if errorlevel 1 (
    echo .NET SDK not found. Please install the .NET 8 SDK and re-run.
    exit /b 1
)

set CONFIG=Release
if not "%1"=="" set CONFIG=%1

echo Restoring NuGet packages...
dotnet restore IRSDKSharper.sln
if errorlevel 1 exit /b 1

echo Building IRSDKSharper solution (%CONFIG%)...
dotnet build IRSDKSharper.sln -c %CONFIG%
if errorlevel 1 exit /b 1

echo Launching overlay console sample with producer panel controller...
dotnet run --project OverlayConsoleApp/OverlayConsoleApp.csproj -c %CONFIG%

endlocal
