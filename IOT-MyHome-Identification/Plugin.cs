namespace IOT_MyHome.Identification
{
    using IOT_MyHome.Identification.Controllers;
    using IOT_MyHome.Identification.Services;
    using IOT_MyHome.Identification.Utilities;
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Settings;
    using System.Threading.Tasks;

    /// <summary>
    /// Plays an audio file simply on loop.
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <summary>
        /// Friendly name for the plugin.
        /// </summary>
        public string Name => "IOT-MyHome-Identification";

        /// <summary>
        /// Description about what the plugin does.
        /// </summary>
        public string Description => "Identification stuff";

        /// <summary>
        /// Namespace designation for API requests.
        /// </summary>
        public string Designation => "home-identification";
        
        private StaticContentProvider StaticContentHandler;

        private Manager Manager;

        private RequestController Controller;
        private ICamera Camera;
        private FacialRecognition Recognition;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingsManager"></param>
        /// <param name="pluginPath"></param>
        public Plugin(SettingsManager settingsManager, string pluginPath)
        {
            Manager = new Manager(settingsManager);
            StaticContentHandler = new StaticContentProvider(pluginPath.TrimEnd('/') + "/IOT-MyHome-Identification-Assets");
            Camera = new USBCamera(Manager.GetCaptureInterval());
            Recognition = new FacialRecognition(Manager, this.Camera);
            Controller = new RequestController(Manager, Recognition);
        }

        /// <summary>
        /// Called if the plugin can start.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            await Task.Run(async () =>
            {
                await this.Recognition.Start();
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
