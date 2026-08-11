using Diffusion.Database.Models;
using Diffusion.Toolkit.Classes;
using Diffusion.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Diffusion.Common;
using Diffusion.Toolkit.Services;
using Node = Diffusion.ComfyUI.Node;

namespace Diffusion.Toolkit.Models;

public class ImageViewModel : BaseNotify
{
    private string _prompt;
    private string _negativePrompt;
    private string _otherParameters;

    private string _modelHash;

    public ImageViewModel()
    {
        CopyPathCommand = new RelayCommand<object>(ServiceLocator.ContextMenuService.CopyPath);
        CopyPromptCommand = new RelayCommand<object>(ServiceLocator.ContextMenuService.CopyPrompt);
        CopyNegativePromptCommand = new RelayCommand<object>(ServiceLocator.ContextMenuService.CopyNegative);
        //_model.CurrentImage.CopySeed = new RelayCommand<object>(CopySeed);
        //_model.CurrentImage.CopyHash = new RelayCommand<object>(CopyHash);
        CopyOthersCommand = new RelayCommand<object>(ServiceLocator.ContextMenuService.CopyOthers);
        CopyParametersCommand = new RelayCommand<object>(ServiceLocator.ContextMenuService.CopyParameters);
        //ShowInExplorerCommand = new RelayCommand<object>(ServiceLocator.ContextMenuService.ShowInExplorer);
    }

    public MainModel MainModel => ServiceLocator.MainModel;

    public int Id { get; set; }

    public bool IsMessageVisible
    {
        get;
        set => SetField(ref field, value);
    }

    public BitmapSource? Image
    {
        get;
        set => SetField(ref field, value);
    }

    public string Path
    {
        get;
        set => SetField(ref field, value);
    }

    public string? Prompt
    {
        get => _prompt;
        set => SetField(ref _prompt, value);
    }

    public string? NegativePrompt
    {
        get => _negativePrompt;
        set => SetField(ref _negativePrompt, value);
    }

    public string? OtherParameters
    {
        get => _otherParameters;
        set
        {
            SetField(ref _otherParameters, value);
            OnPropertyChanged(nameof(ParameterItems));
            OnPropertyChanged(nameof(OtherParameterItems));
        }
    }

    /// <summary>
    /// Ordered list of well-known parameters for the tidy "Parameters" section.
    /// Empty/zero values are skipped so no placeholder rows are shown.
    /// </summary>
    public IReadOnlyList<MetadataItem> ParameterItems
    {
        get
        {
            var items = new List<MetadataItem>(16);

            AddParameter(items, "Metadata.Steps", Steps > 0 ? Steps.ToString(CultureInfo.CurrentCulture) : null);
            AddParameter(items, "Metadata.Sampler", Sampler);
            AddParameter(items, "Metadata.CFGScale", CFGScale > 0 ? CFGScale.ToString(CultureInfo.CurrentCulture) : null);
            AddParameter(items, "Metadata.Seed", Seed > 0 ? Seed.ToString(CultureInfo.CurrentCulture) : null);
            AddParameter(items, "Metadata.Size", Width > 0 && Height > 0 ? $"{Width}\u00d7{Height}" : null);
            AddParameter(items, "Metadata.ModelName", ModelName);
            AddParameter(items, "Metadata.ModelHash", ModelHash);
            AddParameter(items, "Simpai.Metadata.Styles", GetOtherValue("Styles"));
            AddParameter(items, "Simpai.Metadata.BackendEngine", GetOtherValue("Backend Engine"));
            AddParameter(items, "Simpai.Metadata.CLIP", GetOtherValue("CLIP / Text Encoder"));
            AddParameter(items, "Simpai.Metadata.VAE", GetOtherValue("VAE"));
            AddParameter(items, "Simpai.Metadata.UpscaleModel", GetOtherValue("Upscale Model"));
            AddParameter(items, "Simpai.Metadata.Sharpness", GetOtherValue("Sharpness"));
            AddParameter(items, "Simpai.Metadata.Performance", GetOtherValue("Performance"));
            AddParameter(items, "Simpai.Metadata.ADMGuidance", GetOtherValue("ADM Guidance"));
            AddParameter(items, "Simpai.Metadata.Scheduler", GetOtherValue("Scheduler"));
            AddParameter(items, "Simpai.Metadata.Version", GetOtherValue("Version"));
            AddParameter(items, "Simpai.Metadata.AestheticScore", AestheticScore is { Length: > 0 } aesthetic && aesthetic != "0" ? aesthetic : null);
            AddParameter(items, "Simpai.Metadata.HyperNetwork", HyperNetwork);
            AddParameter(items, "Simpai.Metadata.ClipSkip", ClipSkip is > 0 ? ClipSkip.Value.ToString(CultureInfo.CurrentCulture) : null);
            AddParameter(items, "Simpai.Metadata.ENSD", ENSD is > 0 ? ENSD.Value.ToString(CultureInfo.CurrentCulture) : null);
            AddParameter(items, "Simpai.Metadata.FileSize", FileSize > 0 ? FormatFileSize(FileSize) : null);
            AddParameter(items, "Simpai.Metadata.Created", Date);
            AddParameter(items, "Simpai.Metadata.Modified", ModifiedDate);

            return items;
        }
    }

