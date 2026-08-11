# SimpaiViewer.NET

> 一款面向 **AI 生图（Stable Diffusion / simpai / Fooocus 等）** 的本地图片元数据管理与看图工具。
> 基于 [DiffusionToolkit](https://github.com/RupertAvery/DiffusionToolkit)（MIT，作者 Rupert Avery）派生，针对 **SimpAI Studio** 工作流做了本地化与体验增强。

> SimpaiViewer.NET is a fork of [Diffusion Toolkit](https://github.com/RupertAvery/DiffusionToolkit) by Rupert Avery. Upstream credit and the MIT license are preserved.

---

## 一、这是什么

SimpaiViewer.NET 帮你把越来越多、越来越乱的 AI 生图 **组织、检索、排序、看图**。

- 扫描图片/视频，自动提取并索引**提示词（Prompt）**、**负面提示词**、**模型名**、**LoRA**、采样参数等元数据；
- 在预览面板里以**整齐的中文排版**展示元数据；
- 支持评分、收藏、NSFW 标记、相册、自定义标签、文件夹视图、缩略图缓存等。

---

## 二、相对上游新增的能力（本 fork 重点）

| 能力 | 说明 |
|------|------|
| 品牌更名 | 程序名 `SimpaiViewer.NET`，标题栏/图标/更新器路径同步更新 |
| 简体中文界面 | 完整中文本地化（zh-CN），界面默认中文 |
| 深色主题 | 默认开启深色模式，深色下文字改为白字加粗，保证可读 |
| 鼠标滚轮切图 | 在**预览面板**和**全屏**状态下，滚轮向上/向下切换上一张/下一张 |
| 双击全屏 | 预览区**双击进入全屏**，全屏下**双击或按 Esc 退出** |
| 相邻预取 | 光标切换时预取相邻缩略图（±3，去重上限 60 张），翻图更跟手 |
| 元数据面板重整 | 提示词 / 负面提示词 / 模型名称**置顶**；生成参数**分 3 列紧凑显示**；LoRA 高亮；已支持复制原始元数据 |
| simpai 元数据解析 | 自动识别 SimpAI Studio 导出图的元数据格式并整齐解析（见下文「simpai 元数据说明」） |
| 配置隔离 | 设置单独保存在 `AppData\SimpaiViewer`，不污染上游配置；缩略图数据库仍沿用 `dt_thumbnails.db` |

---

## 三、功能特性（沿用上游）

- 扫描图片与视频，存储并索引提示词及其他元数据（PNGInfo）
- 轻松看图并查看元数据
- 通过元数据检索图片/视频
- 打标签：收藏（Favorite）、评分（1–10）、NSFW
- 排序：按创建时间 / 美学分 / 评分
- 关键词自动标 NSFW，并对 NSFW 图片模糊处理
- 相册（Album）：选中图片右键「加入相册」，或拖拽到相册
- 自定义标签、文件夹视图
- 提示词与负面提示词的使用统计与反查
- 拖拽移动图片（Ctrl+拖拽为复制）

### 支持的图片格式

- JPG / JPEG（+EXIF）
- PNG
- WebP
- `.txt` 元数据
- MP4（视频）

### 支持的元数据格式

AUTOMATIC1111 及兼容格式（Tensor.Art、SDNext）、InvokeAI、NovelAI、Stable Diffusion、EasyDiffusion、RuinedFooocus、Fooocus、FooocusMRE、Stable Swarm，以及 **simpai（SimpAI Studio）**。

---

## 四、下载与安装

### 方式 A：自包含版（推荐，免运行时）

1. 在 [Releases](https://github.com/lilesswoo-ai/SimpaiViewer.NET/releases) 下载
   `SimpaiViewer.NET-vX.X.X-win-x64-selfcontained.zip`；
2. 解压到任意文件夹；
3. 双击 `SimpaiViewer.exe` 即可运行，**无需安装任何 .NET 运行时**。

> 自包含版体积约 250 MB，因为已内嵌 .NET 10 运行时的全部依赖。

### 方式 B：框架依赖版（体积小，需先装运行时）

1. 先安装 [.NET 10 桌面运行时（Windows Desktop Runtime）](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)；
2. 下载 `SimpaiViewer.NET-vX.X.X-win-x64.zip`（框架依赖版）；
3. 解压后运行 `SimpaiViewer.exe`。

> 当前仅支持 **Windows**。

---

## 五、快速上手

1. 首次启动：在设置里确认**语言=简体中文**、**主题=深色**、**滚轮切图=开**（默认即如此）；
2. 左侧「文件夹视图」添加你的生图输出目录（如 `I:\SimpAI\users\Local\outputs`）；
3. 右键文件夹 → **扫描 / 重建元数据**，等待索引完成；
4. 在缩略图区选中图片，右侧预览面板自动显示元数据；
5. **按 `I`** 显示/隐藏元数据面板（预览面板聚焦时同样有效）；
6. **鼠标滚轮**在预览/全屏时切换上一张/下一张；
7. **双击预览图**进入全屏，**双击/Esc** 退出全屏。

---

## 六、键盘快捷键

| 按键 | 作用 |
|------|------|
| `I` | 显示/隐藏元数据面板 |
| 鼠标滚轮（预览/全屏） | 上一张 / 下一张 |
| 双击预览图 | 进入全屏 |
| 双击 / `Esc`（全屏） | 退出全屏 |
| ← / → | 上一张 / 下一张（缩略图区聚焦时） |
| 拖拽图片到文件夹 | 移动（Ctrl+拖拽为复制） |

---

## 七、simpai 元数据说明

SimpAI Studio 导出的图片把生图参数写在 JPEG 的 **EXIF UserComment（0x9286）** 字段中，格式为：

```
"UNICODE\0" 头（8 字节） + UTF-16BE 编码的 JSON
```

本工具在扫描时自动识别该格式（`MetaFormat.SimpAI`），解析出：

- 提示词（Prompt）
- 负面提示词（Negative Prompt）
- 模型名称（Model Name，置顶高亮）
- LoRA（若有，高亮显示）
- 采样器、步数、CFG、尺寸、随机种子等生成参数

并在元数据面板中按「提示词/负面提示词/模型置顶 + 3 列紧凑参数 + LoRA 高亮」的方式整齐呈现，等价于原 simpai 项目的整洁显示风格。

> 注：纯 UTF-16BE 的 EXIF UserComment 在部分老旧查看器里显示为乱码，属正常现象——请用本工具查看。

---

## 八、缩略图缓存

- 每个被扫描的文件夹下会生成 `dt_thumbnails.db`（SQLite 数据库）；
- 以 **文件名 + 文件大小** 作为键缓存缩略图，加速二次访问；
- 缓存位于**图片所在文件夹内**。移动/重命名图片后原缓存键失效，需重新扫描或重建以刷新。

---

## 九、从源码构建

### 前置要求

- Visual Studio 2026（勾选「.NET 桌面开发」负载），或
- [.NET 10 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)（含桌面运行时）

### 框架依赖版

```bash
git clone https://github.com/lilesswoo-ai/SimpaiViewer.NET.git
cd SimpaiViewer.NET
./publish.cmd        # 输出到 build/ 目录
```

### 自包含版（win-x64）

```bash
dotnet publish Diffusion.Toolkit/Diffusion.Toolkit.csproj ^
  -c Release -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=false ^
  -o out
```

> 自包含构建需要联网下载对应 RID 的 .NET 运行时包，体积较大。

---

## 十、许可证

本项目基于 [DiffusionToolkit](https://github.com/RupertAvery/DiffusionToolkit) 派生，遵循 **MIT 许可证**。

- 上游版权：Copyright (c) 2022 David Khristepher Santos
- 派生与修改同样以 MIT 许可证发布。

详见 [LICENSE](./LICENSE)。
