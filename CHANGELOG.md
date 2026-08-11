# 更新说明 / Changelog

本文件记录 SimpaiViewer.NET 各版本的变更。详细的发布说明同时发布在 GitHub Releases。

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
