using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Diffusion.Toolkit.AI;

public class ReversePromptRequest
{
    public string Image { get; set; } = string.Empty; // base64 or absolute path
    public string Mode { get; set; } = "reverse_prompt";
    public string? ProviderId { get; set; }
    public string? Model { get; set; }
    public string? SkillId { get; set; }
    public Dictionary<string, object?>? Metadata { get; set; }
    public ReversePromptOptions? Options { get; set; }
}

public class ReversePromptOptions
{
    public bool Deconstruct { get; set; } = true;
    public bool Palette { get; set; } = true;
    public bool Pose { get; set; } = false;
}

public class ReversePromptResponse
{
    public string ReversePrompt { get; set; } = string.Empty;
    public string NegativePrompt { get; set; } = string.Empty;
    public Dictionary<string, string>? Deconstruct { get; set; }
    public List<ColorInfo> Palette { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string? Provider { get; set; }
    public string? Model { get; set; }
    public string? SkillId { get; set; }
    public string Status { get; set; } = "ok";
}

public class ColorInfo
{
    public string Hex { get; set; } = string.Empty;
    public List<int> Rgb { get; set; } = new();
    public double Ratio { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class ProviderDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; }
    public string? DefaultModel { get; set; }
    public Dictionary<string, string> ExtraHeaders { get; set; } = new();
    public bool Enabled { get; set; } = true;
}

public class ProviderListResponse
{
    public List<ProviderDto> Providers { get; set; } = new();
}

public class ModelListResponse
{
    public List<ModelInfo> Models { get; set; } = new();
}

public class ModelInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class SkillListResponse
{
    public List<PromptSkill> Skills { get; set; } = new();
}

public class ImportSkillResponse
{
    public string SkillId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Name { get; set; }
}
