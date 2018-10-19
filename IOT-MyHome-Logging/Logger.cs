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
        private static ILoggerFactory LoggerFactory { get; } = new LoggerFactory();

        /// <summary>
        /// Stors a list of loggers
        /// </summary>
        private static ConcurrentDictionary<Type, dynamic> Loggers { get; set; } = new ConcurrentDictionary<Type, dynamic>();

        /// <summary>
        /// Sets up the LoggerFactory.
        /// </summary>
        static Logger()
        {
#if DEBUG
            LoggerFactory.AddProvider(new CustomConsoleLoggerProvider((n, l) => l >= LogLevel.Debug, true));
#else
            LoggerFactory.AddProvider(new ConsoleLoggerProvider((n, l) => l >= LogLevel.Information, true));
#endif
        }

        /// <summary>
        /// Grab a logger for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger GetLogger<T>()
        {
            Loggers.TryAdd(typeof(T), LoggerFactory.CreateLogger<T>());

            return Loggers[typeof(T)];
        }
    }
}
