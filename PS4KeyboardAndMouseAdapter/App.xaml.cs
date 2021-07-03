using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Squirrel;
using System;
using System.IO;

using System.Threading.Tasks;
using System.Windows;

namespace PS4KeyboardAndMouseAdapter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        LoggingLevelSwitch levelSwitch;

        private void OnAppExit(object sender, ExitEventArgs e)
        {

            Log.Debug("App OnAppExit");
            InstanceSettings.GetInstance().EnableMouseInput = false;

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            UserSettings.Save(UserSettings.PROFILE_PREVIOUS);
            ApplicationSettings.Save();

            //TODO: hardcoded, fix.
            //Injector.FindProcess("RemotePlay").Kill();
        }

        private async void OnAppStartup(object sender, StartupEventArgs e)
        {
            SetupLogger();
            Console.WriteLine("app/adapter started");
            Console.WriteLine("for more about what has happened in this app, see logs/log.txt");
            Log.Information("PS4KMA v{0} started", VersionUtil.GetVersionWithBuildDate());

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            ApplicationSettings.Load();
            UserSettings.LoadPrevious();

            await UpdateIfAvailable();
        }

        public void SetLoggingLevel(LogEventLevel level)
        {
            Log.Information("Logger level set to " + level);
            levelSwitch.MinimumLevel = level;
        }

        private void SetupLogger()
        {
            levelSwitch = new LoggingLevelSwitch();
            levelSwitch.MinimumLevel = LogEventLevel.Information;

            Logger log = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Console()
                .WriteTo.File(LogUtility.GetLogFile())
                .CreateLogger();

            Log.Logger = log;
            Log.Information("Log created");
        }

        private async Task UpdateIfAvailable()
        {
            Log.Information("App.UpdateIfAvailable() start");
            try
            {
                Log.Information("App.UpdateIfAvailable() trying GitHubUpdateManager");
                using (UpdateManager mgr = await UpdateManager.GitHubUpdateManager("https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter"))
                {
                    Log.Information("App.UpdateIfAvailable() trying UpdateApp");
                    await mgr.UpdateApp();
                    Log.Information("App.UpdateIfAvailable() UpdateApp complete");
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("App.UpdateIfAvailable() Github Update failed: " + ex.Message);
            }
            Log.Information("App.UpdateIfAvailable() update check completed");

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
    }
}
