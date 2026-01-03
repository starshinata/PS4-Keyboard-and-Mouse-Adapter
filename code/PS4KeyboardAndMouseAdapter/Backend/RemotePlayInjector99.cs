using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.GamepadProcessing;
using Pizza.KeyboardAndMouseAdapter.Backend.IpcCommunication;
using Pizza.RemotePlayInjector;
using Serilog;
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

// TODO do detection
// TODO rename project
// (a) RemotePlayInjectior to
// (b) RemotePlayInjector
                string exe = "RemotePlayInjectior.exe";

                //TODO processName -> PID
                string processArguments = " --emulationMode " + ApplicationSettings.GetInstance().EmulationMode +
                    " --processName " + remotePlayProcess.ProcessName;

Log.Error("FileName:" + exe);
Log.Error("Arguments:" + processArguments);

                Process injectorProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exe,
                        Arguments = processArguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                 injectorProcess.Start();

       //* Read the output (or the error)
       string output = injectorProcess.StandardOutput.ReadToEnd();
       Log.Information(output);
       string err = injectorProcess.StandardError.ReadToEnd();
Log.Error(err);

                injectorProcess.WaitForExit();

            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("RemotePlayInjector.Inject error", e);
                //TODO shouldnt this be fatal?
                throw e;
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
 Log.Error("RemotePlayInjector99.OpenRemotePlayAndInject line 64");


            try
            {
                RemotePlayStarter.OpenRemotePlay();
            }
            catch (Exception e)
            {

                ExceptionLogger.LogException("RemotePlayInjector.OpenRemotePlayAndInject() fatal error 69", e);

                System.Windows.MessageBox.Show(
                    e.Message,
                    "Fatal error",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                throw e;
            }

Log.Error("RemotePlayInjector99.OpenRemotePlayAndInject line 85");

            try
            {
                StartIpc();
            }
            catch (Exception e)
            {

                ExceptionLogger.LogException("RemotePlayInjector.OpenRemotePlayAndInject() fatal error 93", e);

                System.Windows.MessageBox.Show(
                    e.Message,
                    "Fatal error",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                throw e;
            }


Log.Error("RemotePlayInjector99.OpenRemotePlayAndInject line 104");
            try
            {
                Log.Error("RemotePlayInjector99.OpenRemotePlayAndInject line 109");
                      string exe = @"E:\workspace\PS4-Keyboard-and-Mouse-Adapter\master.move-to-dotnet-8.2025\code\RemotePlayInjector\bin\Release\RemotePlayInjectior.exe";
                Log.Error("Inject 33");
                                //TODO correct log level
                 Log.Error("exe:" + exe);

                Inject(gamepadProcessor);
                Log.Error("RemotePlayInjector99.OpenRemotePlayAndInject line 111");
            }
            catch (Exception e)
            {

                ExceptionLogger.LogException("RemotePlayInjector.OpenRemotePlayAndInject() fatal error 116", e);

                System.Windows.MessageBox.Show(
                    e.Message,
                    "Fatal error",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                throw e;
            }
        }

    }
}
