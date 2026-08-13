using System.Collections.Generic;

namespace Diffusion.Toolkit.AI;

/// <summary>
/// AI-related settings persisted inside Settings.
/// </summary>
public class AiSettings
{
    public bool Enabled { get; set; } = true;
    public string SidecarBaseUrl { get; set; } = "http://127.0.0.1:8765";
    public int TimeoutSeconds { get; set; } = 120;
    public bool AutoStartSidecar { get; set; } = true;
    public string SidecarPath { get; set; } = "sidecar";

    public List<AiProviderSettings> Providers { get; set; } = new();
    public string? DefaultProviderId { get; set; }

    public List<PromptSkill> Skills { get; set; } = new();
    public string? DefaultSkillId { get; set; }
}
