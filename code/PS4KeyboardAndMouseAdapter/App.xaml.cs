using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using PS4RemotePlayInjection;
using Serilog;
using System;
using System.Windows;
using Velopack;

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
        }

        private async void OnAppStartup(object sender, StartupEventArgs e)
        {
            AppStart();
        }

        private void AppStart()
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
            appUpdater.UpdateIfAvailable();
        }

        // Since WPF has an "automatic" Program.Main, we need to create one Velopack.
        // In order for this to work, you must also add the following to your .csproj:
        // <StartupObject>Pizza.KeyboardAndMouseAdapter.App</StartupObject>
        private static void Main(string[] args)
        {
            try {
                // Velopack says it is important to Run() the VelopackApp as early as possible in app startup.
                VelopackApp.Build().Run();

                // We can now launch the WPF application as normal.
                var app = new App();
                app.InitializeComponent();



                app.Run();
                app.AppStart();

            } catch (Exception ex) {
                MessageBox.Show("Unhandled exception: " + ex.ToString());
            }
        }
    }
}
