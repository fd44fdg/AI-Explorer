[English](#english) | [中文](#chinese)

---

<a name="english"></a>
# AI Explorer 🚀

**AI Explorer** is an ultra-lightweight Windows context menu tool. It allows you to instantly launch AI assistants like **Claude Code** or **Gemini CLI** with a single right-click in any folder.

## ✨ Features
- **Effortless**: No more manual `cd` or typing startup commands.
- **Minimalist**: Adds only one entry to the context menu, keeping it clean.
- **Fast**: Instant detection and one-click access to AI.

## 📸 Demo
![Demo](./assets/screenshot_new.png)

## 🚀 30s Quick Start
1. **Download and Extract** the project.
2. **Right-click** `install.ps1` and select "Run with PowerShell" (Administrator required).
3. **Done!** You will see **"AI Tools"** in the folder background right-click menu.

## 🛠 Currently Supported
- **Claude Code**
- **Gemini CLI**
- **Codex CLI**
- **iFlow CLI**

> [!TIP]
> AI Explorer automatically detects installed tools; tools not found will be disabled in the UI.

## 🛠️ How to Extend?
Want to add a new tool like `Aider`? It's just two steps:
1. **Add Enum**: Add the tool name to `AICommandType.cs`.
2. **Register Metadata**: Add the name and detection paths in `AICLILauncher.cs`.
The UI will update automatically!

## 🗑 Uninstallation
Simply run `uninstall.ps1` as Administrator to remove all traces.

---

<a name="chinese"></a>
# AI Explorer (中文版) 🚀

**AI Explorer** 是一个超轻量级的 Windows 右键菜单工具。让你在文件夹里点一下右键，就能立刻唤起 **Claude Code** 或 **Gemini CLI** 等 AI 助手。

## ✨ 为什么用它？
- **省心**：不用再费劲 `cd` 进文件夹，也不用手动输入启动命令。
- **干净**：右键菜单只加一个入口，不乱占地方。
- **快**：秒级检测，一键直达 AI。

## 📸 运行效果
![运行截图](./assets/screenshot_new.png)

## 🚀 30秒快速安装
1. **下载并解压** 整个项目。
2. **右键** `install.ps1`，选择“使用 PowerShell 运行”（需要管理员权限）。
3. **完成！** 现在你随便找个文件夹点右键，就能看到 **"AI 工具"** 菜单了。

## 🛠 目前支持
- **Claude Code**
- **Gemini CLI**
- **Codex CLI**
- **iFlow CLI**

> [!TIP]
> 如果你在系统里装了这些工具，AI Explorer 会自动发现它们；没装的工具会自动变灰，不会报错。

## 🛠️ 如何添加更多 AI 工具？
如果你想加入新的 CLI 工具（例如 `Aider` 或 `Cursor`），只需两步：
1. **添加枚举**：在 `AICommandType.cs` 中增加工具名称。
2. **注册元数据**：在 `AICLILauncher.cs` 的 `ToolRegistry` 中填入名称和检测路径。
界面会自动刷新，无需手动调整 UI！

## 🗑 卸载
想删掉？右键运行 `uninstall.ps1` 即可，不留任何痕迹。

## 📄 开源协议
[MIT License](./LICENSE)
