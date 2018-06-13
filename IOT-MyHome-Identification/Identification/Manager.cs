namespace IOT_MyHome.Identification
{
    using IOT_MyHome.Identification.Model.JsonObjects;
    using IOT_MyHome.Settings;
    using Microsoft.Extensions.Logging;
    using System.Linq;

    internal class Manager
    {
        private SettingsManager SettingsManager { get; set; }
        private Settings ManagerSettings { get; set; }
        private ILogger Logger { get; set; }

        public Manager(SettingsManager manager)
        {
            this.Logger = Logging.Logger.GetLogger<Manager>();
            this.SettingsManager = manager;
            this.ManagerSettings = manager.LoadSettings<Settings>();
        }

        internal Person GetPerson(string faceId)
        {
            return this.ManagerSettings.People.FirstOrDefault(p => p.RemoteIDs.Contains(faceId));
        }

        internal Person[] GetPeople()
        {
            return this.ManagerSettings.People.ToArray();
        }

        internal void UpdatePerson(Person person)
        {
            var existing = this.ManagerSettings.People.FirstOrDefault(p => p.RemoteIDs.Contains(person.RemoteIDs[0]));
            this.ManagerSettings.People.Remove(existing);
            this.ManagerSettings.People.Add(person);
            this.SettingsManager.SaveSettings(this.ManagerSettings);
        }

        internal void AddPerson(Person person)
        {
            this.ManagerSettings.People.Add(person);
            this.SettingsManager.SaveSettings(this.ManagerSettings);
        }

        internal string GetAmazonRegion()
        {
            return this.ManagerSettings.AmazonRegion;
        }

        internal string GetAmazonAccessKey()
        {
            return this.ManagerSettings.AmazonAccessKey;
        }

        internal string GetAmazonSecretKey()
        {
            return this.ManagerSettings.AmazonSecretKey;
        }

        internal string GetAmazonRekognitionCollection()
        {
            return this.ManagerSettings.AmazonRekognitionCollection;
        }

        internal int GetSleepAfterMatchInterval()
        {
            return this.ManagerSettings.SleepAfterMatchInterval;
        }

        internal float GetRequiredSimilary()
        {
            return this.ManagerSettings.RequiredSimilary;
        }

        internal int GetCaptureInterval()
        {
            return this.ManagerSettings.CaptureInterval;
        }
    }
}
