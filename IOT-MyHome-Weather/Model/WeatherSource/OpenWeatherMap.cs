namespace IOT_MyHome.Weather.Model.WeatherSource
{
    using IOT_MyHome.Weather.Model.JsonObjects;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class OpenWeatherMap
    {
        private string AppId;
        private const string SourceUrl = "http://api.openweathermap.org/data/2.5/forecast?id=2633352&units=metric&APPID=";

        public OpenWeatherMap(string appId)
        {
            AppId = appId;
        }

        public async Task<WeatherData> GetForecast()
        {
            using (var client = new HttpClient())
            {
                using (var result = await client.GetAsync(SourceUrl + AppId))
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var jsonSource = await result.Content.ReadAsStringAsync();
                    var ret = new WeatherData();
                    dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(jsonSource);

                    ret.LocationID = Convert.ToString(json.city.id);
                    ret.Location = json.city.name + ", " + json.city.country;
                    ret.Latitude = json.city.coord.lat;
                    ret.Longitude = json.city.coord.lon;

                    foreach (dynamic item in json.list)
                    {
                        var dt = DateTimeOffset.FromUnixTimeSeconds(item.dt).DateTime.ToLocalTime();
                        ret.Items.Add(dt, new WeatherDataItem()
                        {
                            DateTime = dt,
                            CloudCover = item.clouds.all,
                            Humidity = item.main.humidity,
                            PressureGround = item.main.grnd_level,
                            PressureSea = item.main.sea_level,
                            Temperature = item.main.temp,
                            TemperatureMin = item.main.temp_min,
                            TemperatureMax = item.main.temp_max,
                            WeatherDescription = item.weather[0].description,
                            WeatherTitle = item.weather[0].main,
                            WindDirection = item.wind.deg,
                            WindSpeed = item.wind.speed
                        });
                    }

                    return ret;
                }
            }
        }
    }
}
