﻿namespace IOT_MyHome.Weather
{
    using IOT_MyHome.Weather.Controllers;
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Settings;
    using System.Threading.Tasks;
    using IOT_MyHome.Weather.Model.WeatherSource;

    /// <summary>
    /// Plays an audio file simply on loop.
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <summary>
        /// Friendly name for the plugin.
        /// </summary>
        public string Name => "IOT-MyHome-Weather";

        /// <summary>
        /// Description about what the plugin does.
        /// </summary>
        public string Description => "Weather stuff";

        /// <summary>
        /// Namespace designation for API requests.
        /// </summary>
        public string Designation => "home-weather";
        
        private StaticContentProvider StaticContentHandler;

        private Manager Manager;

        private RequestController Controller;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingsManager"></param>
        /// <param name="pluginPath"></param>
        public Plugin(SettingsManager settingsManager, string pluginPath)
        {
            Manager = new Manager(settingsManager);
            StaticContentHandler = new StaticContentProvider(pluginPath.TrimEnd('/') + "/IOT-MyHome-Weather-Assets");
            Controller = new RequestController(Manager);
        }

        /// <summary>
        /// Called if the plugin can start.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            await Task.Run(() =>
            {
            });
        }

        /// <summary>
        /// Handler for when a request is incoming to the plugin.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResponse> Handle(IRequest request)
        {
            if (request.Path[0] == "api")
            {
                return await Controller.Handle(request);
            }
            else
            {
                return await StaticContentHandler.GetResponse(request.PathString);
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
