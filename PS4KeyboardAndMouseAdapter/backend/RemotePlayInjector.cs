
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;

namespace PS4KeyboardAndMouseAdapter
{

    public class RemotePlayInjector
    {

        private GamepadProcessor gamepadProcessor;

        public RemotePlayInjector(GamepadProcessor gamepadProcessor)
        {
            this.gamepadProcessor = gamepadProcessor;

            Injector.FindProcess(RemotePlayConstants.TARGET_PROCESS_NAME)?.Kill();
        }

        public void Inject()
        {
            try
            {
                Thread.Sleep(3100);
                int remotePlayProcessId = Injector.Inject(RemotePlayConstants.TARGET_PROCESS_NAME, RemotePlayConstants.INJECT_DLL_NAME);
                Process RemotePlayProcess = Process.GetProcessById(remotePlayProcessId);
                RemotePlayProcess.EnableRaisingEvents = true;
                RemotePlayProcess.Exited += (sender, args) => { Utility.ShowCursor(true); };

                InstanceSettings.GetInstance().RemotePlayProcess = RemotePlayProcess;

                Injector.Callback += gamepadProcessor.OnReceiveData;
            }
            catch (Exception e)
            {
                Log.Logger.Error("MainViewModel Inject error: " + e.Message);
                Log.Logger.Error(e.StackTrace);
            }
        }

        private bool OpenRemotePlay()
        {
            string exeLocation = ApplicationSettings.GetInstance().RemotePlayPath;

            if (File.Exists(exeLocation))
            {
                Process.Start(exeLocation);
                Log.Information("RemotePlay start requested-57");
                return true;
            }

            try
            {
                //TODO: hardcoded currently, so it doesn't work when OS is set to non-default system language.
                string shortcutPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\PS Remote Play.lnk";
                IWshRuntimeLibrary.IWshShell wsh = new IWshRuntimeLibrary.WshShellClass();
                IWshRuntimeLibrary.IWshShortcut sc = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
                shortcutPath = sc.TargetPath;

                if (string.IsNullOrEmpty(shortcutPath))
                    return false;

                Process.Start(shortcutPath);
                Log.Information("RemotePlay start requested-73");
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error("Cannot open RemotePlay: " + e.Message);
            }

            return false;
        }

        public void OpenRemotePlayAndInject()
        {
            try
            {
                EventWaitHandle waitHandle = new ManualResetEvent(initialState: false);

                bool success = OpenRemotePlay();
                if (success)
                {
                    Inject();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error("MainViewModel OpenRemotePlayAndInject() fatal error" + e.Message);
                Log.Logger.Error("" + e.GetType());
                Log.Logger.Error(e.StackTrace);

                System.Windows.MessageBox.Show(
                    "Fatal error, program closing",
                    "fatal",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                throw e;
            }
        }



    }
}
