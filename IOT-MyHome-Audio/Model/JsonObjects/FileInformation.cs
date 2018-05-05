namespace IOT_MyHome.Audio.Model.JsonObjects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Information about a specific file so it can be viewed but also gives the disk filename
    /// </summary>
    internal sealed class FileInformation
    {
        public readonly static Regex TidyTerm = new Regex(@"[^\w.' \-]", RegexOptions.Compiled);

        /// <summary>
        /// Filename on the filesystem
        /// </summary>
        public string FileName { get; set; }


        /// <summary>
        /// Friendly display name for the file
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// List of things that trigger this item.
        /// </summary>
        public string[] SearchTerms { get; set; }

        /// <summary>
        /// Check if a search term matches this item
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        internal bool Matches(string term)
        {
            term = TidyTerm.Replace(term, "").ToLowerInvariant().Trim();

            return Array.IndexOf(SearchTerms, term) > -1;
        }

        /// <summary>
        /// Create an object for a file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        internal static FileInformation FromStorage(string file)
        {
            var fname = Path.GetTempPath() + "/iot-myhome-audio-temp" + Path.GetExtension(file);
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[32768];

                fs.Read(buffer, 0, 16384);
                fs.Seek(-16384, SeekOrigin.End);
                fs.Read(buffer, 16384, 16384);

                File.WriteAllBytes(fname, buffer);
            }

            var musicProperties = TagLib.File.Create(fname);

            File.Delete(fname);

            var titleTerms = musicProperties.Tag.Title?.Split(new char[] { ',' }).Select(item => TidyTerm.Replace(item, "").ToLowerInvariant().Trim());
            var terms = new List<string>(titleTerms ?? new string[0]);
            terms.Add(Path.GetFileNameWithoutExtension(file).ToLowerInvariant().Trim());

            terms.Remove("");

            terms = terms.Distinct().ToList();

            return new FileInformation()
            {
                FileName = file,
                DisplayName = Path.GetFileNameWithoutExtension(file),
                SearchTerms = terms.ToArray()
            };
        }
    }
}
