namespace IOT_MyHome.WebServer
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using IOT_MyHome.Settings;
    using IOT_MyHome.WebServer.Model.JsonObjects;
    using Microsoft.AspNetCore.Http;

    internal class BasicAuthMiddleware
    {
        private readonly RequestDelegate NextDelegate;
        private SettingsManager SettingsManager;
        private Settings SettingsCache;
        private DateTime SettingsCacheExpires = new DateTime();

        public BasicAuthMiddleware(RequestDelegate next, SettingsManager settingsManager)
        {
            this.NextDelegate = next;
            this.SettingsManager = settingsManager;
        }

        private Settings GetSettings()
        {
            if (this.SettingsCache == null || this.SettingsCacheExpires < DateTime.UtcNow)
            {
                this.SettingsCache = this.SettingsManager.LoadSettings<Settings>();
                this.SettingsCacheExpires = DateTime.UtcNow.AddSeconds(3);
            }

            return this.SettingsCache;
        }

        public async Task Invoke(HttpContext context)
        {
            var settings = this.GetSettings();
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                var userPass = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim()));

                var username = userPass.Split(':', 2)[0];
                var password = userPass.Split(':', 2)[1];

                if (IsAuthorized(username, password))
                {
                    await NextDelegate.Invoke(context);
                    return;
                }
            }

            context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{settings.Realm}\"";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        public bool IsAuthorized(string username, string password)
        {
            var settings = this.GetSettings();

            return username.Equals(settings.Username)
                   && password.Equals(settings.Password);
        }
    }
}