    /// <summary>
    /// Retrieves the value for a named key from the parsed "Other Parameters"
    /// text (e.g. "Styles" or "Backend Engine"). Returns null when absent.
    /// </summary>
    private string? GetOtherValue(string key)
    {
        foreach (var item in ParseOtherParameters(OtherParameters))
        {
            if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
            {
                return item.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// "Other" parameters parsed into one row per key/value pair instead of a
    /// single wall-of-text. Rows whose key duplicates a well-known parameter
    /// (already shown in <see cref="ParameterItems"/>) are skipped.
    /// </summary>
    public IReadOnlyList<MetadataItem> OtherParameterItems
    {
        get
        {
            if (string.IsNullOrWhiteSpace(OtherParameters))
            {
                return System.Array.Empty<MetadataItem>();
            }

            var items = new List<MetadataItem>();
            foreach (var item in ParseOtherParameters(OtherParameters))
            {
                if (CuratedParameterKeys.Contains(item.Key))
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }
    }

    private static readonly HashSet<string> CuratedParameterKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "steps", "sampler", "cfg", "cfg scale", "seed", "size",
        "model", "model name", "model hash", "aesthetic", "aesthetic score", "aesthetic_score",
        "hypernetwork", "clip skip", "ensd", "file size", "created", "modified", "date",
        "styles", "backend engine", "clip / text encoder", "vae", "upscale model",
        "sharpness", "performance", "adm guidance", "scheduler", "version"
    };

    private const string KnownKeyDelimiters =
        "Steps|Sampler|CFG Scale|CFG|Seed|Size|Model Hash|Model Name|Model|Batch Size|Batch Pos|" +
        "Aesthetic Score|aesthetic_score|Clip skip|ClipSkip|ENSD|Hires upscale|Hires steps|" +
        "Denoising strength|Hypernetwork|Hypernetwork Strength|Prompt Strength|Upscaler|Scale Factor|" +
        "ControlNet|Lora|LoRA|Schedule type|ETA|Scheduler";

    private static readonly Regex OtherParameterRegex = new(
        $@"(?<key>[A-Za-z_][A-Za-z0-9_ .()/+\-]*?):\s*(?<value>.*?)(?=\s+(?:{KnownKeyDelimiters}):|$)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    /// Splits a metadata line on commas, but only when the text following a
    /// comma begins a new "Key:" pair. This keeps comma-separated A1111-style
    /// parameters intact while preserving multi-value fields such as
    /// "Styles: Artstyle Abstract, Watercolor 2" or "ADM Guidance: (1.5, 0.8, 0.3)".
    /// </summary>
    private static readonly Regex OtherParameterSplitRegex = new(
        @",\s*(?=[A-Za-z_][A-Za-z0-9_ .()/+\-]*:)",
        RegexOptions.Compiled);

    /// <summary>
    /// Parses a metadata "other parameters" string (which can be newline-,
    /// comma- or space-separated "Key: Value" pairs) into a sequence of
    /// <see cref="MetadataItem"/>, deduplicating repeated keys.
    /// </summary>
    public static IEnumerable<MetadataItem> ParseOtherParameters(string? other)
    {
        if (string.IsNullOrWhiteSpace(other))
        {
            yield break;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawLine in other.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0)
            {
                continue;
            }

            foreach (var rawChunk in OtherParameterSplitRegex.Split(line))
            {
                var chunk = rawChunk.Trim();
                if (chunk.Length == 0)
                {
                    continue;
                }

                var matches = OtherParameterRegex.Matches(chunk);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var key = match.Groups["key"].Value.Trim();
                        var value = match.Groups["value"].Value.Trim().TrimEnd(',', ';');
                        if (key.Length == 0 || value.Length == 0 || !seen.Add(key))
                        {
                            continue;
                        }

                        yield return new MetadataItem(key, value);
                    }
                }
                else
                {
                    var colonIndex = chunk.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        var key = chunk.Substring(0, colonIndex).Trim();
                        var value = chunk.Substring(colonIndex + 1).Trim();
                        if (key.Length == 0 || value.Length == 0 || !seen.Add(key))
                        {
                            continue;
                        }

                        yield return new MetadataItem(key, value);
                    }
                }
            }
        }
    }

