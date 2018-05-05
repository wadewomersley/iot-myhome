namespace IOT_MyHome.Plugins
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// Response from the web API
    /// </summary>
    public class Response : IResponse
    {
        /// <summary>
        /// Response code provided to client
        /// </summary>
        public HttpStatusCode ResponseCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// Raw byte[] body
        /// </summary>
        public byte[] Body { get; set; } = new byte[0];

        /// <summary>
        /// Content type of response, default "application/json";
        /// </summary>
        public string ResponseType { get; set; } = "application/json";

        /// <summary>
        /// Custom headers
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Constructor for just a status code response
        /// </summary>
        /// <param name="code"></param>
        public Response(HttpStatusCode code)
        {
            ResponseCode = code;
        }

        /// <summary>
        /// Constructor for just a status code response
        /// </summary>
        /// <param name="code"></param>
        /// <param name="headers"></param>
        public Response(HttpStatusCode code, Dictionary<string, string> headers)
        {
            ResponseCode = code;
            Headers = headers;
        }

        /// <summary>
        /// Constructor to JSON encode an object
        /// </summary>
        /// <param name="jsonSerializableObject"></param>
        public Response(object jsonSerializableObject)
        {
            Body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonSerializableObject));
        }

        /// <summary>
        /// Constructor to JSON encode an object with optional status code response and type
        /// </summary>
        /// <param name="jsonSerializableObject"></param>
        /// <param name="code"></param>
        /// <param name="responseType"></param>
        public Response(object jsonSerializableObject, HttpStatusCode code = HttpStatusCode.OK, string responseType = "application/json")
        {
            ResponseCode = code;
            ResponseType = responseType;
            Body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonSerializableObject));
        }

        /// <summary>
        /// Constructor to JSON encode an object with optional status code response and type
        /// </summary>
        /// <param name="jsonSerializableObject"></param>
        /// <param name="responseType"></param>
        /// <param name="code"></param>
        public Response(object jsonSerializableObject, string responseType = "application/json", HttpStatusCode code = HttpStatusCode.OK)
        {
            ResponseCode = code;
            ResponseType = responseType;
            Body = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonSerializableObject));
        }
    }
}
