using Pizza.Common;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInterceptor;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter
{

    public class RemotePlayStarter
    {

        public RemotePlayStarter()
        {
            Injector.FindProcess(RemotePlayConstants.TARGET_PROCESS_NAME)?.Kill();
        }

        public void OpenRemotePlay()
        {
            try
            {
                EventWaitHandle waitHandle = new ManualResetEvent(initialState: false);

                OpenRemotePlayInternal();
                RefreshRemotePlayProcess();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("RemotePlayStarter.OpenRemotePlay() fatal error", e);

                System.Windows.MessageBox.Show(
                    "Fatal error, program closing",
                    "fatal",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                throw e;
            }
        }

        protected bool OpenRemotePlayInternal()
        {
            string exeLocation = ApplicationSettings.GetInstance().RemotePlayPath;

            if (File.Exists(exeLocation))
            {
                Process.Start(exeLocation);
                Log.Information("RemotePlayStarter.OpenRemotePlayInternal start requested-54");
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
                Log.Information("RemotePlayStarter.OpenRemotePlay start requested-73");
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error("Cannot open RemotePlay: " + e.Message);
            }

            return false;
        }

        protected void RefreshRemotePlayProcess()
        {
            Process RemotePlayProcess = Injector.FindProcess(RemotePlayConstants.TARGET_PROCESS_NAME);
            InstanceSettings.GetInstance().SetRemotePlayProcess(RemotePlayProcess);
        }

    }
}
