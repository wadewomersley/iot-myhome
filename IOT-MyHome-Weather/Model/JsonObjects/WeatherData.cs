namespace IOT_MyHome.Weather.Model.JsonObjects
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Settings used by the service
    /// </summary>
    public sealed class WeatherData
    {
        /// <summary>
        /// Identifier of location
        /// </summary>
        public string LocationID { get; set; }
        
        /// <summary>
        /// Friendly name of location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Latitude of data
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of data
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<DateTime, WeatherDataItem> Items { get; set; } = new Dictionary<DateTime, WeatherDataItem>();
    }
}
