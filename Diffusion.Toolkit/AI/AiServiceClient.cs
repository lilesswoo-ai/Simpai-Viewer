using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Diffusion.Toolkit.AI;

/// <summary>
/// Thin HTTP client that talks to SimpaiAI Sidecar.
/// All AI logic lives in Sidecar; this class only routes requests.
/// </summary>
public class AiServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AiServiceClient(string baseUrl = "http://127.0.0.1:8765")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };
    }

    public AiServiceClient(HttpClient httpClient, string baseUrl = "http://127.0.0.1:8765")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = httpClient;
    }

    public void SetTimeout(int seconds)
    {
        _httpClient.Timeout = TimeSpan.FromSeconds(seconds);
    }

    public async Task<bool> HealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ProviderListResponse?> GetProvidersAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<ProviderListResponse>($"{_baseUrl}/v1/providers", cancellationToken);
    }

    public async Task<ProviderDto?> CreateProviderAsync(ProviderDto provider, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/v1/providers", provider, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProviderDto>(cancellationToken);
    }

    public async Task<ModelListResponse?> GetModelsAsync(string providerId, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<ModelListResponse>($"{_baseUrl}/v1/providers/{Uri.EscapeDataString(providerId)}/models", cancellationToken);
    }

    public async Task<SkillListResponse?> GetSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<SkillListResponse>($"{_baseUrl}/v1/skills", cancellationToken);
    }

    public async Task<ImportSkillResponse?> ImportSkillAsync(string zipPath, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(zipPath);
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        content.Add(fileContent, "file", Path.GetFileName(zipPath));

        var response = await _httpClient.PostAsync($"{_baseUrl}/v1/skills/import", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ImportSkillResponse>(cancellationToken);
    }

    public async Task<ReversePromptResponse?> ReversePromptAsync(
        string imagePath,
        string? providerId = null,
        string? model = null,
        string? skillId = null,
        string? mode = null,
        Dictionary<string, object?>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ReversePromptRequest
        {
            Image = imagePath,
            Mode = mode ?? "reverse_prompt",
            ProviderId = providerId,
            Model = model,
            SkillId = skillId,
            Metadata = metadata,
            Options = new ReversePromptOptions()
        };

        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/v1/reverse-prompt", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReversePromptResponse>(cancellationToken);
    }
}
