namespace IOT_MyHome.Weather.Model.JsonObjects
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Settings used by the service
    /// </summary>
    internal sealed class WeatherData
    {
        public string LocationID { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Dictionary<DateTime, WeatherDataItem> Items { get; set; } = new Dictionary<DateTime, WeatherDataItem>();
    }
}
