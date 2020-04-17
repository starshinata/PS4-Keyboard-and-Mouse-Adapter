using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Logging;
using FlaUI.UIA2;
using Newtonsoft.Json;
using PS4KeyboardAndMouseAdapter.Annotations;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;
using SFML.System;
using SFML.Window;
using Shell32;
using Squirrel;
using Application = FlaUI.Core.Application;
using Window = FlaUI.Core.AutomationElements.Window;

namespace PS4KeyboardAndMouseAdapter
{
    public enum VirtualKey
    {
        Left,
        Right,
        Up,
        Down,
        Triangle,
        Circle,
        Cross,
        Square,
        L1,
        L2,
        L3,
        R1,
        R2,
        R3,
        Options,
        TouchButton,
        DPadLeft,
        DPadRight,
        DPadUp,
        DPadDown,
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        // Constants
        private string TARGET_PROCESS_NAME = "RemotePlay";
        private string INJECT_DLL_NAME = "PS4RemotePlayInjection.dll";

        public DualShockState CurrentState { get; private set; }
        public bool EnableMouseInput { get; set; } = false;
        private Stopwatch mouseIdleTimer = Stopwatch.StartNew();
        public double MouseSensitivity { get; set; } = 3;
        public Vector2i MouseDirection { get; set; }
        public Vector2i anchor { get; set; } = new Vector2i(900, 500);
        public Process RemotePlayProcess;

        public string Title { get; set; } =
            "PS4 Keyboard&Mouse Adapter v" + GetAssemblyVersion();

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; }
        public Dictionary<Keyboard.Key, VirtualKey> ReverseMappings { get; }

        public int MouseFrameCounter = 1;
        public bool IsFrameEven = true;
        private Stopwatch timer = new Stopwatch();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool isCursorHideRequested;

        public bool IsCursorHideRequested
        {
            get => isCursorHideRequested;
            set => isCursorHideRequested = value;
        }

