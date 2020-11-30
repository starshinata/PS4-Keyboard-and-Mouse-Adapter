using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA2;
using PS4KeyboardAndMouseAdapter.Annotations;
using PS4KeyboardAndMouseAdapter.Dll;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;
using SFML.System;
using SFML.Window;

using Application = FlaUI.Core.Application;
using Window = FlaUI.Core.AutomationElements.Window;

namespace PS4KeyboardAndMouseAdapter
{

    public class MainViewModel : INotifyPropertyChanged
    {
        // Constants
        private readonly string TARGET_PROCESS_NAME = "RemotePlay";
        private readonly string INJECT_DLL_NAME = "PS4RemotePlayInjection.dll";

        public DualShockState CurrentState { get; private set; }
        public bool EnableMouseInput { get; set; } = false;
        public Vector2i MouseDirection { get; set; }
        public UserSettings Settings { get; set; } = null;

        public int AnalogX
        {
            get => analogX;
            set
            {
                analogX = value;
                OnPropertyChanged(nameof(AnalogX));
            }
        }

        public int AnalogY
        {
            get => analogY;
            set
            {
                analogY = value;
                OnPropertyChanged(nameof(AnalogY));
            }
        }

        // Anchor is relative to the top left of the primary monitor
        public Vector2i Anchor { get; set; } = new Vector2i(900, 500);

        public Process RemotePlayProcess;

        public string Title { get; set; } = "PS4 Keyboard&Mouse Adapter v" + GetAssemblyVersion();

        public bool IsCursorHideRequested { get; set; }


        // constructor
        // AKA init
        public MainViewModel()
        {
            Log.Information("MainViewModel constructor IN");

            // cause not having a cursor is a pain in the ass
            Utility.ShowCursor(true);

            Injector.FindProcess(TARGET_PROCESS_NAME)?.Kill();

            LoadSettings();

            OpenRemotePlayAndInject();

            Task.Run(AutoClickStart);
            Log.Information("MainViewModel constructor OUT");
        }

        public void AutoClickStart()
        {
            try
            {
                Application app = FlaUI.Core.Application.Attach(Process.GetProcessById(RemotePlayProcess.Id));

                using (var automation = new UIA2Automation())
                {
                    while (true)
                    {
                        Window window = app.GetMainWindow(automation);
                        Button button1 = window.FindFirstDescendant(cf => cf.ByText("Start"))?.AsButton();
                        if (button1 == null)
                            Thread.Sleep(1000);

                        else
                        {
                            button1?.Invoke();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("Problem with auto-clicker: " + ex.Message);
            }
        }

        public static string GetAssemblyVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public void HandleKeyboardInput()
        {
            // ORDER
            // LEFT -- MIDDLE -- RIGHT

            ////////////////////////////////////////////
            ////////////////////////////////////////////

            //left face
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.DPadUp]))
                CurrentState.DPad_Up = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.DPadLeft]))
                CurrentState.DPad_Left = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.DPadDown]))
                CurrentState.DPad_Down = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.DPadRight]))
                CurrentState.DPad_Right = true;

            //left stick Analog
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.LeftStickLeft]))
                CurrentState.LX = 0;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.LeftStickRight]))
                CurrentState.LX = 255;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.LeftStickUp]))
                CurrentState.LY = 0;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.LeftStickDown]))
                CurrentState.LY = 255;

            //left stick Buttons
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.L1]))
                CurrentState.L1 = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.L2]))
                CurrentState.L2 = 255;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.L3]))
                CurrentState.L3 = true;

            ////////////////////////////////////////////
            ////////////////////////////////////////////

            // middle face
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.Share]))
                CurrentState.Share = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.TouchButton]))
                CurrentState.TouchButton = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.Options]))
                CurrentState.Options = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.PlaystationButton]))
                CurrentState.PS = true;

            ////////////////////////////////////////////
            ////////////////////////////////////////////

            //right stick Analog
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.RightStickLeft]))
                CurrentState.RX = 0;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.RightStickRight]))
                CurrentState.RX = 255;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.RightStickUp]))
                CurrentState.RY = 0;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.RightStickDown]))
                CurrentState.RY = 255;

            //right stick Buttons
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.R1]))
                CurrentState.R1 = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.R2]))
                CurrentState.R2 = 255;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.R3]))
                CurrentState.R3 = true;

            //right face
            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.Triangle]))
                CurrentState.Triangle = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.Circle]))
                CurrentState.Circle = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.Cross]))
                CurrentState.Cross = true;

            if (Keyboard.IsKeyPressed(Settings.Mappings[VirtualKey.Square]))
                CurrentState.Square = true;

        }

        public void HandleMouseInput()
        {
            // Mouse Input
            var prevVal = EnableMouseInput;
            EnableMouseInput = IsCursorHideRequested && IsProcessInForeground(RemotePlayProcess);
            if (EnableMouseInput != prevVal)
                Utility.ShowCursor(prevVal);

            if (EnableMouseInput)
            {
                HandleMouseCursor();
                HandleMouseClick();
            }
        }

        public void HandleMouseClick()
        {
            // Mouse Input
            var prevVal = EnableMouseInput;
            EnableMouseInput = IsCursorHideRequested && IsProcessInForeground(RemotePlayProcess);
            if (EnableMouseInput != prevVal)
                Utility.ShowCursor(prevVal);

            if (EnableMouseInput)
            {


                // Left mouse button
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Left))
                    CurrentState.R2 = 255;

                // Right mouse button
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Right))
                    CurrentState.L2 = 255;

                // Middle mouse button
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Middle))
                    CurrentState.R3 = true;
            }
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

                MouseDirection = new Vector2i((int)(MouseDirection.X * Settings.MouseXAxisSensitivityModifier), (int)(MouseDirection.Y * Settings.MouseYAxisSensitivityModifier));
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


        public void LoadSettings()
        {
            try
            {
                UserSettings.LoadPrevious();
            }
            catch (Exception ex)
            {
                Log.Logger.Error("MainViewModel.LoadSettings failed: " + ex.Message);
                Log.Logger.Error(ex.StackTrace);
            }

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

            HandleKeyboardInput();

            HandleMouseInput();

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


        private readonly Stopwatch mouseTimer = new Stopwatch();
        private Vector2i mouseDirection = new Vector2i(0, 0);
        private int analogX;
        private int analogY;

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

        public void SetMapping(VirtualKey key, Keyboard.Key value)
        {
            Log.Information("MainViewModel.SetMapping {VirtKey:" + key + ", keyboardValue: " + value + "}");
            Console.WriteLine("MainViewModel.SetMapping {VirtKey:" + key + ", keyboardValue: " + value + "}");

            Settings.Mappings[key] = value;
            OnPropertyChanged(nameof(Settings));

            UserSettings.Save(UserSettings.PROFILE_PREVIOUS);
        }

        public Process RunRemotePlaySetup()
        {
            MessageBox.Show("In order to play, PS4 Remote Play is required. Do you want to install it now?",
                "Install PS4 Remote play", MessageBoxButton.OK);

            string installerName = "RemotePlayInstaller.exe";

            using (var client = new WebClient())
            {
                client.DownloadFile("https://remoteplay.dl.playstation.net/remoteplay/module/win/RemotePlayInstaller.exe", installerName);
            }

            return Process.Start(installerName);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
