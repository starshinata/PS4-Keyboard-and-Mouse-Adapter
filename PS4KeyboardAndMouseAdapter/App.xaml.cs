using PS4KeyboardAndMouseAdapter.backend.DebugLogging;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using Serilog;
using Serilog.Core;
using Squirrel;
using System;
using System.IO;
using System.Runtime.InteropServices;

using System.Threading.Tasks;
using System.Windows;

namespace PS4KeyboardAndMouseAdapter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // http://msdn.microsoft.com/en-us/library/ms681944(VS.85).aspx
        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        /// <returns>nonzero if the function succeeds; otherwise, zero.</returns>
        /// <remarks>
        /// A process can be associated with only one console,
        /// so the function fails if the calling process already has a console.
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int AllocConsole();

        // http://msdn.microsoft.com/en-us/library/ms683150(VS.85).aspx
        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        /// <returns>nonzero if the function succeeds; otherwise, zero.</returns>
        /// <remarks>
        /// If the calling process is not already attached to a console,
        /// the error code returned is ERROR_INVALID_PARAMETER (87).
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();

        private void OnAppExit(object sender, ExitEventArgs e)
        {

            Console.WriteLine("App OnAppExit");
            InstanceSettings.GetInstance().EnableMouseInput = false;

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            UserSettings.Save(UserSettings.PROFILE_PREVIOUS);
            ApplicationSettings.Save();

            //TODO: hardcoded, fix.
            //Injector.FindProcess("RemotePlay").Kill();

            FreeConsole();
        }

        private async void OnAppStartup(object sender, StartupEventArgs e)
        {
            AllocConsole();

            SetupLogger();
            Console.WriteLine("app/adapter started");
            Console.WriteLine("for more about what has happened in this app, see logs/log.txt");
            Log.Information("PS4KMA v{0} started", VersionUtil.GetVersionWithBuildDate());

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            ApplicationSettings.Load();
            UserSettings.LoadPrevious();

            DebugDump.Dump();

            await UpdateIfAvailable();
        }

        private void SetupLogger()
        {
            Logger log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt")
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
