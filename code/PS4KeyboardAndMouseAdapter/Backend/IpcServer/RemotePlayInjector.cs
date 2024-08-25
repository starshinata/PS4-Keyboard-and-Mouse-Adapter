using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using PS4RemotePlayInjection;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Remote
{

    public class RemotePlayInjector : RemotePlayStarter
    {

        public RemotePlayInjector()
        {
            try
            {
                Injector.FindProcess(RemotePlayConstants.TARGET_PROCESS_NAME)?.Kill();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("RemotePlayInjector.init error", e);
            }
        }

        private void Inject(GamepadProcessor gamepadProcessor)
        {
            try
            {
                Thread.Sleep(3100);
                int remotePlayProcessId = Injector.Inject(
                    ApplicationSettings.GetInstance().EmulationMode,
                    RemotePlayConstants.TARGET_PROCESS_NAME,
                    RemotePlayConstants.INJECT_DLL_NAME);

                Log.Logger.Information("RemotePlayInjector.Inject remotePlayProcessId " + remotePlayProcessId);
                Process RemotePlayProcess = Process.GetProcessById(remotePlayProcessId);
                RemotePlayProcess.EnableRaisingEvents = true;
                RemotePlayProcess.Exited += (sender, args) => { CursorUtility.ShowCursor(true); };

                RefreshRemotePlayProcess();

                Injector.Callback += gamepadProcessor.OnReceiveData;
            }
            catch (DllNotFoundException ex)
            {
                //Deferr to OpenRemotePlayAndInject()
                throw ex;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("RemotePlayInjector.Inject error", e);
            }
        }
        private void StartIpc()
        {
            var x = new ServerThing();
            x.Moooain();
        }

        public void OpenRemotePlayAndInject(GamepadProcessor gamepadProcessor)
        {
            try
            {
                EventWaitHandle waitHandle = new ManualResetEvent(initialState: false);

                bool success = OpenRemotePlayInternal();
                if (success)
                {
                    StartIpc();
                    Inject(gamepadProcessor);
                }
            }
            catch (DllNotFoundException ex)
            {
                ExceptionLogger.LogException("DllNotFoundException, this looks fatal", ex);

                System.Windows.MessageBox.Show("Got a DllNotFoundException and that is 99% bad, please report this to the developer",
                    "PS4KMA DllNotFoundException",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                Log.Logger.Fatal("Application Exit RemotePlayInjector.Inject() Ref81");
                throw ex;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("RemotePlayInjector.OpenRemotePlayAndInject() fatal error", e);

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
