namespace IOT_MyHome.Plugins
{
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// Interface for the response from the web API
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Response code provided to client
        /// </summary>
        HttpStatusCode ResponseCode { get; set; }

        /// <summary>
        /// Content type of response, default "application/json";
        /// </summary>
        string ResponseType { get; set; }

        /// <summary>
        /// Raw byte[] body
        /// </summary>
        byte[] Body { get; set; }
        /// <summary>
        /// Custom headers
        /// </summary>
        Dictionary<string, string> Headers { get; set; }
    }
}
