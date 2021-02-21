namespace IOT_MyHome.Logging
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Be sure to add 
    /// using Microsoft.Extensions.Logging;
    /// to your file for easy logging!
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Stores the logger factory
        /// </summary>
        private static ILoggerFactory _LoggerFactory { get; } = new LoggerFactory();

        /// <summary>
        /// Stors a list of loggers
        /// </summary>
        private static ConcurrentDictionary<Type, dynamic> Loggers { get; set; } = new ConcurrentDictionary<Type, dynamic>();

        /// <summary>
        /// Sets up the LoggerFactory.
        /// </summary>
        static Logger()
        {
            _LoggerFactory = LoggerFactory.Create(builder => {
#if DEBUG
                builder.AddFilter(l => l >= LogLevel.Debug);
#else
                builder.AddFilter(l => l >= LogLevel.Information);
#endif
                builder.AddConsole();
            });
        }

        /// <summary>
        /// Grab a logger for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger GetLogger<T>()
        {
            Loggers.TryAdd(typeof(T), _LoggerFactory.CreateLogger<T>());

            return Loggers[typeof(T)];
        }
    }
}
