namespace IOT_MyHome.Audio
{
    using IOT_MyHome.Settings;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IOT_MyHome.Audio.Model.JsonObjects;
    using Microsoft.Extensions.Logging;

    internal class Manager
    {
        private SettingsManager SettingsManager { get; set; }
        private Settings ManagerSettings { get; set; }
        private ILogger Logger { get; set; }

        public Manager(SettingsManager manager)
        {
            Logger = Logging.Logger.GetLogger<Manager>();
            SettingsManager = manager;
            ManagerSettings = manager.LoadSettings<Settings>();
        }

        public string[] GetFileList()
        {
            try
            {
                return Directory.GetFiles(ManagerSettings.MusicFolder);
            }
            catch (Exception ex)
            {
                Logger.LogInformation("GetFileList() failed with error {0}: {1}", ex.GetType().Name, ex.Message);
                return new string[0];
            }
        }

        internal int GetStartupVolume()
        {
            return ManagerSettings.Volume;
        }

        internal void SaveStartupVolume(int volume)
        {
            ManagerSettings.Volume = volume;
            SettingsManager.SaveSettings(ManagerSettings);
        }

        internal void SaveStartupFile(string fileName)
        {
            ManagerSettings.StartupFilename = fileName;
            SettingsManager.SaveSettings(ManagerSettings);
        }

        internal string GetStartupFile()
        {
            return ManagerSettings.StartupFilename;
        }

        internal void SaveMusicFolder(string folder)
        {
            ManagerSettings.MusicFolder = folder;
            SettingsManager.SaveSettings(ManagerSettings);
        }

        internal string GetMusicFolder()
        {
            return ManagerSettings.MusicFolder;
        }
    }
}
