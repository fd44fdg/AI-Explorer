# AI Explorer Installation Script
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  AI Explorer Shell Extension Installer" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check for admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "[!] Warning: Not running as Administrator. Registration may fail." -ForegroundColor Yellow
    Write-Host "    Please run PowerShell as Administrator and try again." -ForegroundColor Yellow
    Write-Host ""
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Join-Path $scriptDir "AI-Explorer"
$outputDir = Join-Path $projectDir "bin\Release\net8.0-windows10.0.19041.0"

Write-Host "Project: $projectDir" -ForegroundColor Yellow
Write-Host ""

# Build
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build $projectDir -c Release

$exePath = Join-Path $outputDir "AIExplorer.exe"

# Copy assets to output directory
$targetAssetsDir = Join-Path $outputDir "assets"
if (-not (Test-Path $targetAssetsDir)) { New-Item -ItemType Directory -Path $targetAssetsDir -Force | Out-Null }
Copy-Item (Join-Path $scriptDir "assets\*") $targetAssetsDir -Force

if (-not (Test-Path $exePath)) {
    Write-Error "Build failed - EXE not found at $exePath"
    exit 1
}

Write-Host "Found EXE: $exePath" -ForegroundColor Green
Write-Host ""

# Register using the exe directly
Write-Host "Registering Shell Extension..." -ForegroundColor Yellow
& $exePath --register

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  Installation Complete!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Please restart Explorer to activate:"
Write-Host "  1. Open Task Manager"
Write-Host "  2. Find 'Windows Explorer' and click 'Restart'"
Write-Host ""
Write-Host "Or run: Stop-Process -Name explorer -Force" -ForegroundColor Gray
Write-Host ""