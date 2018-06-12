namespace IOT_MyHome.Identification.Controllers
{
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Identification.Model.JsonObjects;
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
        private Manager Manager;
        private ILogger Logger;

        internal RequestController(Manager manager)
        {
            Logger = Logging.Logger.GetLogger<RequestController>();
            Manager = manager;
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
                }

                return null;
            });
        }

        internal async Task<IResponse> GetSettings()
        {
            return await Task.Run(() =>
            {
                Logger.LogDebug("Getting settings");

                var settings = new Settings()
                {
                };

                return new Response(settings);
            });
        }
    }
}
