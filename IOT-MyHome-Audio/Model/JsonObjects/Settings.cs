namespace IOT_MyHome.Audio.Model.JsonObjects
{
    /// <summary>
    /// Settings used by the service
    /// </summary>
    internal sealed class Settings
    {
        /// <summary>
        /// Current volume
        /// </summary>
        public int Volume { get; set; } = 100;

        /// <summary>
        /// Startup file when app is launched
        /// </summary>
        public string StartupFilename { get; set; } = null;

        /// <summary>
        /// Folder to find music
        /// </summary>
        public string MusicFolder { get; set; } = "/music";
    }
}
