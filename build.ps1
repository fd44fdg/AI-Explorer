$ErrorActionPreference = "Continue"
$logPath = "E:\学习与项目\CS\项目\WJJ\build.log"

try {
    $process = Start-Process -FilePath "dotnet" -ArgumentList "build", "E:\学习与项目\CS\项目\WJJ\WJJ.AIExplorer\WJJ.AIExplorer.csproj", "-c", "Release" -PassThru -NoNewWindow -Wait

    $exitCode = $process.ExitCode

    if ($exitCode -ne 0) {
        Write-Host "Build failed with exit code: $exitCode"
        Write-Host "Please check the build log for details."
    }

    exit $exitCode
} catch {
    Write-Host "Error: $_"
    exit 1
}
