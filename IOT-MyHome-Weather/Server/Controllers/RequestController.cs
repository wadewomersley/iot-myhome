namespace IOT_MyHome.Weather.Controllers
{
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Weather.Model.JsonObjects;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// API interface for the front end
    /// </summary>
    internal class RequestController
    {
        private ILogger Logger;

        internal RequestController()
        {
            Logger = Logging.Logger.GetLogger<RequestController>();
        }

        internal T GetObject<T>(byte[] json)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json));
        }

        internal async Task<IResponse> Handle(IRequest request)
        {
            return await Task.Run(() =>
            {
                Logger.LogDebug("Handling {0} request to {1}", request.Method, request.PathString);

                switch (request.Path[1])
                {
                    case "settings":
                        return request.Method == "GET" ? GetSettings() : null;
                    case "latest":
                        return request.Method == "GET" ? GetLatest() : null;
                }

                return null;
            });
        }

        private IResponse GetLatest()
        {
            Logger.LogDebug("Getting latest weather");
            
            var data = new WeatherData()
            {
            };

            return new Response(data);
        }

        internal IResponse GetSettings()
        {
            Logger.LogDebug("Getting settings");

            var settings = new Settings()
            {
            };

            return new Response(settings);
        }
    }
}