    private static void AddParameter(List<MetadataItem> items, string labelKey, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        items.Add(new MetadataItem(GetLocalizedLabel(labelKey), value));
    }

    private static string GetLocalizedLabel(string key)
    {
        try
        {
            var localized = Localization.JsonLocalizationProvider.Instance.GetLocalizedObject(key, null, CultureInfo.InvariantCulture);
            return localized as string ?? key;
        }
        catch
        {
            return key;
        }
    }

    private static string FormatFileSize(long bytes)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        var unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.##} {units[unit]}";
    }

    public decimal CFGScale
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public int Height
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public int Width
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public string ModelName
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    /// <summary>
    /// Looks up the "LoRAs" / "LoRA" / "Lora" entry in <see cref="OtherParameters"/>
    /// and returns the raw value string (e.g. "MyLoRA: 0.8, StyleLoRA: 0.5") or
    /// <c>null</c> if not present. Used by the preview pane to highlight LoRA usage.
    /// </summary>
    public string? LoRAsSummary
    {
        get
        {
            if (string.IsNullOrWhiteSpace(OtherParameters))
            {
                return null;
            }
            foreach (var item in OtherParameterItems)
            {
                if (string.Equals(item.Key, "LoRAs", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(item.Key, "LoRA", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(item.Key, "Lora", StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }
            return null;
        }
    }

    public string Date
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public ICommand CopyPromptCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand SearchModelCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand CopyPathCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand ShowInExplorerCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand DeleteCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand FavoriteCommand
    {
        get;
        set => SetField(ref field, value);
    }


    public ICommand CopyNegativePromptCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand CopyOthersCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand CopyParametersCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public bool Favorite
    {
        get;
        set => SetField(ref field, value);
    }

    public int? Rating
    {
        get;
        set => SetField(ref field, value);
    }

    public bool ForDeletion
    {
        get;
        set => SetField(ref field, value);
    }

    public bool NSFW
    {
        get;
        set => SetField(ref field, value);
    }

    public bool HasError
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand ShowInThumbnails
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsParametersVisible
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand ToggleParameters
    {
        get;
        set => SetField(ref field, value);
    }

    public long Seed
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public string? ModelHash
    {
        get => _modelHash;
        set
        {
            SetField(ref _modelHash, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public string? AestheticScore
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
            OnPropertyChanged(nameof(OtherParameterItems));
        }
    }

    public string? HyperNetwork
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public int? ClipSkip
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public int? ENSD
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public long FileSize
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public string? ModifiedDate
    {
        get;
        set;
    }

    public IEnumerable<Album> Albums
    {
        get;
        set => SetField(ref field, value);
    }

    public string? Sampler
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public int Steps
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(ParameterItems));
        }
    }

    public bool IsLoading
    {
        get;
        set => SetField(ref field, value);
    }

    public string Message
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand OpenAlbumCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public ICommand RemoveFromAlbumCommand
    {
        get;
        set => SetField(ref field, value);
    }

    public string? Workflow
    {
        get;
        set
        {
            SetField(ref field, value);
            OnPropertyChanged(nameof(FormattedWorkflow));
        }
    }

    /// <summary>
    /// Returns the raw workflow/metadata JSON pretty-printed so the "Raw
    /// Metadata" panel is readable instead of a single wall of text. Falls
    /// back to the raw string when it is not valid JSON.
    /// </summary>
    public string? FormattedWorkflow
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Workflow))
            {
                return Workflow;
            }

            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(Workflow);
                return System.Text.Json.JsonSerializer.Serialize(doc.RootElement, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return Workflow;
            }
        }
    }

    public ImageType Type
    {
        get;
        set => SetField(ref field, value);
    }

    public IReadOnlyCollection<Node> Nodes
    {
        get;
        set => SetField(ref field, value);
    }

    public IReadOnlyCollection<ImageTagView> ImageTags
    {
        get;
        set => SetField(ref field, value);
    }

    public IReadOnlyCollection<ImageTagView> FilteredTags
    {
        get;
        set => SetField(ref field, value);
    }


    public string ErrorMessage
    {
        get;
        set => SetField(ref field, value);
    }
}


public class ImageTagView : BaseNotify
{
    public int Id { get; set; }
    public string Name { get; set; }

    public bool IsTicked
    {
        get; 
        set => SetField(ref field, value); 
    }
}

public class TagFilterView : BaseNotify
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TagCount { get; set; }

    public bool IsTicked
    {
        get;
        set => SetField(ref field, value);
    }
}