using Serilog;
using System;

namespace Pizza.Common
{
    public class ExceptionLogger
    {
        private static readonly ILogger StaticLogger = Log.ForContext(typeof(ExceptionLogger));

        public static void LogException(string message, Exception e)
        {
            StaticLogger.Error(message + e.Message);
            StaticLogger.Error(e.GetType().ToString());
            StaticLogger.Error(e.StackTrace);
        }
    }
}
