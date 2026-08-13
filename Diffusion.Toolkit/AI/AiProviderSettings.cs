using System;
using System.Collections.Generic;

namespace Diffusion.Toolkit.AI;

/// <summary>
/// VLM / LLM Provider configuration.
/// Persisted in Settings.AiSettings.Providers.
/// </summary>
public class AiProviderSettings
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; set; } = "New Provider";

    /// <summary>
    /// local  = Sidecar loads model from models/ (JoyCaption/Qwen/etc.)
    /// openai = OpenAI-compatible API (OpenAI, Gemini, Claude, etc.)
    /// ollama = Ollama local server
    /// custom = Any other OpenAI-compatible endpoint
    /// </summary>
    public string Kind { get; set; } = "openai";

    public string? ApiKey { get; set; }
    public string? BaseUrl { get; set; }
    public string? DefaultModel { get; set; }
    public Dictionary<string, string> ExtraHeaders { get; set; } = new();
    public bool Enabled { get; set; } = true;
}
