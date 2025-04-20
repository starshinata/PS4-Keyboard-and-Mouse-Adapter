using Serilog;
using System;

namespace Pizza.PS4RemotePlayDll.Stuff
{
    public class LogLog
    {
        public static void Setup()
        {
            var logfile = "E:\\donk.log";

            Console.WriteLine($"LogLog.cs: see logfile {logfile}");
            Console.WriteLine("");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logfile)
                .CreateLogger();
        }
    }
}
