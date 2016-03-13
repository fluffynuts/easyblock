using System;
using System.Diagnostics;
using PeanutButter.ServiceShell;

namespace EasyBlock.Core
{
    public interface ISimpleLoggerFacade: ISimpleLogger
    {
        void SetLogger(ISimpleLogger logger);
    }

    public class SimpleLoggerFacade: ISimpleLoggerFacade
    {
        public Action<string> Fallback { get; set; } = Console.WriteLine;
        private ISimpleLogger _simpleLogger;

        public void SetLogger(ISimpleLogger logger)
        {
            _simpleLogger = logger;
        }

        public void LogDebug(string message)
        {
            TryLog(message, 
                    s => _simpleLogger.LogDebug(s), 
                    s => Fallback($"DEBUG: {s}"));
        }

        public void LogInfo(string message)
        {
            TryLog(message, 
                    s => _simpleLogger.LogInfo(s), 
                    s => Fallback($"INFO: {s}"));
        }

        public void LogWarning(string message)
        {
            TryLog(message, 
                    s => _simpleLogger.LogWarning(s), 
                    s => Fallback($"WARNING: {s}"));
        }

        public void LogFatal(string message)
        {
            TryLog(message, 
                    s => _simpleLogger.LogFatal(s), 
                    s => Fallback($"FATAL: {s}"));
        }

        private void TryLog(string message, Action<string> primary, Action<string> fallback)
        {
            try
            {
                if (_simpleLogger == null)
                    fallback(message);
                else
                    primary(message);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Unable to log \"{message}\"\n{e.Message}");
            }
        }
    }
}
