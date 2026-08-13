using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Diffusion.Database.Models;
using Diffusion.Toolkit.AI;
using Diffusion.Toolkit.Models;
using Diffusion.Toolkit.Services;

namespace Diffusion.Toolkit.Controls;

/// <summary>
/// Displays the SimpaiAI Sidecar reverse-prompt / deconstruct result and lets
/// the user copy or save it into the local asset library.
/// </summary>
public partial class AiReversePromptWindow : Window
{
    private readonly ImageViewModel _image;
    private readonly AiServiceClient _client;
    private readonly string? _providerId;
    private readonly string? _skillId;
    private readonly string? _mode;
    private readonly Dictionary<string, object?>? _metadata;
    private ReversePromptResponse _response;

    private static readonly string[] _dims =
    {
        "subject", "environment", "composition", "lighting", "color",
        "camera", "lens", "material", "mood", "style", "pose",
    };

    private static readonly Dictionary<string, string> _dimLabels = new()
    {
        ["subject"] = "主体", ["environment"] = "环境", ["composition"] = "构图",
        ["lighting"] = "光影", ["color"] = "色彩", ["camera"] = "机位",
        ["lens"] = "镜头", ["material"] = "材质", ["mood"] = "情绪",
        ["style"] = "风格", ["pose"] = "姿态",
    };

    public AiReversePromptWindow(
        ImageViewModel image,
        ReversePromptResponse response,
        AiServiceClient client)
    {
        InitializeComponent();
        _image = image;
        _response = response;
        _client = client;

        var ai = ServiceLocator.Settings?.AiSettings;
        _providerId = ai?.DefaultProviderId;
        _skillId = ai?.DefaultSkillId;
        _mode = response.Status == "metadata_hit" ? "reverse_prompt" : "reverse_prompt";
        _metadata = new Dictionary<string, object?>();
        if (!string.IsNullOrEmpty(image.Prompt)) _metadata["prompt"] = image.Prompt;
        if (!string.IsNullOrEmpty(image.NegativePrompt)) _metadata["negative_prompt"] = image.NegativePrompt;

        Loaded += (_, _) => Populate();
    }

    private void Populate()
    {
        try
        {
            if (File.Exists(_image.Path))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(_image.Path);
                bitmap.EndInit();
                PreviewImage.Source = bitmap;
            }
        }
        catch
        {
            // preview is best-effort
        }

        HeaderText.Text = _response.Status == "metadata_hit"
            ? "AI 分析（命中原始元数据，零成本）"
            : "AI 分析结果";
        var meta = new List<string>();
        if (!string.IsNullOrEmpty(_response.Provider)) meta.Add($"Provider: {_response.Provider}");
        if (!string.IsNullOrEmpty(_response.Model)) meta.Add($"Model: {_response.Model}");
        if (!string.IsNullOrEmpty(_response.SkillId)) meta.Add($"Skill: {_response.SkillId}");
        StatusText.Text = string.Join("   |   ", meta);

        // Deconstruct
        DeconstructItems.Items.Clear();
        var deconstruct = _response.Deconstruct ?? new Dictionary<string, string>();
        foreach (var dim in _dims)
        {
            var value = deconstruct.TryGetValue(dim, out var v) ? v : "";
            if (string.IsNullOrWhiteSpace(value)) continue;
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };
            row.Children.Add(new TextBlock
            {
                Text = _dimLabels.GetValueOrDefault(dim, dim),
                FontWeight = FontWeights.Bold,
                Width = 56,
                VerticalAlignment = VerticalAlignment.Top,
            });
            row.Children.Add(new TextBlock
            {
                Text = value,
                TextWrapping = TextWrapping.Wrap,
                Width = 620,
            });
            DeconstructItems.Items.Add(row);
        }

        ReversePromptBox.Text = _response.ReversePrompt ?? "";
        NegativePromptBox.Text = _response.NegativePrompt ?? "";
        KeywordsBox.Text = _response.Keywords != null ? string.Join(", ", _response.Keywords) : "";

        // Palette
        PaletteItems.Items.Clear();
        foreach (var c in _response.Palette ?? new List<ColorInfo>())
        {
            try
            {
                var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(c.Hex));
                var cell = new StackPanel { Margin = new Thickness(0, 0, 10, 0), Width = 64 };
                cell.Children.Add(new Border
                {
                    Background = brush,
                    Height = 40,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                });
                cell.Children.Add(new TextBlock
                {
                    Text = c.Hex,
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
                PaletteItems.Items.Add(cell);
            }
            catch
            {
                // ignore malformed color
            }
        }
    }

    private void CopyReverseButton_Click(object sender, RoutedEventArgs e) => Copy(ReversePromptBox.Text, "Reverse Prompt");
    private void CopyNegativeButton_Click(object sender, RoutedEventArgs e) => Copy(NegativePromptBox.Text, "Negative Prompt");
    private void CopyKeywordsButton_Click(object sender, RoutedEventArgs e) => Copy(KeywordsBox.Text, "关键词");

    private void Copy(string text, string label)
    {
        if (string.IsNullOrEmpty(text)) return;
        Clipboard.SetText(text);
        StatusText.Text = $"已复制{label}";
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var analysis = new AiImageAnalysis
        {
            ImageId = _image.Id,
            Status = _response.Status,
            Provider = _response.Provider,
            Model = _response.Model,
            SkillId = _response.SkillId,
            ReversePrompt = ReversePromptBox.Text,
            NegativePrompt = NegativePromptBox.Text,
            DeconstructJson = JsonSerializer.Serialize(_response.Deconstruct ?? new Dictionary<string, string>()),
            PaletteJson = JsonSerializer.Serialize(_response.Palette ?? new List<ColorInfo>()),
            KeywordsJson = JsonSerializer.Serialize(ParseKeywords()),
        };
        ServiceLocator.DataStore?.SaveAiAnalysis(analysis);
        StatusText.Text = "已保存到资产库";
        SaveButton.IsEnabled = false;
    }

    private List<string> ParseKeywords()
    {
        if (_response.Keywords != null && _response.Keywords.Count > 0)
            return _response.Keywords;
        return new List<string>(KeywordsBox.Text.Split(new[] { ',', '，', '\n' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private async void RegenerateButton_Click(object sender, RoutedEventArgs e)
    {
        RegenerateButton.IsEnabled = false;
        try
        {
            var result = await _client.ReversePromptAsync(
                _image.Path, _providerId, null, _skillId, mode: _mode, metadata: _metadata);
            if (result != null)
            {
                _response = result;
                Populate();
                StatusText.Text = "已重新生成";
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"重新生成失败：{ex.Message}";
        }
        finally
        {
            RegenerateButton.IsEnabled = true;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
