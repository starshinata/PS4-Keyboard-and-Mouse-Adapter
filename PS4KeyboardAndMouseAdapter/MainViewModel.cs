using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;

namespace PS4KeyboardAndMouseAdapter
{

    public class MainViewModel
    {
        // Constants
        private readonly string TARGET_PROCESS_NAME = "RemotePlay";
        private readonly string INJECT_DLL_NAME = "PS4RemotePlayInjection.dll";

        public InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();
        public UserSettings Settings { get; set; } = UserSettings.GetInstance();

        public Process RemotePlayProcess;

        private GamepadProcessor gp;

        public string WindowTitle { get; set; } = "PS4 Keyboard&Mouse Adapter v" + GetAssemblyVersion();


        public MainViewModel()
        {
            Log.Information("MainViewModel constructor IN");

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            Injector.FindProcess(TARGET_PROCESS_NAME)?.Kill();

            //GamepadProcessor needs to exist, so we can bind/inject PsRemotePlay
            gp = new GamepadProcessor();

            OpenRemotePlayAndInject();

            //Now that we have RemotePlayprocess, GamepadProcessor needs to know what the process is
            gp.RemotePlayProcess = RemotePlayProcess;

            Log.Information("MainViewModel constructor OUT");
        }

        public static string GetAssemblyVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public void Inject()
        {
            try
            {
                Thread.Sleep(3100);
                int remotePlayProcessId = Injector.Inject(TARGET_PROCESS_NAME, INJECT_DLL_NAME);
                RemotePlayProcess = Process.GetProcessById(remotePlayProcessId);
                RemotePlayProcess.EnableRaisingEvents = true;
                RemotePlayProcess.Exited += (sender, args) => { Utility.ShowCursor(true); };

                Injector.Callback += gp.OnReceiveData;
            }
            catch (Exception e)
            {
                Log.Logger.Error("MainViewModel Inject error: " + e.Message);
                Log.Logger.Error(e.StackTrace);
            }
        }

        public bool OpenRemotePlay()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            var exeLocation = path + @"\Sony\PS Remote Play\RemotePlay.exe";

            if (File.Exists(exeLocation))
            {
                Process.Start(exeLocation);
                return true;
            }

            try
            {
                //TODO: hardcoded currently, so it doesn't work when OS is set to non-default system language.
                var shortcutPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\PS Remote Play.lnk";
                IWshRuntimeLibrary.IWshShell wsh = new IWshRuntimeLibrary.WshShellClass();
                IWshRuntimeLibrary.IWshShortcut sc = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);
                shortcutPath = sc.TargetPath;

                if (string.IsNullOrEmpty(shortcutPath))
                    return false;

                Process.Start(shortcutPath);
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error("Cannot open RemotePlay: " + e.Message);
            }

            return false;
        }

        public void OpenRemotePlayAndInject()
        {
            try
            {
                EventWaitHandle waitHandle = new ManualResetEvent(initialState: false);

                bool success = OpenRemotePlay();
                if (!success)
                {
                    Process installerProcess = RunRemotePlaySetup();
                    installerProcess.EnableRaisingEvents = true;
                    installerProcess.Exited += (sender, args) =>
                    {
                        OpenRemotePlay();
                        Inject();
                        waitHandle.Set();
                    };

                    waitHandle.WaitOne();
                }
                else
                {
                    Inject();
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error("MainViewModel OpenRemotePlayAndInject() fatal error" + e.Message);
                Log.Logger.Error("" + e.GetType());
                Log.Logger.Error(e.StackTrace);
                    
                System.Windows.MessageBox.Show(
                    "Fatal error, program closing",
                    "fatal",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);

                throw e;
            }
        }

        public Process RunRemotePlaySetup()
        {
            System.Windows.MessageBox.Show(
                "In order to play, PS4 Remote Play is required. Do you want to install it now?",
                "Install PS4 Remote play",
                MessageBoxButton.OK);

            string installerName = "RemotePlayInstaller.exe";

            using (var client = new WebClient())
            {
                client.DownloadFile("https://remoteplay.dl.playstation.net/remoteplay/module/win/RemotePlayInstaller.exe", installerName);
            }

            return Process.Start(installerName);
        }

    }
}
