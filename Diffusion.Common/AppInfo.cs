using System;
using System.IO;

namespace Diffusion.Common;

public static class AppInfo
{
    private const string AppName = "SimpaiViewer";
    // Keep the database at the legacy DiffusionToolkit location so existing
    // image collections/favorites are preserved (no data migration).
    private const string DataFolderName = "DiffusionToolkit";

    public static string AppDir { get; }
    public static SemanticVersion Version => SemanticVersionHelper.GetLocalVersion();
    public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

    /// <summary>
    /// The folder that holds the database when running in non-portable mode.
    /// Kept at the legacy DiffusionToolkit location so existing collections,
    /// favorites and other data are preserved (no data migration).
    /// </summary>
    public static string DatabaseAppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DataFolderName);

    public static string DatabasePath { get; }

    public static string SettingsPath { get; }

    public static bool IsPortable { get; }

    static AppInfo()
    {
        AppDir = AppDomain.CurrentDomain.BaseDirectory;

        if (AppDir.EndsWith("\\"))
        {
            AppDir = AppDir.Substring(0, AppDir.Length - 1);
        }

        DatabasePath = Path.Combine(AppInfo.AppDir, "diffusion-toolkit.db");

        IsPortable = true;

        SettingsPath = Path.Combine(AppInfo.AppDir, "config.json");

        if (!File.Exists(SettingsPath))
        {
            IsPortable = false;
            // Config is now SimpaiViewer-specific so a legacy DiffusionToolkit
            // config.json can never override SimpaiViewer's defaults.
            SettingsPath = Path.Combine(AppInfo.AppDataPath, "config.json");
            // The database stays at the legacy location (no data migration).
            DatabasePath = Path.Combine(AppInfo.DatabaseAppDataPath, "diffusion-toolkit.db");
        }

    }


}