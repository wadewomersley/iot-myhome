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
    using System.Collections.Generic;
    using IOT_MyHome.Identification.Services;
    using System;

    /// <summary>
    /// API interface for the front end
    /// </summary>
    internal class RequestController
    {
        private Manager Manager;
        private FacialRecognition Recognition;
        private ILogger Logger;

        internal RequestController(Manager manager, FacialRecognition recognition)
        {
            Recognition = recognition;
            Logger = Logging.Logger.GetLogger<RequestController>();
            Manager = manager;
        }

        internal T GetObject<T>(byte[] json)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json));
        }

        internal async Task<IResponse> Handle(IRequest request)
        {
            Logger.LogDebug("Handling {0} request to {1}", request.Method, request.PathString);

            switch (request.Path[1])
            {
                case "settings":
                    return request.Method == "GET" ? await GetSettings() : null;
                case "lastImageCaptured":
                    return request.Method == "GET" ? await GetLastImageCaptured() : null;
            }

            return null;
        }

        internal async Task<Response> GetLastImageCaptured()
        {
            return await Task.Run(() => {
                Logger.LogDebug("Getting last image captured");

                var base64Image = "data:image/png;base64,";

                if (Recognition.LastImageCapturedPng != null && Recognition.LastImageCapturedPng.Length > 0)
                {
                    base64Image += Convert.ToBase64String(Recognition.LastImageCapturedPng);
                }

                return new Response(base64Image);
            });
        }

        internal async Task<IResponse> GetSettings()
        {
            return await Task.Run(() =>
            {
                Logger.LogDebug("Getting settings");

                var settings = new Settings()
                {
                    AmazonRegion = Manager.GetAmazonRegion(),
                    AmazonAccessKey = Manager.GetAmazonAccessKey(),
                    AmazonSecretKey = Manager.GetAmazonSecretKey(),
                    RequiredSimilary = Manager.GetRequiredSimilary(),
                    AmazonRekognitionCollection = Manager.GetAmazonRekognitionCollection(),
                    CaptureInterval = Manager.GetCaptureInterval(),
                    People = new List<Person>(Manager.GetPeople()),
                    SleepAfterMatchInterval = Manager.GetSleepAfterMatchInterval()
                };

                return new Response(settings);
            });
        }
    }
}
