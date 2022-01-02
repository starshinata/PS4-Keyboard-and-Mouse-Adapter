using Pizza.backend.vigem;
using Pizza.Common;
using PS4KeyboardAndMouseAdapter.Config;
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

            woop();
        }

        //TODO need better name
        public static void woop()
        {
            try
            {
                // wraping in task as it can lag the window
                //TODO test that if we dont have vigem service installed does the error still get propegated 

                System.Threading.Thread.Sleep(1000);
                VigemManager vig = new VigemManager();
                vig.startAndListen();
                InstanceSettings.GetInstance().SetVigemInjector(vig);

            }
            catch (Nefarius.ViGEm.Client.Exceptions.VigemBusNotFoundException e)
            {
                ExceptionLogger.LogException("ApplicationStartUp.OnAppStartup", e);

                System.Windows.MessageBox.Show("Vigembus not installed!",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("ApplicationStartUp.OnAppStartup", e);

                System.Windows.MessageBox.Show("whoops! tell the developer he failed",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }

        }
    }
}
