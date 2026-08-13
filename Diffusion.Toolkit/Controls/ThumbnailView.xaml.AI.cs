using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Diffusion.Toolkit.AI;
using Diffusion.Toolkit.Classes;
using Diffusion.Toolkit.Configuration;
using Diffusion.Toolkit.Models;
using Diffusion.Toolkit.Services;

namespace Diffusion.Toolkit.Controls;

public partial class ThumbnailView
{
    private AiSettings? AiSettings => ServiceLocator.Settings?.AiSettings;

    private async Task AiReversePrompt()
    {
        await RunAiAnalysis("reverse_prompt");
    }

    private async Task AiDeconstruct()
    {
        await RunAiAnalysis("deconstruct");
    }

    private async Task RunAiAnalysis(string mode)
    {
        var image = Model.CurrentImage;
        if (image == null)
        {
            await ServiceLocator.MessageService.Show("请先选择一张图片", "AI 反推", PopupButtons.OK);
            return;
        }

        var ai = AiSettings;
        if (ai == null || !ai.Enabled)
        {
            await ServiceLocator.MessageService.Show(
                "AI 功能未启用，请先在「设置 → AI」中开启并配置 SimpaiAI Sidecar。",
                "AI 反推", PopupButtons.OK);
            return;
        }

        var client = new AiServiceClient(ai.SidecarBaseUrl ?? "http://127.0.0.1:8765");
        client.SetTimeout(ai.TimeoutSeconds);

        bool healthy;
        try
        {
            healthy = await client.HealthAsync();
        }
        catch
        {
            healthy = false;
        }

        if (!healthy)
        {
            await ServiceLocator.MessageService.Show(
                "无法连接 SimpaiAI Sidecar。\n请确认 Sidecar 已启动（默认 http://127.0.0.1:8765）。",
                "AI 反推", PopupButtons.OK);
            return;
        }

        var metadata = new Dictionary<string, object?>();
        if (!string.IsNullOrEmpty(image.Prompt))
            metadata["prompt"] = image.Prompt;
        if (!string.IsNullOrEmpty(image.NegativePrompt))
            metadata["negative_prompt"] = image.NegativePrompt;

        try
        {
            var response = await client.ReversePromptAsync(
                image.Path,
                ai.DefaultProviderId,
                null,
                ai.DefaultSkillId,
                mode: mode,
                metadata: metadata);

            if (response == null)
            {
                await ServiceLocator.MessageService.Show("Sidecar 未返回结果", "AI 反推", PopupButtons.OK);
                return;
            }

            var window = new AiReversePromptWindow(image, response, client);
            if (Application.Current?.MainWindow != null)
                window.Owner = Application.Current.MainWindow;
            window.Show();
        }
        catch (Exception ex)
        {
            await ServiceLocator.MessageService.Show(
                $"AI 反推失败：{ex.Message}", "AI 反推", PopupButtons.OK);
        }
    }
}
