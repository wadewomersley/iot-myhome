namespace IOT_MyHome.Weather.Model.JsonObjects
{
    /// <summary>
    /// Settings used by the service
    /// </summary>
    internal sealed class Settings
    {
        /// <summary>
        /// APPID for OpenWeatherMap
        /// </summary>
        public string OpenWeatherMapAppId { get; set; }

        /// <summary>
        /// Location for OpenWeatherMap
        /// </summary>
        public int OpenWeatherMapLocation { get; set; }
    }
}
