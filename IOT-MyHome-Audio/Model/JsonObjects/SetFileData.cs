namespace IOT_MyHome.Audio.Model.JsonObjects
{
    /// <summary>
    /// Passes a filename back to the service (e.g. to set a file to play or a startupfile)
    /// </summary>
    internal sealed class SetFileData
    {
        /// <summary>
        /// Filename to act on
        /// </summary>
        public string FileName { get; set; }
    }
}
