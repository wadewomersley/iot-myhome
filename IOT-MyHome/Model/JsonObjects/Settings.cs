namespace IOT_MyHome.WebServer.Model.JsonObjects
{
    /// <summary>
    /// Settings used by the service
    /// </summary>
    internal sealed class Settings
    {
        /// <summary>
        /// Username for basic auth.
        /// </summary>
        public string Username { get; set; } = "admin";

        /// <summary>
        /// Password for basic auth.
        /// </summary>
        public string Password { get; set; } = "password";

        /// <summary>
        /// Realm for basic auth.
        /// </summary>
        public string Realm { get; set; } = "IOT MyHome";
    }
}
