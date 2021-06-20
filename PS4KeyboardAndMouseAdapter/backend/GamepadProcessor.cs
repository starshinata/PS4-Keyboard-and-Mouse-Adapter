using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace PS4KeyboardAndMouseAdapter
{

    public class GamepadProcessor
    {
        private InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();
        private UserSettings UserSettings { get; set; } = UserSettings.GetInstance();

        ////////////////////////////////////////////////////////////////////////////

        // timer to know how long it has been since Aim button has been released
        private readonly Stopwatch AimToggleTimer = new Stopwatch();

        // this variable is only used when UserSettings.AimToggle is true
        //
        // "Has" prefix, as in past tense
        //
        // for init the aim button will have never been pressed, so technically it will have never been released
        // so logically the default value is false,
        // BUT if we want the default value to be false we need to add in a bunch of logic
        // so lets have the default as true and skip the extra logic
        private bool HasAimButtonBeenReleased { get; set; } = true;

        private bool IsAiming { get; set; } = false;

        ////////////////////////////////////////////////////////////////////////////

        // position 0,0 is the top left of the primary monitor
        private Vector2i Anchor { get; set; } = new Vector2i(900, 500);

        private DualShockState CurrentState;

        private Vector2i MouseDirection { get; set; }
        private Vector2i MouseDirectionPrevious = new Vector2i(0, 0);

        // timer to know how long it has been since we last polled the mouse for an update
        private readonly Stopwatch MouseInputTimer = new Stopwatch();

        private readonly MouseWheelProcessor MouseWheelProcessor;

        ////////////////////////////////////////////////////////////////////////////

        public GamepadProcessor()
        {
            AimToggleTimer.Start();
            MouseInputTimer.Start();

            MouseWheelProcessor = new MouseWheelProcessor();
        }

        public Vector2i FeedMouseCoords()
        {
            int MillisecondsPerInput = 1000 / UserSettings.MousePollingRate;
            if (MouseInputTimer.ElapsedMilliseconds >= MillisecondsPerInput)
            {
                Vector2i currentMousePosition = Mouse.GetPosition();
                MouseDirectionPrevious = currentMousePosition - Anchor;

                //recalculate incase they moved the window
                Anchor = MouseAnchor.CalculateAnchor();

                Mouse.SetPosition(Anchor);
                MouseInputTimer.Restart();
            }

            // pancakeslp 2020.12.27
            // it might seem sensible to say that we should only return a value when enough time has passed (as per MousePollingRate)
            // and if enough time has not passed, return null
            //
            // if we return null, then the DS/Controller will register that as no stick movement
            // thus the mouse input is perceived as unresponsive or jumpy
            //
            // to mitigate this, just return the previously polled value (and wait for the new value)
            return MouseDirectionPrevious;
        }

        public void HandleAimToggle()
        {
            if (UserSettings.AimToggle)
            {

                // waiting a little before we can re-toggle the Aiming
                if (AimToggleTimer.ElapsedMilliseconds > UserSettings.AimToggleRetoggleDelay)
                {
                    //TODO make it dynamic so the L2 doesnt have to be the AIM key
                    if (IsVirtualKeyPressed(VirtualKey.L2) && HasAimButtonBeenReleased)
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


                // we should only reset the timer once per release of aim button
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

        public void HandleButtonPressed()
        {

            // ORDER
            // LEFT -- MIDDLE -- RIGHT of Controller

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

            // middle
            if (IsVirtualKeyPressed(VirtualKey.Share))
                CurrentState.Share = true;

            if (IsVirtualKeyPressed(VirtualKey.TouchButton))
                CurrentState.TouchButton = true;

            if (IsVirtualKeyPressed(VirtualKey.Options))
                CurrentState.Options = true;

            if (IsVirtualKeyPressed(VirtualKey.PlaystationButton))
                CurrentState.PS = true;


            ////////////////////////////////////////////

            //right face
            if (IsVirtualKeyPressed(VirtualKey.Triangle))
                CurrentState.Triangle = true;

            if (IsVirtualKeyPressed(VirtualKey.Circle))
                CurrentState.Circle = true;

            if (IsVirtualKeyPressed(VirtualKey.Cross))
                CurrentState.Cross = true;

            if (IsVirtualKeyPressed(VirtualKey.Square))
                CurrentState.Square = true;

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
        }

        public void HandleMouseCursor()
        {
            bool EnableMouseInput = InstanceSettings.EnableMouseInput && ProcessUtil.IsRemotePlayInForeground();

            if (EnableMouseInput)
            {
                Utility.ShowCursor(false);

                // mouse displacement relative to the anchor
                MouseDirection = FeedMouseCoords();

                if (IsAimingWithAimSpecificSensitivity())
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

                double normalizedLength = Utility.mapcap(direction.Length(),
                    UserSettings.MouseDistanceLowerRange, UserSettings.MouseDistanceUpperRange,
                    UserSettings.AnalogStickLowerRange / 100f, UserSettings.AnalogStickUpperRange / 100f);

                direction = Vector2.Normalize(direction);


                // L3R3 center is 127, 
                // full left/up is 0
                // full right/down is 255
                byte scaledX = (byte)Utility.map(direction.X * normalizedLength, -1, 1, 0, 255);
                byte scaledY = (byte)Utility.map(direction.Y * normalizedLength, -1, 1, 0, 255);

                direction.X *= (float)UserSettings.XYRatio;
                direction = Vector2.Normalize(direction);

                if (float.IsNaN(direction.X) && float.IsNaN(direction.Y))
                {
                    scaledX = 127;
                    scaledY = 127;
                }

                // if (scaledX != 127 && scaledY != 127)
                // {
                //     Log.Debug("scaledX" + scaledX);
                //     Log.Debug("scaledY" + scaledY);
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

        private bool IsAimingWithAimSpecificSensitivity()
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

        private string DualShockStateToString(ref DualShockState state)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(", 'L1':");
            sb.Append(state.L1);
            sb.Append(", 'L2':");
            sb.Append(state.L2);
            sb.Append(", 'L3':");
            sb.Append(state.L3);

            sb.Append(", 'Lx':");
            sb.Append(state.LX);
            sb.Append(", 'Ly':");
            sb.Append(state.LY);

            sb.Append(", 'R1':");
            sb.Append(state.R1);
            sb.Append(", 'R2':");
            sb.Append(state.R2);
            sb.Append(", 'R3':");
            sb.Append(state.R3);

            sb.Append(", 'Rx':");
            sb.Append(state.RX);
            sb.Append(", 'Ry':");
            sb.Append(state.RY);

            sb.Append(", 'circle':");
            sb.Append(state.Circle);
            sb.Append(", 'cross':");
            sb.Append(state.Cross);
            sb.Append(", 'square':");
            sb.Append(state.Square);
            sb.Append(", 'triangle':");
            sb.Append(state.Triangle);

            sb.Append(", 'DPad_Up':");
            sb.Append(state.DPad_Up);
            sb.Append(", 'DPad_Down':");
            sb.Append(state.DPad_Down);
            sb.Append(", 'DPad_Left':");
            sb.Append(state.DPad_Left);
            sb.Append(", 'DPad_Right':");
            sb.Append(state.DPad_Right);

            sb.Append(", 'PS':");
            sb.Append(state.PS);

            sb.Append("}");

            return sb.ToString();
        }

        public void OnReceiveData(ref DualShockState state)
        {
            Guid uuid = Guid.NewGuid();
            Log.Verbose(uuid + " GamepadProcessor.OnReceiveData in a");

            // Create the default state to modify
            if (true)//CurrentState == null)
            {
                CurrentState = new DualShockState() { Battery = 255 };
            }

            if (!ProcessUtil.IsRemotePlayInForeground())
            {
                Utility.ShowCursor(true);
                return;
            }

            Log.Verbose(uuid + " GamepadProcessor.OnReceiveData in b");

            HandleButtonPressed();

            HandleAimToggle();
            HandleMouseCursor();
            MouseWheelProcessor.Process(CurrentState);

            Log.Verbose(uuid + " GamepadProcessor.OnReceiveData out " + DualShockStateToString(ref CurrentState));

            // Assign the state
            state = CurrentState;
        }
    }
}
