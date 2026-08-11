using Diffusion.Common;
using Diffusion.Database;
using Diffusion.Toolkit.Services;
using System;
using System.IO;
using System.Windows;
using Diffusion.Toolkit.Configuration;

namespace Diffusion.Toolkit
{
    public partial class MainWindow
    {

        private string AppDir
        {
            get
            {
                var appDir = AppDomain.CurrentDomain.BaseDirectory;

                if (appDir.EndsWith("\\"))
                {
                    appDir = appDir.Substring(0, appDir.Length - 1);
                }

                return appDir;
            }
        }

        private void GoPortable()
        {
            SwitchConfig("portable", AppInfo.AppDataPath, AppInfo.DatabaseAppDataPath, AppDir, AppDir);
        }

        private void GoLocal()
        {
            SwitchConfig("application settings", AppDir, AppDir, AppInfo.AppDataPath, AppInfo.DatabaseAppDataPath);
        }

        private void SwitchConfig(string target, string sourceSettingsPathDir, string sourceDbPathDir, string targetSettingsPathDir, string targetDbPathDir)
        {
            string sourceSettingsPath = Path.Combine(sourceSettingsPathDir, "config.json");
            string sourceDbPath = Path.Combine(sourceDbPathDir, "diffusion-toolkit.db");

            string targetSettingsPath = Path.Combine(targetSettingsPathDir, "config.json");
            string targetDbPath = Path.Combine(targetDbPathDir, "diffusion-toolkit.db");


            if (!File.Exists(targetSettingsPath) && !File.Exists(targetDbPath))
            {
                if (File.Exists(sourceSettingsPath))
                {
                    File.Copy(sourceSettingsPath, targetSettingsPath);
                }
                if (File.Exists(sourceDbPath))
                {
                    File.Copy(sourceDbPath, targetDbPath);
                }

                File.Delete(sourceSettingsPath);
                File.Delete(sourceDbPath);
            }
            else
            {
                var existsDialogResult = MessageBox.Show(this, GetLocalizedText("Simpai.Messages.ConfigFound").Replace("{target}", target), "SimpaiViewer", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);

                if (existsDialogResult == MessageBoxResult.No)
                {
                    var confirmResult = MessageBox.Show(this, GetLocalizedText("Simpai.Messages.OverwriteFiles").Replace("{target}", target), "SimpaiViewer", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        if (File.Exists(sourceSettingsPath))
                        {
                            File.Copy(sourceSettingsPath, targetSettingsPath, true);
                        }
                        if (File.Exists(sourceDbPath))
                        {
                            File.Copy(sourceDbPath, targetDbPath, true);
                        }
                    }
                }

                if (existsDialogResult == MessageBoxResult.Cancel)
                {
                    return;
                }



                if (existsDialogResult == MessageBoxResult.Yes)
                {
                    if (target == "application settings")
                    {
                        // rename portable files so that DT doesn't try to load them on startup

                        var bSettingsPath = Path.Combine(sourceSettingsPathDir, "config.backup");
                        var bDbPath = Path.Combine(sourceDbPathDir, "diffusion-toolkit.backup");

                        var moved = false;

                        if (File.Exists(sourceSettingsPath))
                        {
                            File.Move(sourceSettingsPath, bSettingsPath, true);
                            moved = true;
                        }

                        if (File.Exists(sourceDbPath))
                        {
                            File.Move(sourceDbPath, bDbPath, true);
                            moved = true;
                        }

                        if (moved)
                        {
                            MessageBox.Show(this, GetLocalizedText("Simpai.Messages.PortableRenamed"), "SimpaiViewer", MessageBoxButton.OK);
                        }
                    }

                }

            }



            _configuration = new Configuration<Settings>(targetSettingsPath, false);

            if (_configuration.TryLoad(out var settings))
            {
                TypeHelpers.Copy(settings, _settings);
                _settings.PortableMode = _configuration.Portable;
                _settings.SetPristine();
            }

            Logger.Log($"Opening database at {targetDbPath}");

            var dataStore = new DataStore(targetDbPath);

            ServiceLocator.SetDataStore(dataStore);
        }

    }
}
