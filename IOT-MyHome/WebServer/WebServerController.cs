namespace IOT_MyHome.WebServer
{
    using IOT_MyHome.Settings;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    internal class WebServerStartup : IStartup
    {
        private RequestDelegate Handler;
        private SettingsManager SettingsManager;

        public WebServerStartup(RequestDelegate handler, SettingsManager settingsManager)
        {
            Handler = handler;
            SettingsManager = settingsManager;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<BasicAuthMiddleware>(SettingsManager);
            app.Run(Handler);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}
