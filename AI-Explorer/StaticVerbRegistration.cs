using System;
using System.IO;
using Microsoft.Win32;

namespace WJJ.AIExplorer;

/// <summary>
/// 静态动词注册 - 使用纯注册表方式，无需 COM
/// </summary>
public static class StaticVerbRegistration
{
    private const string MenuName = "AI 工具";
    private const string VerbName = "AIExplorer";

    /// <summary>
    /// 注册静态动词到右键菜单
    /// </summary>
    public static void Register()
    {
        var exePath = typeof(StaticVerbRegistration).Assembly.Location;
        
        // For single-file publish or WinExe, get the actual exe path
        var processPath = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(processPath) && processPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            exePath = processPath;
        }

        Console.WriteLine($"Registering Static Verb from: {exePath}");

        try
        {
            // Register under Directory\Background\shell (folder background right-click)
            // Path: HKLM\Software\Classes\Directory\Background\shell\WJJ.AIExplorer
            var shellKeyPath = $@"Software\Classes\Directory\Background\shell\{VerbName}";
            
            using var shellKey = Registry.LocalMachine.CreateSubKey(shellKeyPath);
            shellKey?.SetValue(null, MenuName);
            // Use a system "Star" icon (shell32.dll, index 43) to represent AI/Magic
            shellKey?.SetValue("Icon", "shell32.dll,43");
            
            // Command subkey
            using var commandKey = shellKey?.CreateSubKey("command");
            // %V is the folder path that was right-clicked in
            commandKey?.SetValue(null, $"\"{exePath}\" \"%V\"");

            Console.WriteLine("Static verb registered successfully!");
            Console.WriteLine("菜单项 'AI 工具' 已添加到文件夹背景右键菜单。");
            Console.WriteLine("请重启 Windows Explorer 以使更改生效。");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("错误: 权限不足。请以管理员身份运行。");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"注册错误: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 注销静态动词
    /// </summary>
    public static void Unregister()
    {
        Console.WriteLine("Unregistering Static Verb...");

        try
        {
            // Remove from HKLM
            var shellKeyPath = $@"Software\Classes\Directory\Background\shell\{VerbName}";
            Registry.LocalMachine.DeleteSubKeyTree(shellKeyPath, false);

            // Also clean up any old COM registrations
            try { Registry.LocalMachine.DeleteSubKeyTree(@"Software\Classes\Directory\Background\shellex\ContextMenuHandlers\WJJ.AIExplorer", false); } catch {}
            try { Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\Directory\Background\shellex\ContextMenuHandlers\WJJ.AIExplorer", false); } catch {}

            Console.WriteLine("Static verb unregistered successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"注销警告: {ex.Message}");
        }
    }
}
