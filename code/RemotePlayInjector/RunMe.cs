using Pizza.Common;
using Serilog;
using System;
using System.Threading.Tasks;

namespace RemotePlayInjector
{
    public class RunMe
    {
        static void Main(string[] args)
        {

            LogManager logManager = new LogManager();
            logManager.SetupLogger();

            Log.Information("MAIN");
            Log.Information("" + args.Length);
            Console.WriteLine("MAIN");
            Console.WriteLine(args.Length);
        }
    }
}
