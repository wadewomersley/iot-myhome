namespace IOT_MyHome.Identification
{
    using IOT_MyHome.Settings;
    using System.Threading.Tasks;
    using IOT_MyHome.Identification.Model.JsonObjects;
    using Microsoft.Extensions.Logging;

    internal class Manager
    {
        private SettingsManager SettingsManager { get; set; }
        private Settings ManagerSettings { get; set; }
        private ILogger Logger { get; set; }

        public Manager(SettingsManager manager)
        {
            Logger = Logging.Logger.GetLogger<Manager>();
            SettingsManager = manager;
            ManagerSettings = manager.LoadSettings<Settings>();
        }
    }
}
