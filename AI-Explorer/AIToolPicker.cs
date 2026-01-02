using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WJJ.AIExplorer;

/// <summary>
/// AI 工具选择器窗口
/// </summary>
public class AIToolPicker : Form
{
    private readonly string _workingDirectory;
    private readonly List<AICommandType> _availableTools;

    public AIToolPicker(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
        _availableTools = AICLILauncher.GetAvailableTools();

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Window settings
        Text = "AI 工具";
        Size = new Size(320, 280);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(30, 30, 30);
        ForeColor = Color.White;

        // Title label
        var titleLabel = new Label
        {
            Text = "选择要启动的 AI 工具：",
            Location = new Point(20, 15),
            Size = new Size(280, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.White
        };
        Controls.Add(titleLabel);

        // Directory label
        var dirLabel = new Label
        {
            Text = $"📁 {TruncatePath(_workingDirectory, 35)}",
            Location = new Point(20, 42),
            Size = new Size(280, 20),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.FromArgb(180, 180, 180)
        };
        Controls.Add(dirLabel);

        int buttonY = 75;
        
        // Create a button for each available tool
        foreach (var toolType in Enum.GetValues<AICommandType>())
        {
            var toolInfo = AICLILauncher.GetToolInfo(toolType);
            if (toolInfo == null) continue;

            bool isInstalled = _availableTools.Contains(toolType);

            var button = new Button
            {
                Text = $"  {toolInfo.Name}",
                Location = new Point(20, buttonY),
                Size = new Size(260, 38),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = isInstalled ? Cursors.Hand : Cursors.No,
                Enabled = isInstalled,
                BackColor = isInstalled ? Color.FromArgb(50, 50, 50) : Color.FromArgb(40, 40, 40),
                ForeColor = isInstalled ? Color.White : Color.FromArgb(100, 100, 100),
                Tag = toolType
            };
            
            button.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 70);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);

            if (isInstalled)
            {
                button.Click += OnToolButtonClick;
            }
            else
            {
                button.Text += " (未安装)";
            }

            Controls.Add(button);
            buttonY += 45;
        }

        // Adjust form height based on number of tools
        Height = buttonY + 30;
    }

    private void OnToolButtonClick(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.Tag is AICommandType toolType)
        {
            if (AICLILauncher.LaunchTool(toolType, _workingDirectory))
            {
                Close();
            }
            else
            {
                MessageBox.Show(
                    $"无法启动 {AICLILauncher.GetToolInfo(toolType)?.Name}。\n请检查工具是否正确安装。",
                    "启动失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }
    }

    private static string TruncatePath(string path, int maxLength)
    {
        if (path.Length <= maxLength) return path;
        return "..." + path.Substring(path.Length - maxLength + 3);
    }
}
