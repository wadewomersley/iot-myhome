namespace IOT_MyHome.Plugins
{
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Holds a plugin to provide some extra info.
    /// </summary>
    public class PluginContainer
    {
        /// <summary>
        /// The plugin proper.
        /// </summary>
        public IPlugin Plugin { get; private set; }

        /// <summary>
        /// Assembly the plugin has in it.
        /// </summary>
        private Assembly PluginAssembly { get; set; }

        /// <summary>
        /// Path (directory) of the plugin.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        public PluginContainer(IPlugin plugin, Assembly assembly, string path)
        {
            Plugin = plugin;
            PluginAssembly = assembly;
            Path = path;
        }

        /// <summary>
        /// Passes a <see cref="IRequest"/> to the plugin to get a <see cref="IResponse"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResponse> Handle(IRequest request)
        {
            return await Plugin.Handle(request);
        }
    }
}
