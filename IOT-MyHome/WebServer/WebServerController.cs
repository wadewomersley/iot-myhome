﻿namespace IOT_MyHome.WebServer
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    internal class WebServerStartup : IStartup
    {
        private RequestDelegate Handler;

        public WebServerStartup(RequestDelegate handler)
        {
            Handler = handler;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(Handler);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}