namespace IOT_MyHome.RunCommand
{
    using IOT_MyHome.Plugins;
    using IOT_MyHome.RunCommand.Controllers;
    using IOT_MyHome.Settings;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Plays an audio file simply on loop.
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <summary>
        /// Friendly name for the plugin.
        /// </summary>
        public string Name => "IOT-MyHome-RunCommand";

        /// <summary>
        /// Description about what the plugin does.
        /// </summary>
        public string Description => "Run a command";

        /// <summary>
        /// Namespace designation for API requests.
        /// </summary>
        public string Designation => "home-runcommand";

        private StaticContentProvider StaticContentHandler;

        private RequestController Controller;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingsManager"></param>
        /// <param name="pluginPath"></param>
        public Plugin(SettingsManager settingsManager, string pluginPath)
        {
            //StaticContentHandler = new StaticContentProvider(pluginPath.TrimEnd('/') + "/IOT-MyHome-RunCommand-Assets");
            Controller = new RequestController();
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
                return new Response();
                //return await StaticContentHandler.GetResponse(request.PathString);
            }
        }

        /// <summary>
        /// Dispose the plugin
        /// </summary>
        public void Dispose()
        {
        }
    }
}
