using EasyHook;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Principal;

namespace PS4RemotePlayInterceptor
{
    public delegate void InterceptionDelegate(ref DualShockState state);

    public enum InjectionMode
    {
        Auto,
        Compatibility
    }

    public class Injector
    {
        // EasyHook
        private static string _channelName = null;
        private static string _channelName2 = null;
        private static IpcServerChannel _ipcServer;
        private static IpcServerChannel _ipcServer2;
        private static bool _noGAC = false;

        public static DateTime LastPingTime { get; set; }

        // Injection
        public static InjectionMode InjectionMode = InjectionMode.Auto;
        // Emulation
        public static bool EmulateController = true;

        // Delegate
        public static InterceptionDelegate Callback { get; set; }

        public static int Inject(string processName, string dllToInject)
        {
            Log.Logger.Information("Injector.Inject processName " + processName);

            // Find the process
            Process process = FindProcess(processName);
            if (process == null)
            {
                string error = string.Format("{0} not found in list of processes", processName);
                Log.Error(error);
                throw new InterceptorException(error);

            }

            // Full path to our dll file
            string injectionLibrary = Path.Combine(Path.GetDirectoryName(typeof(InjectionInterface).Assembly.Location), dllToInject);
            Log.Information("Injector.Inject() injectionLibrary " + injectionLibrary);


            bool shouldInject = false;
            try
            {

                //Original from Komefai
                //
                //if (InjectionMode == InjectionMode.Auto)
                //{
                //    if (_ipcServer == null)
                //    {
                //        // Setup remote hooking
                //        _channelName = DateTime.Now.ToString();
                //        _ipcServer = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName,
                //            WellKnownObjectMode.Singleton,
                //            WellKnownSidType.WorldSid);
                //
                //        shouldInject = true;
                //    }
                //}
                //else if (InjectionMode == InjectionMode.Compatibility)
                //{
                //    // Setup remote hooking
                //    _channelName = null;
                //    _ipcServer = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName, WellKnownObjectMode.Singleton);
                //    shouldInject = true;
                //}




                if (_ipcServer == null)
                {
                    Log.Debug("Injector.Inject making ipcServer1");

                    // Setup remote hooking
                    _channelName = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
                    _ipcServer = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName, WellKnownObjectMode.Singleton, WellKnownSidType.WorldSid);
                    Log.Debug("Injector.Inject _ipcServer1 made");
                }

                if (_ipcServer2 == null)
                {
                    Log.Debug("Injector.Inject making ipcServer2");
                    _channelName2 = "dotnethooks";
                    _ipcServer2 = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName2, WellKnownObjectMode.Singleton, WellKnownSidType.WorldSid);
                    shouldInject = true;
                    Log.Debug("Injector.Inject _ipcServer2 made");
                }


            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to setup IPC server: {0}", ex.Message);
                Log.Error(error);
                Log.Error(ex.StackTrace);
                throw new InterceptorException(error, ex);
            }

            try
            {
                Log.Information("Injector.Inject() shouldInject " + shouldInject);
                // Inject dll into the process
                if (shouldInject)
                {
                    Log.Debug("Injector.Inject RemoteHooking.Inject start");
                    RemoteHooking.Inject(
                        process.Id, // ID of process to inject into
                        (_noGAC ? InjectionOptions.DoNotRequireStrongName : InjectionOptions.Default),
                        // if not using GAC allow assembly without strong name
                        injectionLibrary, // 32-bit version (the same because AnyCPU)
                        injectionLibrary, // 64-bit version (the same because AnyCPU)
                        _channelName
                    );
                    Log.Debug("Injector.Inject RemoteHooking.Inject done");
                }

                // Success
                return process.Id;
            }
            catch (Exception ex)
            {
                string error = string.Format("Failed to inject to target: {0}", ex.Message);
                Log.Error(error);
                Log.Error(ex.StackTrace);
                throw new InterceptorException(error, ex);
            }
        }

        public static void StopInjection()
        {
            if (_ipcServer != null)
            {
                _ipcServer.StopListening(null);
                _ipcServer = null;
            }

            if (_ipcServer2 != null)
            {
                _ipcServer2.StopListening(null);
                _ipcServer2 = null;
            }

        }

        public static Process FindProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
                return process;

            return null;
        }
    }
}
