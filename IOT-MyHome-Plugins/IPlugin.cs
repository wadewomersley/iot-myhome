namespace IOT_MyHome.Plugins
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Basic plugin interface.
    /// </summary>
    public interface IPlugin : IDisposable
    {
        /// <summary>
        /// Friendly name for the plugin.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Description about what the plugin does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Namespace designation for API requests.
        /// </summary>
        string Designation { get; }

        /// <summary>
        /// Handler for when a request is incoming to the plugin.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<IResponse> Handle(IRequest request);

        /// <summary>
        /// Called if the plugin can start.
        /// </summary>
        /// <returns></returns>
        Task Start();
    }
}
