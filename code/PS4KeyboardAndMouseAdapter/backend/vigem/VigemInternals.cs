using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Pizza.Common;
using PS4KeyboardAndMouseAdapter;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pizza.backend.vigem
{

    //thanks https://github.com/Ryochan7/DS4Windows/blob/2f1d8d353253d7f2ab7edadd70888658f1dacd7c/DS4Windows/DS4Control/ControlService.cs

    public class VigemInternals
    {

        public static ushort VENDOR_ID = (ushort)HidConstants.VENDOR_ID_SONY;
        public static ushort PRODUCT_ID = (ushort)HidConstants.PRODUCT_ID_PS4_CONTROLLER_A;

        private ViGEmClient vigemClient;

        private IDualShock4Controller controller;

        private void listen()
        {
            Log.Information("VigemManager.listen");


            GamepadProcessor gp = new GamepadProcessor();

            int RequestsPerSecondCounter = 0;

            Stopwatch RequestsPerSecondTimer = new Stopwatch();
            RequestsPerSecondTimer.Start();


            Stopwatch SleepTimer = new Stopwatch();
            SleepTimer.Start();

            Task.Factory.StartNew(() =>
            {
                controller.ResetReport();

                while (true)
                {
                    try
                    {
                        PS4RemotePlayInterceptor.DualShockState x = gp.GetState();
                        if (x != null)
                        {
                            //Log.Information("ds" + gp.DualShockStateToString(ref x));
                            GamepadConverter.ConvertandSendReport(controller, x);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogException("VigemManager.listen L49", ex);
                    }

                    SleepTimer.Restart();
                    int MillisecondsPerInput = 0; // 1000 / UserSettings.GetInstance().MousePollingRate;
                    sleep(MillisecondsPerInput);
                    long sleepDuration = SleepTimer.ElapsedMilliseconds;

                    //Log.Information("MillisecondsPerInput {0}", MillisecondsPerInput);
                    //Log.Information("sleepDuration {0}", sleepDuration);
                    RequestsPerSecondCounter++;

                    SleepTimer.Restart();

                    if (RequestsPerSecondTimer.ElapsedMilliseconds >= 1000)
                    {
                        //Log.Information("MillisecondsPerInput {0}", MillisecondsPerInput);
                        //Log.Information("VigemManager.listen  RequestsPerSecondCounter={0}", RequestsPerSecondCounter);
                        RequestsPerSecondTimer.Restart();
                        RequestsPerSecondCounter = 0;
                    }
                }
            });

        }

        public void sleep(int sleepDuration)
        {
            // 2021.12.22 pancakeslp
            // 15ms as that seems to be roughly the minimum amount of time Thread.Sleep can sleep for on Windows
            // it seems unix will honour sleep(1) to sleep for 1 ms
            //
            // for more reading see 
            // https://stackoverflow.com/questions/19066900/thread-sleep1-takes-longer-than-1ms
            // https://stackoverflow.com/questions/8860803/pause-a-thread-for-less-than-one-millisecond
            // https://stackoverflow.com/questions/85122/how-to-make-thread-sleep-less-than-a-millisecond-on-windows/11456112#11456112

            int revisedSleepDuration = sleepDuration - 15;
            if (revisedSleepDuration < 0)
            {
                revisedSleepDuration = 0;
            }
            System.Threading.Thread.Sleep(revisedSleepDuration);
        }

        public void startAndListen()
        {
            Log.Information("VigemManager.startAndListen");
            start_ViGEm();
            start_controller();
            listen();
        }

        public void start()
        {
            Log.Information("VigemManager.startAndListen");
            start_ViGEm();
            start_controller();

        }

        private void start_controller()
        {
            Log.Information("VigemManager.start_controller");
            controller = vigemClient.CreateDualShock4Controller(VENDOR_ID, PRODUCT_ID);
            controller.Connect();
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
