namespace IOT_MyHome.Identification.Model.JsonObjects
{
    using System.Collections.Generic;

    /// <summary>
    /// Settings used by the service
    /// </summary>
    internal sealed class Settings
    {
        public float RequiredSimilary { get; set; } = 75;

        public List<Person> People { get; set; } = new List<Person>();

        public int SleepInterval { get; set; } = 1000;

        public int SleepAfterMatchInterval { get; set; } = 5000;

        public int CaptureInterval { get; set; } = 1000;

        public string AmazonRegion { get; set; } = "eu-west-1";

        public string AmazonAccessKey { get; set; } = "";

        public string AmazonSecretKey { get; set; } = "";

        public string AmazonRekognitionCollection { get; set; } = "home-control";
    }

    /// <summary>
    /// Person in settings
    /// </summary>
    internal sealed class Person
    {
        public IList<string> RemoteIDs { get; set; }

        public string Name { get; set; }

        public string SpokenName { get; set; }
    }
}
