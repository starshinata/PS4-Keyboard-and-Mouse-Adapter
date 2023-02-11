using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;
using System;
using System.ComponentModel;
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
                vig.Start();
                System.Threading.Thread.Sleep(1000);
                vig.Stop();
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("VigemManager.IsVigemDriverInstalled error L28", e);
                return false;
            }
            catch (Win32Exception e)
            {
                Log.Error("message: " + e.Message);
                Log.Error("ErrorCode:" + e.ErrorCode.ToString());
                Log.Error("NativeErrorCode: " + e.NativeErrorCode.ToString());
                Log.Error("Data:" + e.Data);
                Log.Error("HResult :" + e.HResult);
                Log.Error("HelpLink :" + e.HelpLink);
                Log.Error("Source: " + e.Source);
                Log.Error(e.StackTrace);
                Exception b = e.GetBaseException();
                if (b != null)
                {
                    Log.Error("b.Message: " + b.Message);
                }

                ExceptionLogger.LogException("VigemManager.IsVigemDriverInstalled error L33", e);
                return false;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("VigemManager.IsVigemDriverInstalled error L38", e);
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
                vig.StartAndListen();
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("VigemManager.Start error L55", e);

                System.Windows.MessageBox.Show("Vigembus not installed!",
                    "Fatal Error",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("VigemManager.Start error L65", e);

                System.Windows.MessageBox.Show("whoops! tell the developer he failed",
                    "Fatal Error",
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
                        vig.Stop();
                    }
                }
            }
            // You might be expecting the catches of this method to mirror, the Start() method
            // logically we are calling VigManager.Stop() is to end the application
            // so there it no point to call application exit
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("VigemManager.Stop error L96", e);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("VigemManager.Stop error L100", e);
            }
        }

    }
}
