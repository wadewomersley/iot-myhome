namespace IOT_MyHome.RunCommand.Controllers
{
    using IOT_MyHome.Plugins;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// API interface for the front end
    /// </summary>
    internal class RequestController
    {
        private ILogger Logger;

        internal RequestController()
        {
            Logger = Logging.Logger.GetLogger<RequestController>();
        }

        internal T GetObject<T>(byte[] json)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json));
        }

        internal async Task<IResponse> Handle(IRequest request)
        {
            Logger.LogDebug("Handling {0} request to {1}", request.Method, request.PathString);

            switch (request.Path[1])
            {
                case "run":
                    return request.Method == "GET" ? await RunCommand(request.Search["command"]) : null;
            }

            return null;
        }

        internal async Task<IResponse> RunCommand(string command, bool waitForExit = false)
        {
            return await Task.Run(() =>
            {
                var escapedArgs = command.Replace("\"", "\\\"");

                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = waitForExit,
                        RedirectStandardInput = waitForExit
                    }
                };

                process.Start();

                if (waitForExit)
                {
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    string err = process.StandardError.ReadToEnd();

                    dynamic outputObject = new ExpandoObject();
                    outputObject.output = output;
                    outputObject.error = err;

                    return new Response(JsonConvert.SerializeObject(outputObject));
                }

                return new Response("");
            });
        }
    }
}
