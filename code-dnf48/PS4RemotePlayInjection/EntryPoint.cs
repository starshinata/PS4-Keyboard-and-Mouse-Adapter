using CommandLine;
using Pizza.Common;
using Pizza.PS4RemotePlayDll.Stuff;
using PS4RemotePlayInterceptor;
using Serilog;
using System;

namespace PS4RemotePlayInjection
{
    public class EntryPoint
    {

        public class Options
        {

            [Option("emulationMode", Required = true, HelpText = "TODO eg Set output to verbose messages.")]
            public int EmulationMode { get; set; }


            [Option("processName", Required = true, HelpText = "TODO eg Set output to verbose messages.")]
            public string ProcessName { get; set; }
        }

        static void Main(string[] args)
        {
            LogLog.Setup();
            
            Log.Information("PS4 remoteplay injection - EntryPoint.cs");
            Log.Error("PS4 remoteplay injection - EntryPoint.cs - Log Test");

            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                if (o.EmulationMode <= 0)
                {
                    Console.WriteLine("emulationMode must be greater than 0");
                    System.Environment.Exit(1);
                }

                try
                {
                    Log.Information("info");
                    //we have everything
                    Injector.Inject(o.EmulationMode, o.ProcessName);
                }
                catch (Exception ex)
                {
                    string error = string.Format("fail: {0}", ex.Message);
                    ExceptionLogger.LogException(error, ex);
                    throw new InterceptorException(error, ex);
                }
            });
        }
    }
}
