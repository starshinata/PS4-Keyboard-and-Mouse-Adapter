using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;
using System;
using System.Windows;
using System.Windows.Forms;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Vigem
{
    public class VigemManager
    {

        //for simplicity would like this to be static, however cant mock it and have it static
        public virtual bool IsVigemDriverInstalled()
        {
            Log.Information("VigemManager.IsVigemDriverInstalled");

            try
            {
                VigemInternals vig = new VigemInternals();
                vig.start();
                System.Threading.Thread.Sleep(1000);
                vig.stop();
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("VigemManager.IsVigemDriverInstalled a", e);
                return false;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("VigemManager.IsVigemDriverInstalled b", e);
                return false;
            }

            // didnt error, sooo hopefully ok
            return true;
        }

        public static void Start()
        {
            Log.Information("VigemManager.Start");

            try
            {
                //TODO test that if we dont have vigem bus driver installed does the error still get propegated 

                System.Threading.Thread.Sleep(1000);
                VigemInternals vig = new VigemInternals();
                InstanceSettings.GetInstance().SetVigemInternals(vig);
                vig.startAndListen();
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("VigemManager.Start", e);

                System.Windows.MessageBox.Show("Vigembus not installed!",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("VigemManager.Start", e);

                System.Windows.MessageBox.Show("whoops! tell the developer he failed",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
        }


        public static void Stop(InstanceSettings instanceSettings)
        {
            Log.Information("VigemManager.Stop");

            try
            {
                if (instanceSettings != null)
                {
                    VigemInternals vig = instanceSettings.GetVigemInternals();
                    if (vig != null)
                    {
                        vig.stop();
                    }
                }
            }
            // You might be expecting the catches of this method to mirror, the Start() method
            // logically we are calling VigManager.Stop() is to end the application
            // so there it no point to call application exit
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("VigemManager.Stop", e);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("VigemManager.Stop", e);
            }
        }

    }
}
