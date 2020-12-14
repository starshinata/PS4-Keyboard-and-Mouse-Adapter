using PS4KeyboardAndMouseAdapter.Config;
using PS4KeyboardAndMouseAdapter.Dll;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter
{

    public class GamepadProcessor
    {
        // Constants
        private readonly string TARGET_PROCESS_NAME = "RemotePlay";
        private readonly string INJECT_DLL_NAME = "PS4RemotePlayInjection.dll";

        public DualShockState CurrentState { get; private set; }
        public bool EnableMouseInput { get; set; } = false;
        public Vector2i MouseDirection { get; set; }
        public UserSettings Settings { get; set; } = UserSettings.GetInstance();

        public int AnalogX { get; set; }
        public int AnalogY { get; set; }

        private readonly Stopwatch mouseTimer = new Stopwatch();
        private Vector2i mouseDirection = new Vector2i(0, 0);

        // Anchor is relative to the top left of the primary monitor
        public Vector2i Anchor { get; set; } = new Vector2i(900, 500);

        public Process RemotePlayProcess;

        public string Title { get; set; } = "PS4 Keyboard&Mouse Adapter v" + GetAssemblyVersion();

        public bool IsCursorHideRequested { get; set; }


        // constructor
        // AKA init
        public GamepadProcessor()
        {
            Log.Information("MainViewModel constructor IN");

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            Injector.FindProcess(TARGET_PROCESS_NAME)?.Kill();

            LoadSettings();

            OpenRemotePlayAndInject();

            Log.Information("MainViewModel constructor OUT");
        }

        public Vector2i FeedMouseCoords()
        {
            mouseTimer.Start();

            int MillisecondsPerInput = 1000 / Settings.MousePollingRate;
            if (mouseTimer.ElapsedMilliseconds >= MillisecondsPerInput)
            {
                Vector2i currentMousePosition = Mouse.GetPosition();
                mouseDirection = currentMousePosition - Anchor;

                //recalculate incase they moved the window
                Anchor = MouseAnchor.CalculateAnchor();

                Mouse.SetPosition(Anchor);
                mouseTimer.Restart();
            }

            return mouseDirection;
        }

        public static string GetAssemblyVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public void HandleButtonPressed()
        {
            // ORDER
            // LEFT -- MIDDLE -- RIGHT

            ////////////////////////////////////////////
            ////////////////////////////////////////////

            //left face
            if (IsVirtualKeyPressed(VirtualKey.DPadUp))
                CurrentState.DPad_Up = true;

            if (IsVirtualKeyPressed(VirtualKey.DPadLeft))
                CurrentState.DPad_Left = true;

            if (IsVirtualKeyPressed(VirtualKey.DPadDown))
                CurrentState.DPad_Down = true;

            if (IsVirtualKeyPressed(VirtualKey.DPadRight))
                CurrentState.DPad_Right = true;

            //left stick Analog
            if (IsVirtualKeyPressed(VirtualKey.LeftStickLeft))
                CurrentState.LX = 0;

            if (IsVirtualKeyPressed(VirtualKey.LeftStickRight))
                CurrentState.LX = 255;

            if (IsVirtualKeyPressed(VirtualKey.LeftStickUp))
                CurrentState.LY = 0;

            if (IsVirtualKeyPressed(VirtualKey.LeftStickDown))
                CurrentState.LY = 255;

            //left stick Buttons
            if (IsVirtualKeyPressed(VirtualKey.L1))
                CurrentState.L1 = true;

            if (IsVirtualKeyPressed(VirtualKey.L2))
                CurrentState.L2 = 255;

            if (IsVirtualKeyPressed(VirtualKey.L3))
                CurrentState.L3 = true;

            ////////////////////////////////////////////
            ////////////////////////////////////////////

            // middle face
            if (IsVirtualKeyPressed(VirtualKey.Share))
                CurrentState.Share = true;

            if (IsVirtualKeyPressed(VirtualKey.TouchButton))
                CurrentState.TouchButton = true;

            if (IsVirtualKeyPressed(VirtualKey.Options))
                CurrentState.Options = true;

            if (IsVirtualKeyPressed(VirtualKey.PlaystationButton))
                CurrentState.PS = true;

            ////////////////////////////////////////////
            ////////////////////////////////////////////

            //right stick Analog
            if (IsVirtualKeyPressed(VirtualKey.RightStickLeft))
                CurrentState.RX = 0;

            if (IsVirtualKeyPressed(VirtualKey.RightStickRight))
                CurrentState.RX = 255;

            if (IsVirtualKeyPressed(VirtualKey.RightStickUp))
                CurrentState.RY = 0;

            if (IsVirtualKeyPressed(VirtualKey.RightStickDown))
                CurrentState.RY = 255;

            //right stick Buttons
            if (IsVirtualKeyPressed(VirtualKey.R1))
                CurrentState.R1 = true;

            if (IsVirtualKeyPressed(VirtualKey.R2))
                CurrentState.R2 = 255;

            if (IsVirtualKeyPressed(VirtualKey.R3))
                CurrentState.R3 = true;

            //right face
            if (IsVirtualKeyPressed(VirtualKey.Triangle))
                CurrentState.Triangle = true;

            if (IsVirtualKeyPressed(VirtualKey.Circle))
                CurrentState.Circle = true;

            if (IsVirtualKeyPressed(VirtualKey.Cross))
                CurrentState.Cross = true;

            if (IsVirtualKeyPressed(VirtualKey.Square))
                CurrentState.Square = true;

        }


        public void HandleMouseCursor()
        {
            // Mouse Input
            var prevVal = EnableMouseInput;
            EnableMouseInput = IsCursorHideRequested && IsProcessInForeground(RemotePlayProcess);
            if (EnableMouseInput != prevVal)
                Utility.ShowCursor(prevVal);

            if (EnableMouseInput)
            {
                Utility.ShowCursor(false);

                // mouse displacement relative to the anchor
                MouseDirection = FeedMouseCoords();

                if (IsUserAiming())
                {
                    Console.WriteLine("Aiming, X:" + Settings.MouseXAxisSensitivityAimModifier + " Y: " + Settings.MouseYAxisSensitivityAimModifier);
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * Settings.MouseXAxisSensitivityAimModifier),
                        (int)(MouseDirection.Y * Settings.MouseYAxisSensitivityAimModifier));
                }
                else
                {

                    Console.WriteLine("LOOKING, X:" + Settings.MouseXAxisSensitivityLookModifier + " Y: " + Settings.MouseYAxisSensitivityLookModifier);
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * Settings.MouseXAxisSensitivityLookModifier),
                        (int)(MouseDirection.Y * Settings.MouseYAxisSensitivityLookModifier));
                }
                var direction = new Vector2(MouseDirection.X, MouseDirection.Y);

                // Cap length to fit range.

                var normalizedLength = Utility.mapcap(direction.Length(),
                    Settings.MouseDistanceLowerRange, Settings.MouseDistanceUpperRange,
                    Settings.AnalogStickLowerRange / 100f, Settings.AnalogStickUpperRange / 100f);

                direction = Vector2.Normalize(direction);


                // L3R3 center is 127, 
                // full left is 0
                // full right is 255
                var scaledX = (byte)Utility.map(direction.X * normalizedLength, -1, 1, 0, 255);
                var scaledY = (byte)Utility.map(direction.Y * normalizedLength, -1, 1, 0, 255);

                direction.X *= (float)Settings.XYRatio;
                direction = Vector2.Normalize(direction);

                if (float.IsNaN(direction.X) && float.IsNaN(direction.Y))
                {
                    scaledX = 127;
                    scaledY = 127;
                }

                if (scaledX != 127 && scaledY != 127)
                {
                    Console.WriteLine("scaledX" + scaledX);
                    Console.WriteLine("scaledY" + scaledY);
                }


                if (Settings.MouseControlsL3)
                {
                    CurrentState.LX = scaledX;
                    CurrentState.LY = scaledY;
                }

                if (Settings.MouseControlsR3)
                {
                    CurrentState.RX = scaledX;
                    CurrentState.RY = scaledY;
                }
            }
        }

        public void Inject()
        {
            Thread.Sleep(3100);
            int remotePlayProcessId = Injector.Inject(TARGET_PROCESS_NAME, INJECT_DLL_NAME);
            RemotePlayProcess = Process.GetProcessById(remotePlayProcessId);
            RemotePlayProcess.EnableRaisingEvents = true;
            RemotePlayProcess.Exited += (sender, args) => { Utility.ShowCursor(true); };

            Injector.Callback += OnReceiveData;
        }

        public static bool IsProcessInForeground(Process process)
        {
            if (process == null)
                return false;

            var activeWindow = User32.GetForegroundWindow();

            if (activeWindow == IntPtr.Zero)
                return false;

            if (activeWindow != process.MainWindowHandle)
                return false;

            return true;
        }

        private bool IsUserAiming()
        {
            return Settings.MouseAimSensitivityEnabled && SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Right);
        }

        public bool IsVirtualKeyPressed(VirtualKey key)
        {
            if (key == VirtualKey.NULL)
                return false;

            if (Settings.Mappings[key] == null)
                return false;

            if (Keyboard.IsKeyPressed(Settings.Mappings[key].KeyboardValue))
                return true;

            if (Settings.Mappings[key].MouseValue != MouseButton.Unknown)
            {
                Mouse.Button csfmlMouseButton = (Mouse.Button)Settings.Mappings[key].MouseValue;
                if (Mouse.IsButtonPressed(csfmlMouseButton))
                    return true;
            }

            return false;
        }

        public void LoadSettings()
        {
            UserSettings.LoadPrevious();
            Settings = UserSettings.GetInstance();
        }

        public void OnReceiveData(ref DualShockState state)
        {
            // if (!IsCursorHideRequested)
            //     return;
            //timer.Start();
            //counter++;
            //if (timer.ElapsedMilliseconds > 1000)
            //{
            //    Console.WriteLine("OnReceiveData is called " + counter + "times per second");
            //    counter = 0;
            //    timer.Restart();
            //}

            // Create the default state to modify
            if (true)//CurrentState == null)
            {
                CurrentState = new DualShockState() { Battery = 255 };
            }

            if (!IsProcessInForeground(RemotePlayProcess))
            {
                Utility.ShowCursor(true);
                return;
            }

            HandleButtonPressed();
            HandleMouseCursor();

            // Assign the state
            state = CurrentState;
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
                System.Windows.MessageBox.Show("Fatal error, program closing",
                   "fatal",
                   (MessageBoxButton)MessageBoxButtons.OK,
                   (MessageBoxImage)MessageBoxIcon.Error);
                throw e;
            }
        }

        public Process RunRemotePlaySetup()
        {
            System.Windows.MessageBox.Show("In order to play, PS4 Remote Play is required. Do you want to install it now?",
                "Install PS4 Remote play", MessageBoxButton.OK);

            string installerName = "RemotePlayInstaller.exe";

            using (var client = new WebClient())
            {
                client.DownloadFile("https://remoteplay.dl.playstation.net/remoteplay/module/win/RemotePlayInstaller.exe", installerName);
            }

            return Process.Start(installerName);
        }

    }
}
