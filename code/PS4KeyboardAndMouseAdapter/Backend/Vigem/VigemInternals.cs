using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Vigem
{

    //thanks https://github.com/Ryochan7/DS4Windows/blob/2f1d8d353253d7f2ab7edadd70888658f1dacd7c/DS4Windows/DS4Control/ControlService.cs

    public class VigemInternals
    {

        public static ushort VENDOR_ID = (ushort)HidConstants.VENDOR_ID_SONY;
        public static ushort PRODUCT_ID = (ushort)HidConstants.PRODUCT_ID_PS4_CONTROLLER_A;

        private ViGEmClient vigemClient;

        private IDualShock4Controller controller;

        private Stopwatch RequestsPerSecondTimer = new Stopwatch();
        private int RequestsPerSecondCounter = 0;
        private Stopwatch SleepTimer = new Stopwatch();

        private bool logSleepDuration = false;
        private bool logRequestPerSecond = false;

        private void Listen()
        {
            Log.Information("VigemInternals.Listen");
            SleepTimer.Start();
            RequestsPerSecondTimer.Start();

            GamepadProcessor gp = new GamepadProcessor();
            
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
                        ExceptionLogger.LogException("VigemInternals.Listen L57", ex);
                    }

                    Sleep();
                }
            });

        }

        public void Sleep()
        {
            if (logSleepDuration)
            {
                SleepTimer.Restart();
            }

            int sleepDuration = 0;
            if (!ApplicationSettings.GetInstance().GamepadUpdaterNoSleep)
            {
                sleepDuration = 1000 / UserSettings.GetInstance().MousePollingRate;
            }

            // 2021.12.22 pancakeslp
            // on Windows OS you can specify a sleep of 5ms, but it will probably 15ms
            // as 15ms is roughly the minimum amount of time Thread.Sleep can sleep for
            // it seems Unix will honour sleep(1) to sleep for 1 ms
            //
            // for more reading see 
            // https://stackoverflow.com/questions/19066900/thread-sleep1-takes-longer-than-1ms
            // https://stackoverflow.com/questions/8860803/pause-a-thread-for-less-than-one-millisecond
            // https://stackoverflow.com/questions/85122/how-to-make-thread-sleep-less-than-a-millisecond-on-windows/11456112#11456112

            System.Threading.Thread.Sleep(sleepDuration);

            if (logSleepDuration)
            {
                long actualSleepDuration = SleepTimer.ElapsedMilliseconds;
                Log.Information("VigemInternals.Sleep: requested sleepDuration {0}", sleepDuration);
                Log.Information("VigemInternals.Sleep: actualSleepDuration {0}", actualSleepDuration);
            }

            if (logRequestPerSecond)
            {
                RequestsPerSecondCounter++;

                if (RequestsPerSecondTimer.ElapsedMilliseconds >= 1000)
                {
                    Log.Information("VigemInternals.Sleep: requested sleepDuration {0}", sleepDuration);
                    Log.Information("VigemInternals.Sleep:  RequestsPerSecondCounter={0}", RequestsPerSecondCounter);
                    RequestsPerSecondTimer.Restart();
                    RequestsPerSecondCounter = 0;
                }
            }
        }

        public void StartAndListen()
        {
            Log.Information("VigemInternals.StartAndListen");
            Start_ViGEm();
            Start_controller();
            Listen();
        }

        public void Start()
        {
            Log.Information("VigemInternals.Start");
            Start_ViGEm();
            Start_controller();
        }

        private void Start_controller()
        {
            Log.Information("VigemInternals.Start_controller");
            controller = vigemClient.CreateDualShock4Controller(VENDOR_ID, PRODUCT_ID);
            controller.Connect();
        }

        private void Start_ViGEm()
        {
            Log.Information("VigemInternals.Start_ViGEm");
            vigemClient = new ViGEmClient();
        }

        public void Stop()
        {
            Log.Information("VigemInternals.Stop");
            Stop_controller();
            Stop_ViGEm();
        }

        private void Stop_controller()
        {
            controller.Disconnect();
        }

        private void Stop_ViGEm()
        {
            if (vigemClient != null)
            {
                vigemClient.Dispose();
                vigemClient = null;
            }
        }

    }
}
