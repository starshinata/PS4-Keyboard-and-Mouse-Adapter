using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using PS4RemotePlayInjection;
using Serilog;
using Squirrel;

namespace PS4KeyboardAndMouseAdapter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppExit(object sender, ExitEventArgs e)
        {
            Utility.ShowCursor(true);

            //TODO: hardcoded, fix.
            //Injector.FindProcess("RemotePlay").Kill();
        }

        private void Debug(String message)
        {
            Console.WriteLine(message);
            Log.Information(message);
        }

        private async Task UpdateIfAvailable()
        {
            Debug("App.UpdateIfAvailable() start");
            try
            {
                Debug("App.UpdateIfAvailable() trying GitHubUpdateManager");
                using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter"))
                {
                    Debug("App.UpdateIfAvailable() trying UpdateApp");
                    await mgr.UpdateApp();
                    Debug("App.UpdateIfAvailable() UpdateApp complete");
                }
            }
            catch (Exception ex)
            {
                Debug("App.UpdateIfAvailable() 44" + ex.Message);
                Log.Logger.Error("Github Update failed: " + ex.Message);
            }
            Debug("App.UpdateIfAvailable() update check completed");

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                File.Delete(Path.Combine(desktopPath, "EasyHookSvc.lnk"));
                File.Delete(Path.Combine(desktopPath, "EasyHookSvc64.lnk"));
                File.Delete(Path.Combine(desktopPath, "EasyHookSvc32.lnk"));
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Shortcut deletion failed:" + ex.Message);
            }
        }

        private void SetupLogger()
        {
            var log = new LoggerConfiguration()
                .WriteTo.File("logs/log.txt")
                .CreateLogger();

            Log.Logger = log;
            Log.Information("Log created");
        }

        private async void OnAppStartup(object sender, StartupEventArgs e)
        {
            SetupLogger();
            await UpdateIfAvailable();
        }
    }
}
