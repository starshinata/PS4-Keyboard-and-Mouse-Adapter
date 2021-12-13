using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Windows;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter.backend
{
    public class ApplicationStartUp
    {
        public static void OnAppStartup()
        {

            ApplicationSettings.Load();
            UserSettings.LoadPrevious();

            try
            {
                VigemInjector vig = new VigemInjector();
                vig.start();
                InstanceSettings.GetInstance().SetVigemInjector(vig);
            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                logException(e);

                System.Windows.MessageBox.Show("Vigembus not installed!",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception e)
            {
                logException(e);
                System.Windows.MessageBox.Show("whoops! tell the developer he failed",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }


        }

        private static void logException(Exception ex)
        {
            Log.Logger.Error("HandleLoad failed: " + ex.Message);
            Log.Logger.Error(ex.GetType().ToString());
            Log.Logger.Error(ex.StackTrace);
        }
    }
}
