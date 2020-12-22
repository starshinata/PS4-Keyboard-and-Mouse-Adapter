using System;
using System.Diagnostics;
using System.Numerics;
using PS4KeyboardAndMouseAdapter.Config;
using PS4KeyboardAndMouseAdapter.Dll;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using SFML.System;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter
{

    public class GamepadProcessor
    {

        public UserSettings UserSettings { get; set; } = UserSettings.GetInstance();
        public InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();

        public DualShockState CurrentState { get; private set; }

        public Vector2i MouseDirection { get; set; }
        public int AnalogX { get; set; }
        public int AnalogY { get; set; }

        private readonly Stopwatch mouseTimer = new Stopwatch();
        private Vector2i mouseDirection = new Vector2i(0, 0);

        // Anchor 0,0 is the top left of the primary monitor
        public Vector2i Anchor { get; set; } = new Vector2i(900, 500);

        public Process RemotePlayProcess;

        public Vector2i FeedMouseCoords()
        {
            mouseTimer.Start();

            int MillisecondsPerInput = 1000 / UserSettings.MousePollingRate;
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
            bool EnableMouseInput = InstanceSettings.EnableMouseInput && IsProcessInForeground(RemotePlayProcess);

            if (EnableMouseInput)
            {
                Utility.ShowCursor(false);

                // mouse displacement relative to the anchor
                MouseDirection = FeedMouseCoords();

                if (IsUserAiming())
                {
                    Console.WriteLine("Aiming, X:" + UserSettings.MouseXAxisSensitivityAimModifier + " Y: " + UserSettings.MouseYAxisSensitivityAimModifier);
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * UserSettings.MouseXAxisSensitivityAimModifier),
                        (int)(MouseDirection.Y * UserSettings.MouseYAxisSensitivityAimModifier));
                }
                else
                {

                    Console.WriteLine("LOOKING, X:" + UserSettings.MouseXAxisSensitivityLookModifier + " Y: " + UserSettings.MouseYAxisSensitivityLookModifier);
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * UserSettings.MouseXAxisSensitivityLookModifier),
                        (int)(MouseDirection.Y * UserSettings.MouseYAxisSensitivityLookModifier));
                }
                var direction = new Vector2(MouseDirection.X, MouseDirection.Y);

                // Cap length to fit range.

                var normalizedLength = Utility.mapcap(direction.Length(),
                    UserSettings.MouseDistanceLowerRange, UserSettings.MouseDistanceUpperRange,
                    UserSettings.AnalogStickLowerRange / 100f, UserSettings.AnalogStickUpperRange / 100f);

                direction = Vector2.Normalize(direction);


                // L3R3 center is 127, 
                // full left/up is 0
                // full right/down is 255
                var scaledX = (byte)Utility.map(direction.X * normalizedLength, -1, 1, 0, 255);
                var scaledY = (byte)Utility.map(direction.Y * normalizedLength, -1, 1, 0, 255);

                direction.X *= (float)UserSettings.XYRatio;
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

                if (UserSettings.MouseControlsL3)
                {
                    CurrentState.LX = scaledX;
                    CurrentState.LY = scaledY;
                }

                if (UserSettings.MouseControlsR3)
                {
                    CurrentState.RX = scaledX;
                    CurrentState.RY = scaledY;
                }
            }
        }

        public bool IsPhysicalKeyPressed(PhysicalKey key)
        {
            if (Keyboard.IsKeyPressed(key.KeyboardValue))
                return true;

            if (key.MouseValue != MouseButton.Unknown)
            {
                Mouse.Button csfmlMouseButton = (Mouse.Button)key.MouseValue;
                if (Mouse.IsButtonPressed(csfmlMouseButton))
                {
                    return true;
                }
            }

            return false;
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
            //TODO make this dynamic
            return UserSettings.MouseAimSensitivityEnabled && Mouse.IsButtonPressed(Mouse.Button.Right);
        }

        public bool IsVirtualKeyPressed(VirtualKey key)
        {
            if (key == VirtualKey.NULL)
                return false;

            PhysicalKeyGroup pkg = UserSettings.Mappings[key];
            if (pkg == null || pkg.PhysicalKeys == null)
                return false;

            foreach (PhysicalKey pk in pkg.PhysicalKeys)
            {
                if (IsPhysicalKeyPressed(pk))
                {
                    return true;
                }
            }

            return false;
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
    }
}
