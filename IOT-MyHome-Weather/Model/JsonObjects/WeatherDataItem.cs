namespace IOT_MyHome.Weather.Model.JsonObjects
{
    using System;
    
    /// <summary>
    /// Instance of weather data
    /// </summary>
    public sealed class WeatherDataItem
    {
        /// <summary>
        /// Temperature
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Lowest temperature
        /// </summary>
        public double TemperatureMin { get; set; }

        /// <summary>
        /// Highest temperature
        /// </summary>
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Pressure at sea level
        /// </summary>
        public double PressureSea { get; set; }

        /// <summary>
        /// Pressure at ground level
        /// </summary>
        public double PressureGround { get; set; }

        /// <summary>
        /// Humidity
        /// </summary>
        public long Humidity { get; set; }

        /// <summary>
        /// Weather name/title
        /// </summary>
        public string WeatherTitle { get; set; }

        /// <summary>
        /// Weather description
        /// </summary>
        public string WeatherDescription { get; set; }

        /// <summary>
        /// Cloud cover percentage
        /// </summary>
        public long CloudCover { get; set; }

        /// <summary>
        /// Wind speed in knots
        /// </summary>
        public double WindSpeed { get; set; }

        /// <summary>
        /// Wind direction in degrees
        /// </summary>
        public double WindDirection { get; set; }

        /// <summary>
        /// Date/Time this data is relevant for
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
