# AI Explorer 强力修复脚本
# 使用方法: 右键点击"以 PowerShell 管理员身份运行"

Write-Host "正在执行彻底修复程序..." -ForegroundColor Cyan

# 1. 杀死所有可能占用文件的进程
Write-Host "正在清理进程冲突..." -ForegroundColor Yellow
Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Stop-Process -Name AIExplorer -Force -ErrorAction SilentlyContinue
Stop-Process -Name WJJ.AIExplorer -Force -ErrorAction SilentlyContinue

# 2. 彻底清理注册表
Write-Host "正在重置注册表状态..." -ForegroundColor Yellow
$keys = @(
    "HKLM:\Software\Classes\Directory\Background\shell\AIExplorer",
    "HKLM:\Software\Classes\Directory\Background\shell\WJJ.AIExplorer",
    "HKCU:\Software\Classes\Directory\Background\shell\AIExplorer",
    "HKCU:\Software\Classes\Directory\Background\shell\WJJ.AIExplorer"
)

foreach ($key in $keys) {
    if (Test-Path $key) {
        Remove-Item -Path $key -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "  已清理注册表项: $key"
    }
}

# 2.5 强力清理构建残留
Write-Host "正在清理构建残留..." -ForegroundColor Yellow
$binPath = Join-Path $scriptDir "AI-Explorer\bin"
if (Test-Path $binPath) { Remove-Item -Path $binPath -Recurse -Force -ErrorAction SilentlyContinue }

$icoPath = Join-Path $scriptDir "assets\app_icon.ico"
if (Test-Path $icoPath) { Remove-Item -Path $icoPath -Force -ErrorAction SilentlyContinue }

# 3. 运行安装脚本
Write-Host "正在重新安装..." -ForegroundColor Yellow
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $scriptDir "install.ps1")

# 4. 重启资源管理器
Write-Host "正在重启资源管理器..." -ForegroundColor Yellow
Start-Process explorer.exe

Write-Host ""
Write-Host "修复完成！请再次尝试右键菜单。" -ForegroundColor Green
