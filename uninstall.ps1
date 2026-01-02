# WJJ.AIExplorer 卸载脚本
# 使用方法: 右键点击"以 PowerShell 管理员身份运行"

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  WJJ.AIExplorer Shell Extension Uninstaller" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 获取脚本所在目录
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectDir = Join-Path $scriptDir "WJJ.AIExplorer"
$outputDir = Join-Path $projectDir "bin\Release\net10.0-windows10.0.19041.0"

Write-Host "项目目录: $projectDir" -ForegroundColor Yellow
Write-Host ""

# 检查是否以管理员身份运行
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Warning "建议以管理员身份运行此脚本以确保完全注销。"
    Write-Host ""
}

# 查找 DLL 文件
$dllPath = Join-Path $outputDir "WJJ.AIExplorer.dll"

# 如果输出目录不存在，尝试构建
if (-not (Test-Path $dllPath)) {
    Write-Host "正在构建项目..." -ForegroundColor Yellow
    try {
        dotnet build $projectDir -c Release | Out-Null
    } catch {
        Write-Warning "构建失败，但将继续尝试卸载..."
    }
}

# 注销 COM 组件
Write-Host "正在注销 Shell 扩展..." -ForegroundColor Yellow

try {
    if (Test-Path $dllPath) {
        # 方法 1: 使用 dotnet 命令注销
        $unregResult = dotnet $dllPath --unregister 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "注销成功!" -ForegroundColor Green
            $unregResult | ForEach-Object { Write-Host "  $_" }
        } else {
            Write-Warning "dotnet 注销失败..."
            Write-Host $unregResult
        }
    }
} catch {
    Write-Warning "注销过程中出现错误: $_"
}

# 手动清理注册表（备用方法）
Write-Host ""
Write-Host "正在清理注册表..." -ForegroundColor Yellow

$cleanupScript = @"
`$ErrorActionPreference = "SilentlyContinue"

# 清理 CommandStore 注册
Remove-Item -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\WJJ.AIExplorer" -Recurse -ErrorAction SilentlyContinue

# 清理 ContextMenus 注册
Remove-Item -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\ContextMenus\FolderOpen" -Recurse -ErrorAction SilentlyContinue

# 清理 Approved 注册
Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved" -Name "A7B8A5B1-4E5C-4D7A-9F8D-8E3A8D7C1B2E" -ErrorAction SilentlyContinue

Write-Host "注册表清理完成"
"@

try {
    $cleanupResult = powershell -Command $cleanupScript
    if ($cleanupResult) {
        $cleanupResult | ForEach-Object { Write-Host "  $_" }
    }
} catch {
    Write-Warning "注册表清理失败: $_"
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  卸载完成!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "请执行以下任一操作以完全移除扩展:"
Write-Host "  1. 重启 Windows Explorer"
Write-Host "     - 打开任务管理器，找到'Windows Explorer'，点击'重启'"
Write-Host ""
Write-Host "  2. 或注销后重新登录"
Write-Host ""
Write-Host ""
