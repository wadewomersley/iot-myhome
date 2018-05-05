namespace IOT_MyHome.Plugins
{
    using System.Collections.Specialized;

    /// <summary>
    /// Request interface to the web API. Given to plugins to handle.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Path requested, relative to plugin root.
        /// </summary>
        string[] Path { get; }

        /// <summary>
        /// Path requested, relative to plugin root.
        /// </summary>
        string PathString { get; }

        /// <summary>
        /// Search arguments provided, if available.
        /// </summary>
        NameValueCollection Search { get; }

        /// <summary>
        /// Raw post/put data, if available.
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// HTTP method used for the request.
        /// </summary>
        string Method { get; }
    }
}
