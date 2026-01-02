using System;
using System.IO;
using System.Windows.Forms;

namespace WJJ.AIExplorer;

/// <summary>
/// Shell 扩展安装程序入口点
/// </summary>
internal class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        // If no arguments, assume user wants to open tools for Current Directory (Double-click mode)
        // This allows placing the EXE (or shortcut) in any folder and just clicking it
        if (args.Length == 0)
        {
            ShowPicker(Environment.CurrentDirectory);
            return 0;
        }

        var command = args[0].ToLowerInvariant();

        try
        {
            switch (command)
            {
                case "--register":
                case "-r":
                case "register":
                    StaticVerbRegistration.Register();
                    break;

                case "--unregister":
                case "-u":
                case "unregister":
                    StaticVerbRegistration.Unregister();
                    break;

                case "--help":
                case "-h":
                case "help":
                case "?":
                    PrintUsage();
                    break;

                default:
                    // If the argument is a path, treat it as the working directory and show picker
                    if (Directory.Exists(args[0]))
                    {
                        ShowPicker(args[0]);
                    }
                    else
                    {
                        Console.WriteLine($"Unknown command or invalid path: {command}");
                        PrintUsage();
                        return 1;
                    }
                    break;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static void ShowPicker(string workingDirectory)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new AIToolPicker(workingDirectory));
    }

    private static void PrintUsage()
    {
        Console.WriteLine(@"
AI Explorer - Windows Explorer AI Tools Shell Extension
=========================================================

Usage:
  AIExplorer.exe [command]
  AIExplorer.exe [directory_path]
  AIExplorer.exe (no args) -> Opens for current directory

Commands:
  --register, -r, register   Register the shell extension
  --unregister, -u, unregister  Unregister the shell extension
  --help, -h, help           Show this help message

If a directory path is provided (or no args), the AI Tool Picker will open.

Examples:
  AIExplorer.exe --register
  AIExplorer.exe --unregister
  AIExplorer.exe ""C:\MyProject""

Portable Usage:
  Copy AIExplorer.exe (and dependencies) to any folder, 
  then double-click it to open tools for that folder.
");
    }
}
