namespace IOT_MyHome.Plugins
{
    using System.Collections.Specialized;
    using System.Web;

    /// <summary>
    /// Request to the web API. Given to plugins to handle.
    /// </summary>
    public class Request : IRequest
    {
        /// <summary>
        /// Path requested, relative to plugin root.
        /// </summary>
        /// 
        public string[] Path { get; private set; }

        /// <summary>
        /// Path requested, relative to plugin root.
        /// </summary>
        public string PathString { get; private set; }

        /// <summary>
        /// Search arguments provided, if available.
        /// </summary>
        public NameValueCollection Search { get; private set; }

        /// <summary>
        /// Raw post/put data, if available.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// HTTP method used for the request.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <param name="search"></param>
        /// <param name="body"></param>
        public Request(string path, string method, string search = null, byte[] body = null)
        {
            PathString = path.Trim('/');
            Path = PathString.Split('/');
            Method = method;

            if (search != null)
            {
                Search = HttpUtility.ParseQueryString(search);
            }

            Data = body;
        }
    }
}
