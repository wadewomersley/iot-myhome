namespace IOT_MyHome.WebServer
{
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Settings;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Deals with handling requests and sending to plugins
    /// </summary>
    internal class Routing
    {
        private PluginLoader Plugins;

        private ILogger Logger;

        private SettingsManager SettingsManager;

        private StaticContentProvider StaticProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plugins"></param>
        /// <param name="settingsManager"></param>
        public Routing(PluginLoader plugins, SettingsManager settingsManager)
        {
            Plugins = plugins;
            SettingsManager = settingsManager;
            Logger = Logging.Logger.GetLogger<Routing>();
            StaticProvider = new StaticContentProvider(Directory.GetCurrentDirectory() + "/Assets");
        }

        /// <summary>
        /// Handles an incoming incoming request and sends a response.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Handler(HttpContext context)
        {
            var parts = context.Request.Path.Value?.Trim('/');

            Logger.LogDebug("{0} request to {1}", context.Request.Method, parts);

            var requestTarget = parts.IndexOf('/') > -1 ? parts.Substring(0, parts.IndexOf('/')) : parts;

            byte[] buffer = new byte[0];

            if (context.Request.Method != "GET")
            {
                try
                {
                    using (var reader = new MemoryStream())
                    {
                        await context.Request.Body.CopyToAsync(reader);
                        reader.Position = 0L;
                        buffer = new byte[reader.Length];
                        await reader.ReadAsync(buffer, 0, (int)reader.Length);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("Failed to read incoming request {0}: {1}", ex.GetType().ToString(), ex.Message);
                }
            }

            IResponse response = new Response(HttpStatusCode.NotFound);

            if (requestTarget.Equals("plugins"))
            {

                parts = parts.Substring(parts.IndexOf('/') + 1);
                requestTarget = parts.IndexOf('/') > -1 ? parts.Substring(0, parts.IndexOf('/')) : parts;
                parts = requestTarget.Length + 1 < parts.Length ? parts.Substring(requestTarget.Length + 1) : "";

                if (requestTarget.Length == 0)
                {
                    response = new Response(Plugins.Containers, HttpStatusCode.OK);
                }
                else if (parts.Length == 0 && !context.Request.Path.Value.EndsWith('/'))
                {
                    Logger.LogDebug("Redirect plugin request for {0} to include trailing slash", requestTarget);

                    response = new Response(HttpStatusCode.MovedPermanently, new Dictionary<string, string>() { { "Location", "/plugins/" + requestTarget + "/" } });
                }
                else
                {
                    var request = new Request(parts, context.Request.Method, context.Request.QueryString.HasValue ? context.Request.QueryString.Value.Substring(1) : "", buffer);

                    try
                    {
                        response = await Plugins.Handle(requestTarget, request);
                    }
                    catch (Exception ex)
                    {
                        response = new Response(JsonConvert.SerializeObject(ex.Message), HttpStatusCode.InternalServerError);
                    }
                }
            }
            else
            {
                response = await StaticProvider.GetResponse(parts);
            }

            foreach (var kvp in response.Headers)
            {
                context.Response.Headers.Add(kvp.Key, kvp.Value);
            }

            context.Response.StatusCode = (int)response.ResponseCode;
            context.Response.ContentType = response.ResponseType;
            await context.Response.Body.WriteAsync(response.Body, 0, response.Body.Length);
        }
    }
}
