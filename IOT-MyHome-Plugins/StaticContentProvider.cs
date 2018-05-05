namespace IOT_MyHome.Plugins
{
    using SharpCompress.Readers;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Simple static file content provider given a base path.
    /// Returns null if can't handle.
    /// Requests to "" ("/") attempt to load index.html
    /// </summary>
    public class StaticContentProvider
    {
        /// <summary>
        /// Root for requests.
        /// </summary>
        private String Root { get; set; }

        /// <summary>
        /// Provide path to base of static content or path to TAR file.
        /// </summary>
        /// <param name="root"></param>
        /// <exception cref="ArgumentException">File/folder does not exist</exception>
        public StaticContentProvider(string root)
        {
            if (File.Exists(root))
            {
                Root = root;
            }
            else if (Directory.Exists(root))
            {
                Root = root.TrimEnd('/') + "/";
            }
            else
            {
                throw new ArgumentException(String.Format("{0} does not exist.", root));
            }
        }

        /// <summary>
        /// Returns response object or null.
        /// Requests to "" ("/") attempt to load index.html
        /// </summary>
        /// <param name="path"></param>
        /// <returns><see cref="IResponse"/> or null if not handled</returns>
        public async Task<IResponse> GetResponse(string path)
        {
            path = path.Trim('/');

            return await Task.Run(() =>
            {
                byte[] body = null;
                var type = "application/octet-stream";

                if (path == "")
                {
                    path = "index.html";
                }


                if ( File.Exists(Root))
                {
                    using (var streamReader = new FileStream(Root, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (var fileReader = ReaderFactory.Open(streamReader))
                        {
                            while (fileReader.MoveToNextEntry())
                            {
                                if (!fileReader.Entry.Key.Replace("\\", "/").Equals(path))
                                {
                                    continue;
                                }

                                using (var es = fileReader.OpenEntryStream())
                                {
                                    body = new byte[fileReader.Entry.Size];
                                    es.Read(body, 0, (int)fileReader.Entry.Size);

                                    type = MimeTypeMap.GetMimeType(Path.GetExtension(fileReader.Entry.Key));
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var targetFile = Root + path;

                    if (File.Exists(targetFile))
                    {
                        body = File.ReadAllBytes(targetFile);
                        type = MimeTypeMap.GetMimeType(Path.GetExtension(targetFile));
                    }
                }

                if (body == null)
                {
                    return null;
                }

                return new Response()
                {
                    Body = body,
                    ResponseCode = System.Net.HttpStatusCode.OK,
                    ResponseType = type
                };
            });
        }
    }
}
