# AI Explorer 卸载脚本
# 使用方法: 右键点击"以 PowerShell 管理员身份运行"

$ErrorActionPreference = "Continue"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  AI Explorer Shell Extension Uninstaller" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 获取脚本所在目录
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Join-Path $scriptDir "AI-Explorer"
$outputDir = Join-Path $projectDir "bin\Release\net8.0-windows10.0.19041.0"
$exePath = Join-Path $outputDir "AIExplorer.exe"

# 检查是否以管理员身份运行
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "[!] 请以管理员身份运行此脚本！" -ForegroundColor Red
    exit 1
}

# 1. 尝试通过 EXE 注销
if (Test-Path $exePath) {
    Write-Host "正在通过程序注销..." -ForegroundColor Yellow
    & $exePath --unregister
}

# 2. 强制清理所有可能的注册表残留 (旧版和新版)
Write-Host "正在清理注册表残留..." -ForegroundColor Yellow

# 旧版 WJJ.AIExplorer (HKLM)
Remove-Item -Path "HKLM:\Software\Classes\Directory\Background\shell\WJJ.AIExplorer" -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path "HKLM:\Software\Classes\Directory\Background\shellex\ContextMenuHandlers\WJJ.AIExplorer" -Recurse -ErrorAction SilentlyContinue

# 新版 AIExplorer (HKLM)
Remove-Item -Path "HKLM:\Software\Classes\Directory\Background\shell\AIExplorer" -Recurse -ErrorAction SilentlyContinue

# 清理可能存在的用户级别注册 (HKCU)
Remove-Item -Path "HKCU:\Software\Classes\Directory\Background\shell\WJJ.AIExplorer" -Recurse -ErrorAction SilentlyContinue
Remove-Item -Path "HKCU:\Software\Classes\Directory\Background\shell\AIExplorer" -Recurse -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  卸载完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "请重启 Windows Explorer 以刷新菜单。"
Write-Host ""
