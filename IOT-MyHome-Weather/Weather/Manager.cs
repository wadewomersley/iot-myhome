namespace IOT_MyHome.Weather
{
    using IOT_MyHome.Settings;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using IOT_MyHome.Weather.Model.JsonObjects;
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

        public WeatherData GetForecast()
        {

        }
    }
}
