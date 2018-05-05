namespace IOT_MyHome.Audio.Model.JsonObjects
{
    using System.Linq;

    /// <summary>
    /// List of files available to play
    /// </summary>
    internal sealed class PlaylistData
    {
        /// <summary>
        /// List of <see cref="FileInformation"/> objects
        /// </summary>
        public FileInformation[] Files { get; set; }

        /// <summary>
        /// Calls <see cref="FileInformation.Matches(string)"/> on each item to find a match
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public FileInformation FindMatchingFile(string term)
        {
            return Files.FirstOrDefault(f => f.Matches(term));
        }
    }
}
