using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using PS4RemotePlayInjection;
using Serilog;
using System;
using System.Windows;

namespace Pizza.KeyboardAndMouseAdapter
{
    public partial class App : Application
    {

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            Log.Debug("App OnAppExit");
            InstanceSettings.GetInstance().EnableMouseInput = false;

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            UserSettings.Save(UserSettings.PROFILE_PREVIOUS);
            ApplicationSettings.Save();

            VigemManager.Stop(InstanceSettings.GetInstance());

            //TODO: hardcoded, fix.
            //Injector.FindProcess("RemotePlay").Kill();
        }

        private async void OnAppStartup(object sender, StartupEventArgs e)
        {
            LogManager logManager = new LogManager();
            logManager.SetupLogger();
            InstanceSettings.GetInstance().SetLogManager(logManager);

            Console.WriteLine("app/adapter started");
            Console.WriteLine("for more about what has happened in this app, see logs/log.txt");
            Log.Information("PS4KMA v{0} started", VersionUtil.GetVersionWithBuildDate());

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            ApplicationSettings.Load();
            UserSettings.LoadPrevious();

            AppUpdater appUpdater = new AppUpdater();

            await appUpdater.UpdateIfAvailable();
        }

    }
}
