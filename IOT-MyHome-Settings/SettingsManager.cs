namespace IOT_MyHome.Settings
{
    using Newtonsoft.Json;
    using System;
    using System.IO;

    /// <summary>
    /// Class to help with saving/loading settings to JSON.
    /// </summary>
    public class SettingsManager
    {
        private string SavePath;

        /// <summary>
        /// Constructor specify path to save to.
        /// </summary>
        /// <param name="path"></param>
        public SettingsManager(string path)
        {
            SavePath = path + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Gets the save path for the specified object using the module and name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private string GetPath<T>()
        {
            return SavePath + typeof(T).Module.ToString().Replace(".dll", "") + "." + typeof(T).Name + ".settings.json";
        }

        /// <summary>
        /// Save settings for the specified object using the module and name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settings"></param>
        public void SaveSettings<T>(T settings)
        {
            var settingsString = JsonConvert.SerializeObject(settings);
            File.WriteAllText(GetPath<T>(), settingsString);
        }

        /// <summary>
        /// Loads settings for the specified object using the module and name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSettings<T>()
        {
            string path = GetPath<T>();

            if (!File.Exists(path))
            {
                SaveSettings(Activator.CreateInstance<T>());
            }

            string settingsJson = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(settingsJson);
        }
    }
}
