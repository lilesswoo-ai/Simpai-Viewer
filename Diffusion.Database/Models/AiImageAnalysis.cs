using System;
using SQLite;

namespace Diffusion.Database.Models;

/// <summary>
/// Stores the AI reverse-prompt / deconstruct result for a single image.
/// Produced by SimpaiAI Sidecar and surfaced in the viewer's AI panel.
/// </summary>
public class AiImageAnalysis
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>FK to Image.Id (1:1).</summary>
    public int ImageId { get; set; }

    /// <summary>Pipeline status: ok | metadata_hit | error.</summary>
    public string? Status { get; set; }

    /// <summary>Provider id that produced the result (e.g. local-joycaption, mock, openai-...).</summary>
    public string? Provider { get; set; }

    /// <summary>Model id used (e.g. joycaption-alpha-two, gpt-4o-mini).</summary>
    public string? Model { get; set; }

    /// <summary>Prompt Skill id used for the reverse-prompt style.</summary>
    public string? SkillId { get; set; }

    /// <summary>Final creative reverse prompt (natural language).</summary>
    public string? ReversePrompt { get; set; }

    /// <summary>Negative prompt suggestions.</summary>
    public string? NegativePrompt { get; set; }

    /// <summary>9-dimension deconstruct as JSON (subject/environment/.../pose).</summary>
    public string? DeconstructJson { get; set; }

    /// <summary>Dominant color palette as JSON ([{hex,rgb,ratio,role}...]).</summary>
    public string? PaletteJson { get; set; }

    /// <summary>Keyword list as JSON.</summary>
    public string? KeywordsJson { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
