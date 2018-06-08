namespace IOT_MyHome.Audio.Controllers
{
    using Audio;
    using IOT_MyHome.Audio.Model.JsonObjects;
    using IOT_MyHome.Plugins;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// API interface for the front end
    /// </summary>
    internal class RequestController
    {
        private Player Player;
        private Manager Manager;
        private ILogger Logger;

        internal RequestController(Player player, Manager manager)
        {
            Player = player;
            Manager = manager;
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
                case "settings":
                    return request.Method == "GET" ? await GetSettings() : null;
                case "volume":
                    return request.Method == "PUT" ? await SetVolume(GetObject<SetVolumeData>(request.Data)) : null;
                case "play":
                    return request.Method == "PUT" ? await SetFileToPlay(GetObject<SetPlayItem>(request.Data)) : null;
                case "startupFile":
                    return request.Method == "PUT" ? await SetStartupFile(GetObject<SetPlayItem>(request.Data)) : null;
                case "playlist":
                    return request.Method == "GET" ? new Response(await GetPlaylist()) : null;
            }

            return null;
        }

        private async Task<PlaylistData> GetPlaylist()
        {
            return await Task.Run(() =>
            {
                Logger.LogDebug("Getting playlist");

                var fileList = Manager.GetFileList();
                var files = fileList.Select(f => FileInformation.FromStorage(f.Replace("\\", "/"))).ToList();
                return new PlaylistData() { Files = files.ToArray() };
            });
        }

        internal async Task<IResponse> GetSettings()
        {
            return await Task.Run(() =>
            {
                Logger.LogDebug("Getting settings");

                var settings = new Settings()
                {
                    StartupFilename = Manager.GetStartupFile(),
                    Volume = Manager.GetStartupVolume(),
                    MusicFolder = Manager.GetMusicFolder(),
                };

                return new Response(settings);
            });
        }

        internal async Task<IResponse> SetVolume(SetVolumeData data)
        {
            return await Task.Run(() =>
            {
                Logger.LogDebug("Setting volume to {0}", data.Volume);

                Player.SetVolume(data.Volume);
                Manager.SaveStartupVolume(data.Volume);

                return new Response(HttpStatusCode.NoContent);
            });
        }

        internal async Task<IResponse> SetFileToPlay(SetPlayItem data)
        {
            if (data.Term == null)
            {
                Logger.LogDebug("Disabling startup file");

                await Player.SetFileName(null);

                return new Response(HttpStatusCode.NoContent);
            }

            Logger.LogDebug("Finding file to play based on term {0}", data.Term);

            var file = (await GetPlaylist()).FindMatchingFile(data.Term);

            if (file != null)
            {
                Logger.LogDebug("Settings file to play to {0}", file.FileName);

                await Player.SetFileName(file.FileName);

                return new Response(HttpStatusCode.NoContent);
            }
            else
            {
                Logger.LogDebug("Could not find matching file for term {0}", data.Term);

                return new Response(HttpStatusCode.NotFound);
            }
        }

        internal async Task<IResponse> SetStartupFile(SetPlayItem data)
        {

            if (data.Term is null)
            {
                Logger.LogDebug("Removing startup file");

                Manager.SaveStartupFile(null);

                return new Response(HttpStatusCode.NoContent);
            }

            Logger.LogDebug("Finding file to set as startup file based on term {0}", data.Term);

            var file = (await GetPlaylist()).FindMatchingFile(data.Term);

            if (file != null)
            {
                Logger.LogDebug("Settings startup file to play to {0}", file.FileName);

                Manager.SaveStartupFile(file.FileName);

                return new Response(HttpStatusCode.NoContent);
            }
            else
            {
                Logger.LogDebug("Could not find matching file for term {0}", data.Term);

                return new Response(HttpStatusCode.NotFound);
            }
        }

        internal IResponse GetFileList()
        {
            return new Response(GetPlaylist());
        }
    }
}
