namespace IOT_MyHome.Weather.Model.JsonObjects
{
    using System;
    
    public sealed class WeatherDataItem
    {
        public double Temperature { get; set; }
        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public double PressureSea { get; set; }
        public double PressureGround { get; set; }
        public long Humidity { get; set; }
        public string WeatherTitle { get; set; }
        public string WeatherDescription { get; set; }
        public long CloudCover { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public DateTime DateTime { get; set; }
    }
}
