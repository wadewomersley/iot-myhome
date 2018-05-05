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
        private const string SourceUrl = "http://api.openweathermap.org/data/2.5/forecast?id=2633352&APPID=";

        public OpenWeatherMap(string appId)
        {
            AppId = appId;
        }

        public async Task<WeatherData[]> GetForecast()
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
                    var ret = new List<WeatherData>();
                    dynamic json = JsonConvert.DeserializeObject<ExpandoObject>(jsonSource);
                    

                    return ret.ToArray();
                }
            }
        }
    }
}
