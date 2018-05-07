namespace IOT_MyHome.Weather
{
    using IOT_MyHome.Settings;
    using IOT_MyHome.Weather.Model.JsonObjects;
    using IOT_MyHome.Weather.Model.WeatherSource;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    internal class Manager
    {
        private SettingsManager SettingsManager { get; set; }
        private Settings ManagerSettings { get; set; }
        private ILogger Logger { get; set; }
        private OpenWeatherMap WeatherDataSource { get; set; }

        public Manager(SettingsManager manager)
        {
            Logger = Logging.Logger.GetLogger<Manager>();
            SettingsManager = manager;
            ManagerSettings = manager.LoadSettings<Settings>();
            WeatherDataSource = new OpenWeatherMap(GetOpenWeatherMapAppID(), GetOpenWeatherMapLocation());
        }

        internal void SaveOpenWeatherMapAppID(string appId)
        {
            ManagerSettings.OpenWeatherMapAppId = appId;
            SettingsManager.SaveSettings(ManagerSettings);
        }

        internal string GetOpenWeatherMapAppID()
        {
            return ManagerSettings.OpenWeatherMapAppId;
        }

        internal void SaveOpenWeatherMapLocation(int location)
        {
            ManagerSettings.OpenWeatherMapLocation = location;
            SettingsManager.SaveSettings(ManagerSettings);
        }

        internal int GetOpenWeatherMapLocation()
        {
            return ManagerSettings.OpenWeatherMapLocation;
        }

        internal async Task<WeatherData> GetLatest()
        {
            return await WeatherDataSource.GetForecast();
        }
    }
}
