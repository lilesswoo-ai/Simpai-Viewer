using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Diffusion.Toolkit.AI;

/// <summary>
/// 启动并跟踪随 SimpaiViewer 捆绑部署的 SimpaiAI Sidecar（Python FastAPI）进程。
/// Sidecar 目录位于 &lt;exeDir&gt;/&lt;SidecarPath&gt;（默认 sidecar）。
/// 通过 venv 启动：&lt;sidecar&gt;/.venv/Scripts/python.exe -m uvicorn ...；若无 venv 则回退系统 python。
/// </summary>
public static class SidecarLauncher
{
    private static Process? _process;
    private static readonly object _lock = new();

    public static string ResolveSidecarDir(AiSettings ai)
    {
        var baseDir = AppContext.BaseDirectory.TrimEnd('\\', '/');
        var rel = (ai?.SidecarPath ?? "sidecar").Trim('\\', '/');
        return Path.Combine(baseDir, rel);
    }

    public static string ResolvePythonExe(AiSettings ai)
    {
        var dir = ResolveSidecarDir(ai);
        var venvPy = Path.Combine(dir, ".venv", "Scripts", "python.exe");
        if (File.Exists(venvPy)) return venvPy;
        return "python";
    }

    public static bool IsRunning
    {
        get { lock (_lock) return _process != null && !_process.HasExited; }
    }

    public static Process? Start(AiSettings ai)
    {
        lock (_lock)
        {
            if (_process != null && !_process.HasExited) return _process;

            var dir = ResolveSidecarDir(ai);
            if (!Directory.Exists(dir))
                throw new DirectoryNotFoundException($"未找到 Sidecar 目录：{dir}");

            var python = ResolvePythonExe(ai);
            var url = ai?.SidecarBaseUrl ?? "http://127.0.0.1:8765";
            var uri = new Uri(url);
            var host = uri.Host;
            var port = uri.Port;

            var psi = new ProcessStartInfo
            {
                FileName = python,
                Arguments = $"-m uvicorn app.main:app --host {host} --port {port}",
                WorkingDirectory = dir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            var p = Process.Start(psi)!;
            p.EnableRaisingEvents = true;
            p.Exited += OnExited;
            _process = p;
            return _process;
        }
    }

    public static void Stop()
    {
        Process? proc;
        lock (_lock)
        {
            proc = _process;
            _process = null;
        }
        if (proc == null) return;
        try
        {
            if (!proc.HasExited) { proc.Kill(); proc.WaitForExit(5000); }
        }
        catch { }
        finally { try { proc.Dispose(); } catch { } }
    }

    private static void OnExited(object? sender, EventArgs e)
    {
        if (sender is not Process p) return;
        bool shouldDispose;
        lock (_lock)
        {
            shouldDispose = (_process == p);
            if (shouldDispose) _process = null;
        }
        if (shouldDispose) { try { p.Dispose(); } catch { } }
    }
}
