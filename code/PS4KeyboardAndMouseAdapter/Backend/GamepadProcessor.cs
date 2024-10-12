using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.ControllerState;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;
using Serilog;
using SFML.System;
using SFML.Window;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Pizza.KeyboardAndMouseAdapter.Backend
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

        private int RequestsPerSecondCounter = 0;
        private readonly Stopwatch RequestsPerSecondTimer = new Stopwatch();

        private readonly MouseWheelProcessor MouseWheelProcessor;

        ////////////////////////////////////////////////////////////////////////////

        public GamepadProcessor()
        {
            AimToggleTimer.Start();
            MouseInputTimer.Start();
            RequestsPerSecondTimer.Start();

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
                Log.Verbose("GamepadProcessor.FeedMouseCoords new rawX={0}, rawY={1}", MouseDirectionPrevious.X, MouseDirectionPrevious.Y);
            }
            else
            {
                Log.Verbose("GamepadProcessor.FeedMouseCoords old rawX={0}, rawY={1}", MouseDirectionPrevious.X, MouseDirectionPrevious.Y);
            }

            // pancakeslp 2020.12.27
            // it might seem sensible to say that we should only return a value when enough time has passed (as per MousePollingRate)
            // and if enough time has not passed, return null
            //
            // BUT if we return null, then the DS/Controller will register that as no stick movement
            // reseting sticks to appear centered
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
                // Log.Verbose("GamepadProcessor.HandleMouseCursor rawX={0}, rawY={1}", MouseDirection.X, MouseDirection.Y);

                if (IsAimingWithAimSpecificSensitivity())
                {
                    // Log.Verbose("GamepadProcessor.HandleMouseCursor MouseXAxisSensitivity_AIM_Modifier " + UserSettings.MouseXAxisSensitivityAimModifier);
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * UserSettings.MouseXAxisSensitivityAimModifier),
                        (int)(MouseDirection.Y * UserSettings.MouseYAxisSensitivityAimModifier));
                }
                else
                {
                    // Log.Verbose("GamepadProcessor.HandleMouseCursor MouseXAxisSensitivity_LOOK_Modifier " + UserSettings.MouseXAxisSensitivityLookModifier);
                    MouseDirection = new Vector2i(
                        (int)(MouseDirection.X * UserSettings.MouseXAxisSensitivityLookModifier),
                        (int)(MouseDirection.Y * UserSettings.MouseYAxisSensitivityLookModifier));
                }
                // Log.Verbose("GamepadProcessor.HandleMouseCursor modifiedX={0}, modifiedY={1}", MouseDirection.X, MouseDirection.Y);


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

                if (scaledX != 127 && scaledX != 128 && scaledY != 127 && scaledY != 128)
                {
                    // Log.Verbose("GamepadProcessor.HandleMouseCursor scaledX={0} scaledY={1}", scaledX, scaledY);
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

        // return TRUE if we did an operation using On Screen Sticks
        // otherwise return FALSE
        private bool HandleOnscreenSticks()
        {
            bool EnableOnscreenSticks = InstanceSettings.EnableMouseInput;


            if (EnableOnscreenSticks)
            {
                //TODO get value
                LeftRightSticks x = null;

                if (x != null && x.Left != null && x.Right != null)
                {

                    byte leftX = HandleOnscreenStickCoordinate(x.Left.X);
                    byte leftY = HandleOnscreenStickCoordinate(x.Left.Y);

                    CurrentState.LX = leftX;
                    CurrentState.LY = leftY;

                    byte rightX = HandleOnscreenStickCoordinate(x.Right.X);
                    byte rightY = HandleOnscreenStickCoordinate(x.Right.Y);

                    CurrentState.RX = rightX;
                    CurrentState.RY = rightY;

                    return true;
                }
            }

            return false;
        }

        // in Point you have two Coordinates
        // Each coordinates is a range between -1 and 1
        // 
        // think of
        // -1 as 100% Left
        // 1 as 100% Right
        // -1 as 100% Up
        // 1 as 100% Down
        private byte HandleOnscreenStickCoordinate(double xOrY)
        {
            // convert the range -1 to 1
            // to -127 to 127
            int scaledXOrY = (int)(xOrY * 127);

            // convert the range -127 to 127
            // to 0 to 255
            scaledXOrY += 127;

            return (byte)scaledXOrY;
        }

        private bool IsAimingWithAimSpecificSensitivity()
        {
            return IsAiming && UserSettings.MouseAimSensitivityEnabled;
        }

        private bool IsPhysicalKeyPressed(PhysicalKey key)
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

        private bool IsVirtualKeyPressed(VirtualKey key)
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

        public string DualShockStateToString(ref DualShockState state)
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
            DualShockState newState = GetState();
            if (newState != null)
            {
                state = newState;
            }
        }

        public DualShockState GetState()
        {
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
            Log.Verbose("GamepadProcessor.GetState screen width={0} height={1}", screenWidth, screenHeight);

            RequestsPerSecondCounter++;
            if (RequestsPerSecondTimer.ElapsedMilliseconds >= 1000)
            {
                Log.Verbose("GamepadProcessor.GetState  RequestsPerSecondCounter={0}", RequestsPerSecondCounter);
                RequestsPerSecondTimer.Restart();
                RequestsPerSecondCounter = 0;
            }

            Guid uuid = Guid.NewGuid();
            // Log.Verbose(uuid + " GamepadProcessor.GetState in a");

            // Create the default state to modify
            if (true)//CurrentState == null)
            {
                CurrentState = new DualShockState() { Battery = 255 };
            }

            if (!ProcessUtil.IsRemotePlayInForeground())
            {
                Log.Verbose(uuid + "GamepadProcessor.GetState return null");
                Utility.ShowCursor(true);
                return null;
            }

            //Stopwatch OnReceiveDataTimer = new Stopwatch();
            //OnReceiveDataTimer.Start();

            // Log.Verbose(uuid + " GamepadProcessor.GetState in b");

            HandleButtonPressed();
            HandleAimToggle();

            // if onscreenSticksUsed we probably not need to hand mouse cursor
            bool onscreenSticksUsed = HandleOnscreenSticks();
            if (!onscreenSticksUsed)
            {
                HandleMouseCursor();
            }

            MouseWheelProcessor.Process(CurrentState);

            //Log.Verbose(uuid + " GamepadProcessor.GetState out " + DualShockStateToString(ref CurrentState));

            //Log.Verbose(uuid + " GamepadProcessor.GetState OnReceiveDataTimer {0} ms ", + OnReceiveDataTimer.ElapsedMilliseconds);
            //OnReceiveDataTimer.Stop();

            Log.Verbose(uuid + "GamepadProcessor.GetState return THING");
            return CurrentState;
        }
    }
}
