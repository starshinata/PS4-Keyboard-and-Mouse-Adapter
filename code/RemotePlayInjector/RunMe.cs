using CommandLine;
using Pizza.Common;
using Pizza.RemotePlayInjector;
using Serilog;
using System;

namespace RemotePlayInjector
{



    public class RunMe
    {
        static void Main(string[] args)
        {
            LogManager logManager = new LogManager();
            logManager.SetupLogger();

            Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(o =>
               {
                   Log.Information("detected args");
                   Log.Information("EmulationMode " + o.EmulationMode);
                   Log.Information("ProcessId " + o.ProcessId);
                   if (o.EmulationMode <= 0 || o.EmulationMode > 3)
                   {
                       Log.Information("EmulationMode out of range");
                       System.Environment.Exit(1);
                   }
                   else if (o.ProcessId <= 0)
                   {
                       Log.Information("ProcessId must be postive");
                       Log.Information("Quick Start Example! App is in Verbose mode!");
                       System.Environment.Exit(1);
                   }
                   else
                   {

                       Injector.Inject(o.EmulationMode, o.ProcessId);

                       System.Environment.Exit(0);
                   }
               });

        }
    }
}
