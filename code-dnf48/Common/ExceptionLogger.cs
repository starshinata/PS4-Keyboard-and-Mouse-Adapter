﻿using Serilog;
using System;

namespace Pizza.Common
{
    public class ExceptionLogger
    {
        public static void LogException(string message, Exception e)
        {
            Log.Error(message + e.Message);
            Log.Error(e.GetType().ToString());
            Log.Error(e.StackTrace);
        }
    }
}
