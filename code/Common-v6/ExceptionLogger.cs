using Serilog;
using System;

namespace Pizza.Common
{
    public class ExceptionLogger
    {
        public static void LogException(string kmaMessage, Exception e)
        {
            Log.Error("kmaMessage:" + kmaMessage);
            Log.Error("errorMessage: " + e.Message);
            Log.Error("type: " + e.GetType().ToString());
            Log.Error("stackTrace:" +e.StackTrace);
        }
    }
}