        public static string GetAssemblyVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public void SetMapping(VirtualKey key, Keyboard.Key value)
        {
            Mappings[key] = value;
            OnPropertyChanged(nameof(Mappings));

            string json = JsonConvert.SerializeObject(Mappings, Formatting.Indented);
            File.WriteAllText("mappings.json", json);
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

        public bool OpenRemotePlay()
        {
            var exeLocation = @"C:\Program Files (x86)\Sony\PS4 Remote Play\RemotePlay.exe";

            if (File.Exists(exeLocation))
            {
                Process.Start(exeLocation);
                return true;
            }

            try
            {
                var shortcutPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\PS4 Remote Play.lnk";
                IWshRuntimeLibrary.IWshShell wsh = new IWshRuntimeLibrary.WshShellClass();
                IWshRuntimeLibrary.IWshShortcut sc = (IWshRuntimeLibrary.IWshShortcut) wsh.CreateShortcut(shortcutPath);
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

        public MainViewModel()
        {
            // Mappings = new Dictionary<VirtualKey, Keyboard.Key>();
            // Mappings.Add(VirtualKey.Left, Keyboard.Key.A);
            // Mappings.Add(VirtualKey.Right, Keyboard.Key.D);
            // Mappings.Add(VirtualKey.Up, Keyboard.Key.W);
            // Mappings.Add(VirtualKey.Down, Keyboard.Key.S);
            // Mappings.Add(VirtualKey.Triangle, Keyboard.Key.F);
            // Mappings.Add(VirtualKey.Circle, Keyboard.Key.C);
            // Mappings.Add(VirtualKey.Cross, Keyboard.Key.V);
            // Mappings.Add(VirtualKey.Square, Keyboard.Key.R);
            // Mappings.Add(VirtualKey.L1, Keyboard.Key.Q);
            // Mappings.Add(VirtualKey.L2, Keyboard.Key.K);
            // Mappings.Add(VirtualKey.L3, Keyboard.Key.LShift);
            // Mappings.Add(VirtualKey.R1, Keyboard.Key.E);
            // Mappings.Add(VirtualKey.R2, Keyboard.Key.J);
            // Mappings.Add(VirtualKey.R3, Keyboard.Key.L);
            // Mappings.Add(VirtualKey.Options, Keyboard.Key.O);
            // Mappings.Add(VirtualKey.TouchButton, Keyboard.Key.M);
            // Mappings.Add(VirtualKey.DPadUp, Keyboard.Key.Num1);
            // Mappings.Add(VirtualKey.DPadLeft, Keyboard.Key.Num2);
            // Mappings.Add(VirtualKey.DPadDown, Keyboard.Key.Num3);
            // Mappings.Add(VirtualKey.DPadRight, Keyboard.Key.Num4);
            //
            // File.WriteAllText("mappings.json", JsonConvert.SerializeObject(Mappings, Formatting.Indented));

            Injector.FindProcess(TARGET_PROCESS_NAME)?.Kill();

            EventWaitHandle waitHandle = new ManualResetEvent(initialState: false);

            string json = File.ReadAllText("mappings.json");
            Mappings = JsonConvert.DeserializeObject<Dictionary<VirtualKey, Keyboard.Key>>(json);

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
            Task.Run(AutoClickStart);
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

        public void Inject()
        {
            Thread.Sleep(3100);
            int remotePlayProcessId = Injector.Inject(TARGET_PROCESS_NAME, INJECT_DLL_NAME);
            RemotePlayProcess = Process.GetProcessById(remotePlayProcessId);
            RemotePlayProcess.EnableRaisingEvents = true;
            RemotePlayProcess.Exited += (sender, args) => { Utility.ShowCursor(true); };

            Injector.Callback += OnReceiveData;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

        public void OnReceiveData(ref DualShockState state)
        {
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

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Left]))
                CurrentState.LX = 0;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Right]))
                CurrentState.LX = 255;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Up]))
                CurrentState.LY = 0;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Down]))
                CurrentState.LY = 255;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Triangle]))
                CurrentState.Triangle = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Circle]))
                CurrentState.Circle = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Cross]))
                CurrentState.Cross = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Square]))
                CurrentState.Square = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.Options]))
                CurrentState.Options = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.L1]))
                CurrentState.L1 = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.L2]))
                CurrentState.L2 = 255;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.L3]))
                CurrentState.L3 = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.R1]))
                CurrentState.R1 = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.R2]))
                CurrentState.R2 = 255;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.R3]))
                CurrentState.R3 = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.TouchButton]))
                CurrentState.TouchButton = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.DPadUp]))
                CurrentState.DPad_Up = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.DPadLeft]))
                CurrentState.DPad_Left = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.DPadDown]))
                CurrentState.DPad_Down = true;

            if (Keyboard.IsKeyPressed(Mappings[VirtualKey.DPadRight]))
                CurrentState.DPad_Right = true;

            // Mouse Input
            var prevVal = EnableMouseInput;
            RemotePlayProcess.Refresh();
            EnableMouseInput = IsCursorHideRequested && IsProcessInForeground(RemotePlayProcess);
            if (EnableMouseInput != prevVal)
                Utility.ShowCursor(prevVal);

            if (EnableMouseInput)
            {
                //ShowCursor(false);
                var checkState = new DualShockState();

                // Left mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Left))
                    CurrentState.R2 = 255;

                // Right mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Right))
                    CurrentState.L2 = 255;

                // Middle mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Middle))
                    CurrentState.R3 = true;

                MouseDirection = FeedMouseCoords();

                //string analogProperty = MouseMovementAnalog == AnalogStick.Left ? "L" : "R";

                var direction = new Vector2(MouseDirection.X, MouseDirection.Y);
                int maxLength = 6;
                int minLength = 3;
                var length = direction.Length() > maxLength ? maxLength : direction.Length();
                if (length < minLength)
                    length = minLength;
                direction = Vector2.Normalize(direction);
                //direction = Vector2.Multiply(direction, minLength);

                var scaledX = (byte)map(direction.X*length, -maxLength, maxLength, 0, 255);
                var scaledY = (byte)map(direction.Y*length, -maxLength, maxLength, 0, 255);

                if (float.IsNaN(direction.X) && float.IsNaN(direction.Y))
                {
                    scaledX = 128;
                    scaledY = 128;
                }

                // Set the analog values
                CurrentState.RX = scaledX;
                CurrentState.RY = scaledY;

                ///Console.WriteLine("{0:000}, {1:000}", scaledX, scaledY);
                //if(scaledX != 128 && scaledY != 128) Console.WriteLine("{0}  {1}", MouseSpeedX, MouseSpeedY);

                // Invoke callback
                //OnMouseAxisChanged?.Invoke(scaledX, scaledY);
            }

            // Assign the state
            state = CurrentState;
        }

        private readonly Stopwatch mouseTimer = new Stopwatch();
        private Vector2i mouseDirection = new Vector2i(0, 0);

        public Vector2i FeedMouseCoords()
        {
            mouseTimer.Start();

            if (mouseTimer.ElapsedMilliseconds >= 33)
            {
                Vector2i currentMousePosition = Mouse.GetPosition();
                mouseDirection = currentMousePosition - anchor;
                Mouse.SetPosition(anchor);
                mouseTimer.Restart();
            }

            return mouseDirection;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        public static bool IsProcessInForeground(Process process)
        {
            if (process == null)
                return false;

            var activeWindow = GetForegroundWindow();

            if (activeWindow == IntPtr.Zero)
                return false;
            
            if (activeWindow != process.MainWindowHandle)
                return false;

            return true;
        }

        double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
