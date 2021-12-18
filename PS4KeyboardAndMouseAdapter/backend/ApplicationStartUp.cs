using Pizza;
using Pizza.backend.vigem;
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

            try
            {
                VigemManager vig = new VigemManager();
                vig.start();
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
