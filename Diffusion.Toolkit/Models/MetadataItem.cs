namespace Diffusion.Toolkit.Models;

/// <summary>
/// A single key/value pair displayed in the metadata parameters panel.
/// <c>Key</c> is either a localized label (for well-known parameters) or the
/// raw key parsed from the image metadata (for "other" parameters).
/// <c>Value</c> is the display value; it is never null for items shown.
/// </summary>
public class MetadataItem
{
    public string Key { get; }

    public string Value { get; }

    public MetadataItem(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
