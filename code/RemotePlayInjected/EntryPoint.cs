using Pizza.Common;
using RemotePlayInjected.Injected;
using Serilog;
using System;

namespace RemotePlayInjected
{
    /// <summary>
    /// EasyHook will look for a class implementing <see cref="EasyHook.IEntryPoint"/> during injection. This
    /// becomes the entry point within the target process after injection is complete.
    /// </summary>
    public class EntryPoint : EasyHook.IEntryPoint
    {

       public EntryPoint(EasyHook.RemoteHooking.IContext context)
        {
            LogManager logManager = new LogManager();
            logManager.SetupLogger();

            Log.Information("HI from `code/RemotePlayInjected/EntryPoint.cs' constructor");
        }

        /// <summary>
        /// The main entry point for our logic once injected within the target process.
        /// This is where the hooks will be created, and a loop will be entered until host process exits.
        /// EasyHook requires a matching Run method for the constructor
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        public void Run(EasyHook.RemoteHooking.IContext context)
        {
            try
            {
                Log.Information("EntryPoint.Run ref30");
                ThreadRpcUpdateListener listener = new ThreadRpcUpdateListener();
                Log.Information("EntryPoint.Run ref32");
                listener.DoWork();
                Log.Information("EntryPoint.Run ref34");
            }
            catch (Exception e) {
                ExceptionLogger.LogException("EntryPoint.Run error ref 40", e);
            }
        }

    }
}
