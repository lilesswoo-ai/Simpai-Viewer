# SimpaiViewer 更新内容 v1.9.1

## 文件夹管理改进

* 根文件夹现在在文件夹视图中管理。
* 监视与递归扫描设置现在按根文件夹独立配置。
* 排除的文件夹现在通过树视图设置。

## 其他

* 修复 A1111 风格元数据中以花括号 `{` 开头的提示词解析。
* 支持按文件大小排序。
* 大量与文件夹重命名等相关的修复。
* 修复根目录（如 `X:\`）根文件夹名称显示为空的问题。
* 修复上次更新导致的自动刷新失效。
* 修复查询中的日期搜索。
* 防止点击查询输入框编辑时误关闭。
* 记住预览窗口的最后位置和状态。

---

# SimpaiViewer Phase 1

* 从 Diffusion Toolkit 分叉并品牌化，命名为 **SimpaiViewer**。
* 默认界面语言：中文（简体）；英文仍可在 设置 > 常规 > 语言 中切换。
* 默认主题：深色（冷启动无白色闪烁）。
* 元数据面板重构：参数逐行显示，标签对齐，等宽字体值（不再是整段文字墙）。
* 预览面板：鼠标滚轮切换图片；Ctrl+滚轮缩放；双击进入/退出全屏。
* 相邻缩略图预取（±3），支持大文件夹中快速连续浏览。

## SimpAI 元数据支持

* 支持解析 SimpAI（基于 Fooocus 扩展格式）生成的 JPEG/PNG 图片元数据。
* 读取 EXIF UserComment 段中的 UTF-16BE JSON（`UNICODE\0` 8 字节头 + JSON）。
* 完整显示：Steps、Sampler、Guidance Scale、Seed、Resolution、Base Model、Model Hash、Styles、Backend Engine、CLIP/文本编码器、VAE、放大模型、锐度、性能、调度器、ADM 引导、版本 等字段。
* 中文提示词完整显示，样式（Styles）作为独立参数行展示。
* 原始元数据以美化后的 JSON 展示，可读性大幅提升。

## 默认设置

* 默认语言：中文（zh-CN）。
* 默认主题：深色。
* 默认启用滚轮切换图片（ScrollNavigation）。
* 配置文件独立存放于 SimpaiViewer 专属路径，不再继承旧版 DiffusionToolkit 配置。
