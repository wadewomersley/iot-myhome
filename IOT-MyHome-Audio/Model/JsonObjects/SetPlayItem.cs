namespace IOT_MyHome.Audio.Model.JsonObjects
{
    /// <summary>
    /// Passes a search term to trigger playing an item.
    /// </summary>
    internal sealed class SetPlayItem
    {
        /// <summary>
        /// Filename to act on
        /// </summary>
        public string Term { get; set; }
    }
}
