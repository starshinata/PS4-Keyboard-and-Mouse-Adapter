using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter
{

    public class RemotePlayInjector : RemotePlayStarter
    {

        private GamepadProcessor gamepadProcessor;

        public RemotePlayInjector(GamepadProcessor gamepadProcessor)
        {
            this.gamepadProcessor = gamepadProcessor;

            Injector.FindProcess(RemotePlayConstants.TARGET_PROCESS_NAME)?.Kill();
        }

        private void Inject()
        {
            try
            {
                Thread.Sleep(3100);
                int remotePlayProcessId = Injector.Inject(RemotePlayConstants.TARGET_PROCESS_NAME, RemotePlayConstants.INJECT_DLL_NAME);
                Log.Logger.Information("RemotePlayInjector.Inject remotePlayProcessId " + remotePlayProcessId);
                Process RemotePlayProcess = Process.GetProcessById(remotePlayProcessId);
                RemotePlayProcess.EnableRaisingEvents = true;
                RemotePlayProcess.Exited += (sender, args) => { Utility.ShowCursor(true); };

                RefreshRemotePlayProcess();

                Injector.Callback += gamepadProcessor.OnReceiveData;
            }
            catch (Exception e)
            {
                Log.Logger.Error("MainViewModel Inject error: " + e.Message);
                Log.Logger.Error(e.StackTrace);
            }
        }

        public void OpenRemotePlayAndInject()
        {
            try
            {
                EventWaitHandle waitHandle = new ManualResetEvent(initialState: false);

                bool success = OpenRemotePlayInternal();
                if (success)
                {
                    Inject();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error("RemotePlayInjector.OpenRemotePlayAndInject() fatal error" + e.Message);
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
