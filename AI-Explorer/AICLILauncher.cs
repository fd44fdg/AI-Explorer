using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WJJ.AIExplorer;

/// <summary>
/// AI CLI 工具检测和启动器
/// </summary>
public static class AICLILauncher
{
    private static readonly Dictionary<AICommandType, ToolInfo> ToolRegistry = new()
    {
        [AICommandType.ClaudeCode] = new ToolInfo
        {
            Name = "Claude Code",
            ExecutableNames = new[] { "claude", "claude.bat", "claude.cmd", "claude.exe", "claude.ps1" },
            InstallationPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Claude", "claude.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "node_modules", "claude-code", "bin", "claude.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Programs", "claude-code", "resources", "cli", "bin", "claude.cmd"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "claude.cmd")
            },
            Description = "Anthropic's AI coding assistant"
        },
        [AICommandType.GeminiCLI] = new ToolInfo
        {
            Name = "Gemini CLI",
            ExecutableNames = new[] { "gemini", "gemini.bat", "gemini.cmd", "gemini.exe", "gemini.ps1" },
            InstallationPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "gemini-cli", "gemini.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "node_modules", "google-gemini-cli", "bin", "gemini.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".gemini", "bin", "gemini.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "gemini.cmd")
            },
            Description = "Google's Gemini AI CLI"
        },
        [AICommandType.Codex] = new ToolInfo
        {
            Name = "Codex CLI",
            ExecutableNames = new[] { "codex", "codex.bat", "codex.cmd", "codex.exe", "codex.ps1", "openai-codex.bat" },
            InstallationPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "codex", "codex.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "node_modules", "@openai", "codex", "bin", "codex.js"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "npm", "codex.cmd")
            },
            Description = "OpenAI's Codex CLI"
        },
        [AICommandType.IFlow] = new ToolInfo
        {
            Name = "iFlow CLI",
            ExecutableNames = new[] { "iflow", "iflow.bat", "iflow.cmd", "iflow.exe", "iflow.ps1" },
            InstallationPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "iFlow", "iflow.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".iflow", "bin", "iflow.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "iFlow", "iflow.bat"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "iFlow", "iflow.bat")
            },
            Description = "iFlow AI CLI"
        }
    };

    /// <summary>
    /// 获取所有可用的 AI CLI 工具
    /// </summary>
    public static List<AICommandType> GetAvailableTools()
    {
        var available = new List<AICommandType>();

        foreach (var tool in ToolRegistry)
        {
            if (IsToolInstalled(tool.Key))
            {
                available.Add(tool.Key);
            }
        }

        return available;
    }

    /// <summary>
    /// 检查指定工具是否已安装
    /// </summary>
    public static bool IsToolInstalled(AICommandType type)
    {
        if (!ToolRegistry.TryGetValue(type, out var toolInfo))
        {
            return false;
        }

        // 检查注册表中的安装路径
        foreach (var path in toolInfo.InstallationPaths)
        {
            if (File.Exists(path))
            {
                return true;
            }
        }

        // 直接在 PATH 中搜索
        var foundInPath = FindExecutableInPath(toolInfo.ExecutableNames);
        return foundInPath != null;
    }

    /// <summary>
    /// 获取工具的完整路径
    /// </summary>
    public static string? GetToolPath(AICommandType type)
    {
        if (!ToolRegistry.TryGetValue(type, out var toolInfo))
        {
            return null;
        }

        // 1. 检查注册表中的路径
        foreach (var path in toolInfo.InstallationPaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        // 2. 在 PATH 中搜索
        return FindExecutableInPath(toolInfo.ExecutableNames);
    }

    /// <summary>
    /// 启动 AI CLI 工具
    /// </summary>
    public static bool LaunchTool(AICommandType type, string workingDirectory)
    {
        var toolInfo = GetToolInfo(type);
        if (toolInfo == null)
        {
            return false;
        }
        
        // Use the simple command name (first entry, e.g., "codex", "claude", "gemini")
        // Let cmd.exe handle PATH resolution
        var commandName = toolInfo.ExecutableNames.FirstOrDefault() ?? type.ToString().ToLowerInvariant();

        try
        {
            // Use cmd.exe /k to launch CLI tools in a new window
            // This lets the shell handle PATH resolution properly
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/k cd /d \"{workingDirectory}\" && {commandName}",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process.Start(startInfo);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to launch {type}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取工具信息
    /// </summary>
    public static ToolInfo? GetToolInfo(AICommandType type)
    {
        return ToolRegistry.TryGetValue(type, out var info) ? info : null;
    }

    private static string? FindExecutableInPath(string[] executableNames)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        var paths = pathEnv.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var dir in paths)
        {
            if (!Directory.Exists(dir))
            {
                continue;
            }

            foreach (var exeName in executableNames)
            {
                var exePath = Path.Combine(dir, exeName);
                if (File.Exists(exePath))
                {
                    return exePath;
                }
            }
        }

        return null;
    }
}

/// <summary>
/// AI CLI 工具信息
/// </summary>
public class ToolInfo
{
    public string Name { get; set; } = string.Empty;
    public string[] ExecutableNames { get; set; } = Array.Empty<string>();
    public string[] InstallationPaths { get; set; } = Array.Empty<string>();
    public string Description { get; set; } = string.Empty;
}
