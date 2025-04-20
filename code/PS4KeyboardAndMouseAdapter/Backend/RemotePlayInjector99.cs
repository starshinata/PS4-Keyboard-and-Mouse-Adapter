using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.GamepadProcessing;
using Pizza.KeyboardAndMouseAdapter.Backend.IpcCommunication;
using Pizza.RemotePlayInjector;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    // we cant just call it 'RemotePlayInjector'
    // as that is a project name, and c# gets confused
    public class RemotePlayInjector99
    {

        private void Inject(GamepadProcessor gamepadProcessor)
        {
            try
            {
                var remotePlayProcess = InstanceSettings.GetInstance().GetRemotePlayProcess();

                string exe = "E:\\workspace\\PS4-Keyboard-and-Mouse-Adapter\\master.move-to-dotnet-8\\code-dnf48\\PS4RemotePlayInjection\\bin\\Debug\\PS4RemotePlayInjection.exe";

                //TODO processName -> PID
                string processArguments = " --emulationMode " + ApplicationSettings.GetInstance().EmulationMode +
                    " --processName " + remotePlayProcess.ProcessName;

                Process injectorProcess = Process.Start(exe, processArguments);

                remotePlayProcess.EnableRaisingEvents = true;
                remotePlayProcess.Exited += (sender, args) => { CursorUtility.ShowCursor(true); };

                Injector.Callback += gamepadProcessor.OnReceiveData;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("RemotePlayInjector.Inject error", e);
                //TODO shouldnt this be fatal?
            }
        }

        private void StartIpc()
        {
            //TODO need proper name
            var x = new ServerThing();
            x.Moooain();
        }

        public void OpenRemotePlayAndInject(GamepadProcessor gamepadProcessor)
        {
            try
            {
                RemotePlayStarter.OpenRemotePlay();
                StartIpc();
                Inject(gamepadProcessor);

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
