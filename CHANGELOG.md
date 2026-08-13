# 更新说明 / Changelog

本文件记录 SimpaiViewer.NET 各版本的变更。详细的发布说明同时发布在 GitHub Releases。

## v1.2.1（2026-08-13）

### 修复与变更
- **修复 SimpleAI / SimpAI 生成图的生成参数不显示**
  - 根因：这批图由 `SimpleAI.FluxAIO` 生成，其参数以两种方式存储，旧代码均读不到：
    1. 生成参数写在 EXIF `UserComment` 中，但**直接位于 IFD0（非标准 ExifIFD）**，且为**无 `UNICODE\0` 编码头的裸 UTF-8 JSON**；
    2. 版本检测只认 `SimpAI`，不认 `SimpleAI`，导致整段 JSON 被丢弃。
  - 修复（`Diffusion.Scanner/Metadata.cs`）：
    - `ReadExifUserComment` 新增 **IFD0 直接读取路径 + ASCII 类型**支持；
    - `TryReadUserCommentEntry` 支持**无编码头的裸 UTF-8 JSON**（`{`/`[` 开头直接按 UTF-8 解码）；
    - `IsSimpAIMetadata` 放宽 Version 检测，兼容 `SimpleAI` 与 `SimpAI`。
  - 实测：该图现已正确读出 Prompt / Model（如 `kolors_unet_fp16`）/ ADM Guidance / Backend Engine 等字段。

### 下载与安装
- 自包含版 `SimpaiViewer_v1.2.1_win_x64.zip`：解压后双击 `SimpaiViewer.exe` 即可运行，已内嵌 .NET 10 运行时，无需安装。
- 当前仅支持 Windows。

## v1.2.0（2026-08-12）

### 修复与变更
- **「入门指南」改为中文优先**
  - 帮助菜单的「入门指南」/ 按 `F1` 现在默认展示中文指南（`Tips.zh-CN.md`），不再因运行时语言设置而显示英文。
- **「检查更新」接入自家 GitHub 仓库**
  - 更新检查（`UpdateChecker`）与自动更新器（`Diffusion.Updater`）的查询目标由上游 `RupertAvery/DiffusionToolkit` 改为本仓库 `lilesswoo-ai/Simpai-Viewer`，现在能正确检查并下载 SimpaiViewer 自己的新版本。
  - 修正本地版本号识别（新增并随包发布 `version.txt`，内容 `v1.2.0`），此前因版本号文件缺失/沿用上游版本号导致更新判断错误。
  - 自动更新器 `Diffusion.Updater.exe` 已随包发布，检查到新版本时可直接从 GitHub 下载并覆盖安装。
- **版本号更新为 1.2.0**。

### 下载与安装
- 自包含版 `SimpaiViewer.NET-v1.2.0-win-x64-selfcontained.zip`：解压后双击 `SimpaiViewer.exe` 即可运行，已内嵌 .NET 10 运行时，无需安装。
- 当前仅支持 Windows。

## v1.1.1（2026-08-12）

基于 [DiffusionToolkit](https://github.com/RupertAvery/DiffusionToolkit)（MIT）派生，针对 SimpAI Studio / simpai 工作流做了本地化与体验增强。

### 修复与变更
- **修复「帮助 → 入门指南」/ 按 `F1` 报错"找不到文件"并卡死的问题**
  - 入门指南窗口（`TipsWindow`）增加资源加载容错：加载失败时回退到英文说明并给出友好提示，不再卡死界面。
  - 在线帮助按钮改为直接打开项目 README。
- **移除「工具 → 下载 Civitai 模型」功能**
  - 删除该菜单、命令及对应方法，避免出错卡死。
  - 保留基于哈希的 Civitai 模型名反查能力（看图时把模型 hash 还原成模型名）。
- **文档完善（README）**
  - 新增「模型路径设置是做什么的？」说明（模型根目录 / A1111 `cache.json` 的作用）。
  - 翻译并补充上游 DiffusionToolkit FAQ（中文）。
  - 新增「支持作者」建议（推荐微信二维码为主、打赏按钮为辅）。

### 下载与安装
- 自包含版 `SimpaiViewer.NET-v1.1.1-win-x64-selfcontained.zip`：解压后双击 `SimpaiViewer.exe` 即可运行，已内嵌 .NET 10 运行时，无需安装。
- 当前仅支持 Windows。

## v1.1.0（2026-08-11）
- 自包含发行版初始发布。
