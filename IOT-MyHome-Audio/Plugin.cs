namespace IOT_MyHome.Audio
{
    using IOT_MyHome.Audio.Controllers;
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Settings;
    using ManagedBass;
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
        public string Name => "IOT-MyHome-Audio";

        /// <summary>
        /// Description about what the plugin does.
        /// </summary>
        public string Description => "Plays ambient music throughout the home";

        /// <summary>
        /// Namespace designation for API requests.
        /// </summary>
        public string Designation => "home-audio";

        private StaticContentProvider StaticContentHandler;

        private Manager Manager;

        private Player Player;

        private RequestController Controller;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settingsManager"></param>
        /// <param name="pluginPath"></param>
        public Plugin(SettingsManager settingsManager, string pluginPath)
        {
            StaticContentHandler = new StaticContentProvider(pluginPath.TrimEnd('/') + "/IOT-MyHome-Audio-Assets.zip");
            Manager = new Manager(settingsManager);
            Player = new Player(pluginPath);
            Controller = new RequestController(Player, Manager);
        }

        /// <summary>
        /// Called if the plugin can start.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            await Task.Run(async () =>
            {
                Player.SetVolume(Manager.GetStartupVolume());

                if (Manager.GetStartupFile() != null)
                {
                    Thread.Sleep(2000);
                    await Player.SetFileName(Manager.GetStartupFile());
                }
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
        /// Dispose the plugin
        /// </summary>
        public void Dispose()
        {
            Bass.Free();
        }
    }
}
