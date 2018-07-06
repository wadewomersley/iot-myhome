namespace IOT_MyHome
{
    using IOT_MyHome.Plugins;
    using IOT_MyHome.Settings;
    using IOT_MyHome.WebServer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Net;
    using System.Reflection;

    class Program
    {
        static void Main(string[] args)
        {
            var logger = Logging.Logger.GetLogger<object>();
            logger.LogDebug("Starting up");

            var settingsManager = new SettingsManager(Directory.GetCurrentDirectory());

            var plugins = new PluginLoader(settingsManager);
            plugins.LoadPlugins();

            var router = new Routing(plugins, settingsManager);
            var WebServerController = new WebServerStartup(router.Handler, settingsManager);

            var host = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IStartup>(WebServerController);
                })
                .UseSetting(WebHostDefaults.ApplicationKey, typeof(WebServerStartup).GetTypeInfo().Assembly.FullName)
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 16000);

                    if (File.Exists("ssl.pfx"))
                    {
                        logger.LogInformation("Enabling HTTPS");
                        options.Listen(IPAddress.Loopback, 16001, listenOptions =>
                        {
                            listenOptions.UseHttps("ssl.pfx", "PASSWORD");
                        });
                    }
                    else
                    {
                        logger.LogInformation("Not enabling HTTPS, can't find certificate");
                    }
                })
                .Build();

            host.Run();
        }
    }
}
