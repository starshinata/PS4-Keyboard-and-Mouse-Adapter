using PS4KeyboardAndMouseAdapter.backend;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PS4KeyboardAndMouseAdapter
{

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

            VigemInjector vig = InstanceSettings.GetInstance().GetVigemInjector();
            vig.stop();

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

            ApplicationStartUp.OnAppStartup();

            AppUpdater appUpdater = new AppUpdater();
            await appUpdater.UpdateIfAvailable();
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

    }
}
