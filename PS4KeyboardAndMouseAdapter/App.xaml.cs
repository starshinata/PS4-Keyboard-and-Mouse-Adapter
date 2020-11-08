using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
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
            Debug("UpdateIfAvailable 37");
            try
            {
                Debug("UpdateIfAvailable 30");
                using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter"))
                {
                    Debug("UpdateIfAvailable 43");
                    await mgr.UpdateApp();
                    Debug("UpdateIfAvailable 45");
                }
            }
            catch (Exception ex)
            {
                Debug("UpdateIfAvailable 50" + ex.Message);
                Log.Logger.Error("Github Update failed: " + ex.Message);
            }
            Debug("UpdateIfAvailable 53");

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
