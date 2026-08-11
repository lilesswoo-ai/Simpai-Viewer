# 搜索帮助

您可以使用查询或过滤来搜索图片。

[查询](#查询) 让您直接输入查询词，适合快速搜索。您不仅可以按提示词搜索，还可以按参数搜索，详见[参数搜索](#参数搜索)。

[过滤](#过滤) 提供更多控制，每个参数单独分开，对 ComfyUI 属性搜索控制更细，适合不想学习查询语法（其实很简单）或更喜欢界面化操作的用户。

无论哪种方式，您都可以使用 **搜索 > 保存查询/过滤器** 保存查询。

* [查询](#查询)
    * [查询语法](#查询语法)
    * [简单提示词搜索](#简单提示词搜索)
    * [参数搜索](#参数搜索)
    * [支持的参数](#支持的参数)
        * [负面提示词](#负面提示词)
        * [步骤](#步骤)
        * [采样器](#采样器)
        * [无分类器指导缩放（CFG/Scale）](#无分类器指导缩放cfgscale)
        * [种子](#种子)
        * [大小](#大小)
        * [模型哈希](#模型哈希)
        * [模型名称](#模型名称)
        * [美学评分](#美学评分)
        * [Hyper Networks](#hyper-networks)
        * [收藏](#收藏)
        * [评分](#评分)
        * [NSFW](#nsfw)
        * [无元数据](#无元数据)
        * [待删除](#待删除)
        * [创建日期](#创建日期)
        * [路径](#路径)
        * [文件夹](#文件夹)
    * [关于搜索的说明](#关于搜索的说明)
    * [多值搜索](#多值搜索)
    * [查询工作流属性和原始元数据](#查询工作流属性和原始元数据)
* [过滤](#过滤)
    * [元数据选项卡](#元数据选项卡)
    * [工作流选项卡](#工作流选项卡)

# 查询

缩略图区域上方的文本框是**查询**输入框。您可以输入想要查找的内容。使用**查询语法**，您不仅可以搜索提示词，还可以按文件的[路径](#路径)或[日期](#创建日期)范围等搜索，也可以组合多个条件进行更精细的搜索。

如果查询语法难以理解或过于冗长，也可以改用[过滤](#过滤)。

## 简单提示词搜索

最基本的搜索方式是输入提示词中出现的文字。大多数情况下它会按您预期的方式工作。不过，逗号的处理方式与提示词中不同。在查询语法中，逗号用于分隔**搜索条件**。

例如这个查询：

```
A man staring into a starry night sky, by Van Gogh
```

它包含两个搜索条件：

* `A man staring into a starry night sky`
* `by Van Gogh`

上述查询会匹配提示词中同时包含 `A man staring into a starry night sky` 和 `by Van Gogh` 的图片，顺序和位置不限。

以下提示词都会被匹配：

```
A man staring into a starry night sky, by Van Gogh
A man staring into a starry night sky, pencil sketch, by Van Gogh
```

如果希望匹配包含逗号的完整词条，请把词条放在双引号中：

```
"A man staring into a starry night sky, by Van Gogh"
```

上述查询只匹配提示词中恰好包含 `A man staring into a starry night sky, by Van Gogh` 的图片。

注意空格很重要：

```
"A man staring into a starry night sky , by Van Gogh"
```

与上一个词条并不相同。

## 参数搜索

要在搜索框中按其他参数搜索，需要使用特殊标记。这些是参数对应的词加上冒号，例如 `seed: 12345` 会添加一个按种子值 `12345` 过滤图片的搜索条件。

提示词查询（如果有）应始终放在最前面，以免被解析为参数标记的参数。

```
A man staring into a starry night sky, by Van Gogh steps: 20 cfg:12
```

参数之间是 AND 关系，即添加更多参数会过滤掉更多结果。上述查询会显示满足以下条件的图片：

* 提示词包含 `A man staring into a starry night sky`
* 且 `by Van Gogh`
* 且 `steps` 为 `20`
* 且 `cfg` 为 `12`

解析查询时，会依次匹配并移除每个可能的参数。剩余未匹配的文本会视为提示词的一部分。

# 支持的参数

## 负面提示词

* `negative prompt: <term> [,<term>]`
* `negative_prompt: <term> [,<term>]`
* `negative: <term> [,<term>]`

## 步骤

* `steps: <number>`
* `steps: <start>-<end>`

## 采样器

* `sampler: <name>`

采样器名称因 AI 生成器而异。有些使用带空格的名称，有些使用小写加下划线。请查看生成器在图片中存储的内容。对于使用空格的采样器名称，请把名称放在引号中。

以下是 A1111 及其他工具中常见采样器列表：

* "Euler a" 或 `euler_a`
* Euler 或 `euler`
* LMS 或 `lms`
* Heun 或 `heun`
* DPM2 或 `dpm2`
* "DPM2 a" 或 `dpm2_a`
* "DPM++ 2S a" 或 `dpm++_2s_a`
* "DPM++ 2M" 或 `dpm++_2m`
* "DPM++ SDE" 或 `dpm++_sde`
* "DPM fast" 或 `dpm_fast`
* "DPM adaptive" 或 `dpm_adaptive`
* "LMS Karras" 或 `lms_karras`
* "DPM2 Karras" 或 `dpm2_karras`
* "DPM2 a Karras" 或 `dpm2_a_karras`
* "DPM++ 2S a Karras" 或 `dpm++_2s_a_karras`
* "DPM++ 2M Karras" 或 `dpm++_2s_karras`
* "DPM++ SDE Karras" 或 `dpm++_sde_karras`
* DDIM 或 `ddim`
* PLMS 或 `plms`

## 无分类器指导缩放（CFG/Scale）

* `cfg: <number>`
* `cfg_scale: <number>`
* `cfg scale: <number>`

## 种子

您可以用数字、范围或通配符查询 `seed`。

* `seed: <number>`
* `seed: <start>-<end>`
* `seed: 123*`
   * 显示种子以 `123` 开头的所有图片
* `seed: 123456???000`
   * 显示种子以 `123456` 开头、中间任意 3 位数字、以 `000` 结尾的所有图片

## 大小

* `size: <width>x<height>`
* `size: <width>:<height>`

`width` 和 `height` 可以是数字或问号（`?`）以匹配任意值。例如 `size:512x?` 匹配宽度为 `512`、任意高度的图片。

## 模型哈希

* `model_hash: <hash>`

## 模型名称

支持通配符（`?`、`*`）

* `model: <term>`

有些工具默认不在元数据中存储模型名称。

如果存在 AUTOMATIC1111 的 `cache.json` 文件，SimpaiViewer 会尝试利用其中存储的信息进行哈希查找。它会尝试查找名称（允许部分匹配），并对匹配模型取哈希，再用哈希查询搜索图片。旧哈希算法和新的 SHA256 哈希都支持。

注意 `cache.json` 文件由 AUTOMATIC1111 实时更新。它会为新模型计算模型哈希。在 UI 中切换到模型时会计算哈希。

您应点击**编辑 > 重新加载模型**，确保程序持有 json 文件的最新副本。

## 美学评分

美学评分是 Aesthetic Image Scorer 扩展为 AUTOMATIC1111 Web UI 添加的标签。

它基于 Chad Scorer 使用 CLIP+MLP 美学评分预测器为生成图片计算美学评分，并存储到元数据中。

* `aesthetic_score: [<|>|<=|>=|<>] <number>`

可以搜索精确数值，例如 `aesthetic_score: 0.6`，但更常见的是比较搜索，例如小于 `aesthetic_score: < 0.6`。

## Hyper Networks

您可以搜索使用了 hypernetwork 的图片，并指定使用的强度（AUTOMATIC1111）。

* `hypernet: <name>`

* `hypernet strength: [<|>|<=|>=|<>] <number>`

## 收藏

收藏是 SimpaiViewer 的元数据，值为 true 或 false，由用户设置。

* `favorite: [true|false]`

## 评分

评分是 SimpaiViewer 的元数据，值为 1-10，由用户设置。

* `rating: [<|>|<=|>=|<>] <number>`

## NSFW

NSFW 是 SimpaiViewer 的元数据，值为 true 或 false，由用户设置。

如果显式指定此条件，会覆盖**从结果中隐藏 NSFW** 选项。

* `nsfw: [true|false]`

## 无元数据

此过滤器显示没有元数据的图片。

* `nometa: [true|false]`
* `nometadata: [true|false]`

## 待删除

**待删除**是 SimpaiViewer 的元数据，值为 true 或 false，由用户设置。

* `delete: [true|false]` - 按标记为删除的文件过滤

## 创建日期

**创建日期**是扫描时从图片文件属性中取得的 SimpaiViewer 元数据。

允许按文件的创建日期搜索。

* `date: <criteria>`

   * `date: today` - 包含当前日期的文件
   * `date: yesterday` - 包含前一天的日期
   * `date: between 11-11-2022 and yesterday` - 包含从 2022 年 11 月 11 日到前一天的日期
   * `date: from 10-10-2022 to 11-11-2022` - 另一种写法
   * `date: before 11-11-2022` - 包含从开始到 2022 年 11 月 11 日的日期
   * `date: since 01-01-2022` - 包含从 2022 年 1 月 1 日到今天的日期

  注意事项：

   * 支持 `YYYY-MM-DD` 格式
   * `XX-XX-XXXX` 日期将按您电脑的日期格式解析，即美国和类似地区为 `MM-DD-YYYY`，欧洲地区为 `DD-MM-YYYY`。

## 路径

**路径**是扫描时从图片文件属性中取得的 SimpaiViewer 元数据。

可以使用通配符（`?`、`*`），或 `starts with`、`contains`、`ends with` 条件。

路径匹配完整路径（包含文件名），因此通常需要配合通配符使用。

带通配符的路径会返回子文件夹中的匹配项。如果只想搜索特定文件夹，请使用[文件夹](#文件夹)。

* `path: [criteria] <search-term>`

   * 使用通配符：
      * `path: D:\diffusion\images*`
      * `path: *img2img*`
      * `path: *.jpg`

   * 使用条件：
      * `path: starts with D:\diffusion\images`
      * `path: contains img2img`
      * `path: ends with .jpg`

   如果路径包含空格，请用双引号包裹路径。

   * 使用 glob：
      * `path: "D:\My pics\images**"`
      * `path: "**funny cats**"`
   * 使用条件：
      * `path: starts with "D:\My pics\images"`
      * `path: contains "funny cats"`

## 文件夹

**文件夹**是扫描时从图片文件属性中取得的 SimpaiViewer 元数据。按文件夹搜索会把结果限制在特定文件夹中，与路径不同——路径会包含子文件夹中的图片。

* `folder: <folder>`

## 关于搜索的说明

* 参数（如 `steps:`、`sampler:`）不区分大小写。可以使用 `Steps:`、`Sampler:`，因此可以从提示词中复制。
* 冒号（`:`）之后、参数值之前可以有 0 个或多个空格。
    * 例如 `steps:20`、`steps: 20`、`steps:   20` 都可以
    * 但 `steps  :20`、`steps :20` 不可以

# 多值搜索

大多数参数都可以搜索多个值。结果之间是 OR 关系，即添加更多值会带来更多结果。

* 可以为 seed 指定范围，如 `seed: <start>-<end>`
  * 例如 `seed: 10000-20000`
* 可以用竖线（`|`）为其他参数指定多个值
  * 例如 `sampler: euler a | ddim | plms`
  * 例如 `cfg: 4.5|7|9|12`
  * 例如 `model_hash: aabbccdd | deadbeef | 12345678`

## 查询工作流属性和原始元数据

您可以通过查询输入框搜索 ComfyUI 工作流或原始元数据。

首先必须在设置中启用工作流和原始元数据扫描，然后重新扫描图片。

然后点击查询栏中的设置图标，配置要搜索哪些属性。

要查找属性名称，请查看元数据面板的"工作流"选项卡，点击每个属性右侧的 `...` 按钮，选择"复制属性名称"。

# 过滤

点击过滤器按钮会打开过滤器对话框，包含**元数据**选项卡和**工作流**选项卡。

## 元数据选项卡

在这里可以选择要过滤的参数。提示词会按与查询相同的方式[解析](#简单提示词搜索)。

请确保要搜索的参数旁边的复选框已勾选，否则会被忽略。

选项卡底部附近会看到一组带 **True** 和 **False** 选项的参数。它们用于搜索图片是否被*标记*（True）或*未标记*（False）。

## 工作流选项卡

工作流选项卡用于过滤带 ComfyUI 元数据的图片。在这里可以选择要搜索的属性以及值的搜索方式。对于文本属性，通常使用 *contains*，其他方法如 *starts with* 也可能有用。

您可以用 *and*、*or*、*not* 运算符组合过滤器。运算符的顺序很重要，因为过滤器的结果会与下一个过滤器叠加，所以请合理规划过滤器顺序。
