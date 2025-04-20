﻿using EasyHook;
using Pizza.Common;
using Pizza.Common.Gamepad;
using Pizza.KeyboardAndMouseAdapter.Backend.Injection;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;

namespace Pizza.RemotePlayInjector
{
    public delegate void InterceptionDelegate(ref DualShockState state);

    public class Injector
    {
        //TODO why is everything static

        //private static ThreadRpcUpdateListener ThreadRpcUpdateListener;
        //private static ThreadRpcUpdateListener threadRpcUpdateListener;
        /*
            threadRpcUpdateListener = new ThreadRpcUpdateListener();
            Thread thread = new Thread(threadRpcUpdateListener.DoWork);
            thread.Start();
            */


        // Emulation
        public static int EmulationMode = -1;

        // Delegate
        public static InterceptionDelegate Callback { get; set; }


        public static void Inject(int emulationMode, Process remotePlayPlayProccess)
        {
            Log.Information("Injector.Inject IS NOT CALLING ThreadRpcUpdateListener");



            if (remotePlayPlayProccess == null)
            {
                throw new Exception("Injector.Inject remotePlayPlayProccess is null");
            }

            Log.Logger.Information("Injector.Inject {emulationMode:{0}||{1}, processName:{2}",
                    emulationMode,
                    EmulationConstants.ToString(emulationMode),
                    remotePlayPlayProccess.ProcessName);

            EmulationMode = emulationMode;

            // Full path to our dll file
            string injectionLibrary = getDllPath();

            try
            {


                Log.Debug("Injector.Inject RemoteHooking.Inject start");

                //TODO needed?
                var _channelName = "PS4KMA--" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                RemoteHooking.Inject(
                     remotePlayPlayProccess.Id, // ID of process to inject into
                     InjectionOptions.Default,
                     // if not using GAC allow assembly without strong name
                     injectionLibrary, // 32-bit version (the same because AnyCPU)
                     injectionLibrary, // 64-bit version (the same because AnyCPU)
                     null);
                Log.Debug("Injector.Inject RemoteHooking.Inject done");
            }
            catch (Exception ex)
            {
                // 2025.01.05 pancakes
                // this will 99% be a we could not find remoting error
                string error = string.Format("Failed to inject to target: {0}", ex.Message);
                ExceptionLogger.LogException(error, ex);
                throw new TempInterceptorException(error, ex);
            }
        }

        private static string getDllPath()
        {
            string dllToInject = "RemotePlayInjected.dll";
            var applicationPath = PathUtil.GetApplicationPath();
            Log.Information("applicationPath: " + applicationPath);

            if (applicationPath.Contains("E:\\workspace\\PS4-Keyboard-and-Mouse-Adapter\\"))
            {
                // assume path is
                //   E:\workspace\PS4-Keyboard-and-Mouse-Adapter\{{SOMETHING}}
                //   \code\PS4KeyboardAndMouseAdapter\bin\Release\net6.0-windows

                // would have used Path.Combine(), but it doesnt like the below string
                var dllPath = applicationPath +
                    "\\..\\..\\..\\..\\" +
                    "RemotePlayInjected\\bin\\Release\\"
                    + dllToInject;

                Log.Information("checking dllPath: " + dllPath);
                if (File.Exists(dllPath))
                {
                    Log.Information("... exists");
                    return dllPath;
                }
                else
                {
                    Log.Information("... does not exist");
                }
            }

            //throw new Exception("I LIKE CHEESE");

            string[] files = Directory.GetFiles(applicationPath);
            foreach (string file in files)
            {
                Log.Information("local file " + file);
            }


            string injectionLibrary = Path.Combine(applicationPath, dllToInject);
            // TODO file exists check

            Log.Information("Injector.Inject() injectionLibrary " + injectionLibrary);
            return injectionLibrary;
        }

    }
}
