using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using PS4KeyboardAndMouseAdapter;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Pizza.backend.vigem
{
    using PVIGEM_TARGET = IntPtr;

    //thanks https://github.com/Ryochan7/DS4Windows/blob/2f1d8d353253d7f2ab7edadd70888658f1dacd7c/DS4Windows/DS4Control/ControlService.cs

    public class VigemManager
    {

        public static ushort VENDOR_ID = (ushort)HidConstants.VENDOR_ID_SONY;
        public static ushort PRODUCT_ID = (ushort)HidConstants.PRODUCT_ID_PS4_CONTROLLER_A;

        private ViGEmClient vigemClient;
        private IDualShock4Controller controller;
        private bool connected;

        protected PVIGEM_TARGET NativeHandle { get; set; }
        public object GamepadConverer { get; private set; }

        public void listen()
        {
            Log.Information("VigemManager.listen");


            GamepadProcessor gp = new GamepadProcessor();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        PS4RemotePlayInterceptor.DualShockState x = gp.GetState();
                        if (x != null)
                        {
                            GamepadConverter.ConvertandSendReport(controller, x);
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogException("VigemManager.listen L49", ex);
                    }

                    int MillisecondsPerInput = 1000 / UserSettings.GetInstance().MousePollingRate;
                    System.Threading.Thread.Sleep(MillisecondsPerInput);
                }
            });

        }

        public void start()
        {
            Log.Information("VigemManager.start");
            start_ViGEm();
            start_controller();
            listen();
        }

        private void start_controller()
        {
            Log.Information("VigemManager.start_controller");
            controller = vigemClient.CreateDualShock4Controller(VENDOR_ID, PRODUCT_ID);
            controller.Connect();
            connected = true;
        }

        private void start_ViGEm()
        {
            Log.Information("VigemManager.start_ViGEm");
            vigemClient = new ViGEmClient();
            //System.Threading.Thread.Sleep(1000);        }
        }

        public void stop()
        {
            stop_controller();
            stop_ViGEm();
        }

        private void stop_controller()
        {
            controller.Disconnect();
        }

        private void stop_ViGEm()
        {
            if (vigemClient != null)
            {
                vigemClient.Dispose();
                vigemClient = null;
            }
        }

    }
}
