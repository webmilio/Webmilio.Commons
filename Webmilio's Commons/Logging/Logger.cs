using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Webmilio.Commons.Logging
{
    public class Logger
    {
        public delegate void LogDelegate(LogLevel level, string message);
        private static readonly ConcurrentDictionary<string, Logger> _loggers = new ConcurrentDictionary<string, Logger>(new LoggerNameEqualityComparer());


        protected Logger(string loggerName, bool toSystemDiagnostics, bool toConsole)
        {
            LoggerName = loggerName;

            ToSystemDiagnostics = toSystemDiagnostics;
            ToConsole = toConsole;

            LogLevel = LogLevel.Info;
        }


        public virtual void Log(LogLevel level, string message)
        {
            if (level < LogLevel)
                return;

            message = $"{DateTime.Now.ToLongTimeString()} [{level.ToString()}] : {message}";

            if (ToSystemDiagnostics)
                System.Diagnostics.Debug.WriteLine(message);

            if (ToConsole)
            {
                if (level > LogLevel.Log)
                {
                    if (level == LogLevel.Warning)
                        WriteLine(message, ConsoleColor.Yellow);
                    else if (level > LogLevel.Warning)
                        WriteLine(message, ConsoleColor.Red);
                }
                else
                    Console.WriteLine(message);
            }
        }


        private void Write(string message, ConsoleColor color)
        {
            ConsoleColor ogColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(message);

            Console.ForegroundColor = ogColor;
        }

        private void WriteLine(string message, ConsoleColor color) => Write(message + Environment.NewLine, color);


        public virtual void Fatal(string message) => Log(LogLevel.Fatal, message);

        public virtual void Severe(string message) => Log(LogLevel.Severe, message);

        public virtual void Warning(string message) => Log(LogLevel.Warning, message);

        public virtual void Log(string message) => Log(LogLevel.Log, message);

        public virtual void Info(string message) => Log(LogLevel.Info, message);

        public virtual void Fine(string message) => Log(LogLevel.Fine, message);

        public virtual void Finest(string message) => Log(LogLevel.Finest, message);

        public virtual void Debug(string message) => Log(LogLevel.Debug, message);


        public string LoggerName { get; }

        public bool ToSystemDiagnostics { get; }
        public bool ToConsole { get; }


        public LogLevel LogLevel { get; set; }


        public event LogDelegate LogCreated;


        #region Static

        public static Logger Get<T>(bool toSystemDiagnostics = true, bool toConsole = true) => Get(typeof(T).FullName, toSystemDiagnostics, toConsole);

        public static Logger Get(string loggerName, bool toSystemDiagnostics = true, bool toConsole = true)
        {
            if (!_loggers.ContainsKey(loggerName))
                _loggers.TryAdd(loggerName, new Logger(loggerName, toSystemDiagnostics, toConsole));

            if (!_loggers.TryGetValue(loggerName, out Logger logger))
                Get(loggerName);

            logger.Log($"Logger created under name {logger.LoggerName}.");

            return logger;
        }


        private class LoggerNameEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => x.Equals(y, StringComparison.CurrentCultureIgnoreCase);

            public int GetHashCode(string obj) => obj.GetHashCode();
        }

        #endregion
    }
}