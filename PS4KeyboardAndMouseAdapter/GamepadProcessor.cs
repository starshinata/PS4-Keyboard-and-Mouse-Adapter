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
        private InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();
        private UserSettings UserSettings { get; set; } = UserSettings.GetInstance();

        ////////////////////////////////////////////////////////////////////////////
        
        private readonly Stopwatch AimToggleTimer = new Stopwatch();

        // only used when UserSettings.AimToggle is true
        // assuming Mouse2 is Aim,
        // Has as in past tense
        //
        // for init the aim button will have never been pressed, so technically it will have never been released
        // so logically the default value is false,
        // if we want the default value to be false we need to add in a bunch of logic
        // so lets have the default as true and skip the extra logic
        private bool HasAimButtonBeenReleased { get; set; } = true;

        private bool IsAiming { get; set; } = false;

        ////////////////////////////////////////////////////////////////////////////

        // Anchor 0,0 is the top left of the primary monitor
        private Vector2i Anchor { get; set; } = new Vector2i(900, 500);

        private DualShockState CurrentState { get; set; }
        
        private Vector2i MouseDirection { get; set; }
        private Vector2i MouseDirectionTemp = new Vector2i(0, 0);

        private readonly Stopwatch MouseTimer = new Stopwatch();

        public Process RemotePlayProcess;

        ////////////////////////////////////////////////////////////////////////////
        
        public GamepadProcessor()
        {
            AimToggleTimer.Start();
            MouseTimer.Start();
        }

        public void HandleAimToggle()
        {
            if (UserSettings.AimToggle)
            {

                // waiting a little before we can re-toggle the Aiming
                if (AimToggleTimer.ElapsedMilliseconds > UserSettings.AimToggleRetoggleDelay)
                {
                    //TODO make it dynamic so the L2 doesnt have to be the AIM key
                    if (IsVirtualKeyPressed(VirtualKey.L2)  && HasAimButtonBeenReleased)
                    {
                        HasAimButtonBeenReleased = false;
                        IsAiming = !IsAiming;
                    }
                }


                if (IsAiming)
                {
                    CurrentState.L2 = 255;
                }
                else
                {
                    CurrentState.L2 = 0;
                }


                // we should only reset the timer once
                // 
                // we could have said ` !IsVirtualKeyPressed(VirtualKey.L2) `
                // however that would mean as soon as you release RightMouse/L2 then you restart the timer
                // and keep restarting the timer, so that the AimToggleRetoggleDelay is never met
                if (!IsVirtualKeyPressed(VirtualKey.L2) && !HasAimButtonBeenReleased)
                {
                    HasAimButtonBeenReleased = true;
                   
                    AimToggleTimer.Restart();
                }
            }
        }

        public Vector2i FeedMouseCoords()
        {

            int MillisecondsPerInput = 1000 / UserSettings.MousePollingRate;
            if (MouseTimer.ElapsedMilliseconds >= MillisecondsPerInput)
            {
                Vector2i currentMousePosition = Mouse.GetPosition();
                MouseDirectionTemp = currentMousePosition - Anchor;

                //recalculate incase they moved the window
                Anchor = MouseAnchor.CalculateAnchor();

                Mouse.SetPosition(Anchor);
                MouseTimer.Restart();
            }

            // pancakeslp 2020.12.27
            // it might seem sensible that was can just return a value when when the time between pollingRates has happened
            // if we do this, sometimes we wont return a value
            // if we dont return a value the mouse input is perceived  as unresponsive of jumpy
            // so until we have a new polled value, just return the previous value
            return MouseDirectionTemp;
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


            ////////////////////////////////////////////
            HandleAimToggle();
        }

        public void HandleMouseCursor()
        {
            bool EnableMouseInput = InstanceSettings.EnableMouseInput && IsProcessInForeground(RemotePlayProcess);

            if (EnableMouseInput)
            {
                Utility.ShowCursor(false);

                // mouse displacement relative to the anchor
                MouseDirection = FeedMouseCoords();

                if (IsAimingWithAimSpecificSenitivity())
                {
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * UserSettings.MouseXAxisSensitivityAimModifier),
                        (int)(MouseDirection.Y * UserSettings.MouseYAxisSensitivityAimModifier));
                }
                else
                {
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * UserSettings.MouseXAxisSensitivityLookModifier),
                        (int)(MouseDirection.Y * UserSettings.MouseYAxisSensitivityLookModifier));
                }
                Vector2 direction = new Vector2(MouseDirection.X, MouseDirection.Y);

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

                // if (scaledX != 127 && scaledY != 127)
                // {
                //     Console.WriteLine("scaledX" + scaledX);
                //     Console.WriteLine("scaledY" + scaledY);
                // }

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

        private bool IsAimingWithAimSpecificSenitivity()
        {
            return IsAiming && UserSettings.MouseAimSensitivityEnabled;
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

            IntPtr activeWindow = User32.GetForegroundWindow();

            if (activeWindow == IntPtr.Zero)
                return false;

            if (activeWindow != process.MainWindowHandle)
                return false;

            return true;
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
