using System;
using System.Collections.Generic;

namespace Diffusion.Toolkit.AI;

/// <summary>
/// A Prompt Skill package defines how reverse-prompt / deconstruct is performed.
/// Imported from a zip containing manifest.json + system_prompt.txt + user_template.txt + deconstruct_schema.json.
/// </summary>
public class PromptSkill
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string SchemaVersion { get; set; } = "2.0";
    public string Language { get; set; } = "zh";
    public List<string> TargetKind { get; set; } = new() { "reverse_prompt", "deconstruct" };
    public string? DefaultModel { get; set; }
    public List<string> Tags { get; set; } = new();
    public string Description { get; set; } = string.Empty;

    // Populated after import from disk
    public string? FolderPath { get; set; }
    public string? SystemPrompt { get; set; }
    public string? UserTemplate { get; set; }
    public string? DeconstructSchemaJson { get; set; }
}
