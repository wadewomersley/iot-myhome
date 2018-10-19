namespace IOT_MyHome.Plugins
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using IOT_MyHome.Settings;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Handles plugin loading and event dispatching.
    /// </summary>
    public class PluginLoader
    {
        /// <summary>
        /// List of loaded plugins wrapped in a container.
        /// </summary>
        public List<PluginContainer> Containers { get; private set; }

        /// <summary>
        /// Reference to the interface that is used for plugins. <see cref="IPlugin"/>.
        /// </summary>
        public Type IType { get; private set; }

        /// <summary>
        /// Logger for logging!
        /// </summary>
        private ILogger Logger { get; set; }

        /// <summary>
        /// Helper for saving/loading settings.
        /// </summary>
        private SettingsManager Settings { get; set; }

        /// <summary>
        /// Sets up the loader.
        /// </summary>
        public PluginLoader(SettingsManager settingsManager)
        {
            Logger = Logging.Logger.GetLogger<PluginLoader>();
            Containers = new List<PluginContainer>();
            IType = typeof(IPlugin);
            Settings = settingsManager;
        }

        /// <summary>
        /// Loads the individual plugins and adds to <see cref="Containers"/>/
        /// </summary>
        public void LoadPlugins()
        {
            string[] plugins = Directory.GetFiles(".", "*.dll", SearchOption.AllDirectories);

            var ignoreTest = new Regex("((Microsoft|System|Newtonsoft|SharpCompress)\\.)", RegexOptions.Compiled);

            plugins = plugins.Where(name => !ignoreTest.IsMatch(name)).ToArray();

            var loadedPaths = new List<string>();

            Array.ForEach(plugins, pluginFile =>
            {
                Logger.LogDebug("Looking at file {0}", pluginFile);

                var pluginContainer = LoadPlugin(pluginFile);

                if (pluginContainer == null)
                {
                    Logger.LogDebug("File {0} did not load as a plugin, ignoring", pluginFile);
                    return;
                }

                if (loadedPaths.Contains(pluginContainer.Plugin.Designation))
                {
                    Logger.LogInformation("Could not load plugin {0} ({1}) as namespace {2} already exists", pluginContainer.Plugin.Name, pluginContainer.Path, pluginContainer.Plugin.Designation);
                    pluginContainer.Plugin.Dispose();

                    return;
                }

                loadedPaths.Add(pluginContainer.Plugin.Designation);
                pluginContainer.Plugin.Start();

                Logger.LogInformation("File {0} loaded as plugin {1}: {2} under namespace {3}", pluginFile, pluginContainer.Plugin.Name, pluginContainer.Plugin.Description, pluginContainer.Plugin.Designation);
                Containers.Add(pluginContainer);
            });
        }

        /// <summary>
        /// Handles a <see cref="IRequest"/> based on its namespace and returns the response.
        /// Returns a 404 if not handled.
        /// </summary>
        /// <param name="designation"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResponse> Handle(string designation, IRequest request)
        {
            foreach (var container in Containers)
            {
                if (container.Plugin.Designation.ToLower().Equals(designation.ToLower()))
                {
                    Logger.LogDebug("Passing request to plugin {0}", container.Plugin.Name);

                    try
                    {
                        return await container.Handle(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Plugin {0} failed to handle request. Exception was {0}: {1}", container.Plugin.Name, ex.GetType().Name, ex.Message);

                        return null;
                    }
                }
            }

            Logger.LogDebug("No handler found for request to designation {0}/{1}", designation, String.Join('/', request.Path));

            return null;
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="pluginFile"></param>
        /// <returns></returns>
        private PluginContainer LoadPlugin(string pluginFile)
        {
            try
            {
                string path = Path.GetFullPath(pluginFile);
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsInterface || type.IsAbstract || type.GetInterface(IType.FullName) == null)
                    {
                        continue;
                    }

                    var pluginInstance = (IPlugin)Activator.CreateInstance(type, new object[] { Settings, Path.GetDirectoryName(path).Replace('\\', '/') });

                    return new PluginContainer(pluginInstance, assembly, pluginFile);
                }
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ((TargetInvocationException)ex).GetBaseException();
                }

                Logger.LogDebug("Ignoring file {0} due to exception {1}: {2}", pluginFile, ex.GetType(), ex.Message);

                if (ex is ReflectionTypeLoadException)
                {
                    var exceptions = ((ReflectionTypeLoadException)ex).LoaderExceptions;
                    foreach(var exception in exceptions)
                    {
                        Logger.LogDebug("{0}: {1}", exception.GetType(), exception.Message);
                    }
                }
            }

            return null;
        }
    }
}
