using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using PS4RemotePlayInjection;
using Serilog;
using Squirrel;
using System;
using System.Windows;

namespace Pizza.KeyboardAndMouseAdapter
{
    public partial class App : Application
    {

        // START!
        // pancakeslp: panny you get confused when start isnt first
        // even though normally methods are alphabetically ordered
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

            try
            {
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: SquirrelOnAppInstall,
                    onAppUninstall: SquirrelOnAppUninstall,
                    onEveryRun: SquirrelOnAppRun);
                /*            AppUpdater appUpdater = new AppUpdater();

                            await appUpdater.UpdateIfAvailable();*/
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("App L47 ", ex);
            }
        }

        private void OnAppExit(object sender, ExitEventArgs e)
        {
            Log.Information("App OnAppExit");
            InstanceSettings.GetInstance().EnableMouseInput = false;

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            UserSettings.Save(UserSettings.PROFILE_PREVIOUS);
            ApplicationSettings.Save();

            VigemManager.Stop(InstanceSettings.GetInstance());

            //TODO: hardcoded, fix.
            //Injector.FindProcess("RemotePlay").Kill();
        }

        private static void SquirrelOnAppInstall(SemanticVersion version, IAppTools tools)
        {
            Log.Information("App SquirrelOnAppInstall");
            tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

        private static void SquirrelOnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
        {
            try
            {
                Log.Information("App SquirrelOnAppRun");
                tools.SetProcessAppUserModelId();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("App L87 ", ex);
            }

        }

        private static void SquirrelOnAppUninstall(SemanticVersion version, IAppTools tools)
        {
            Log.Information("App SquirrelOnAppUninstall");
            tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
        }

    }
}
