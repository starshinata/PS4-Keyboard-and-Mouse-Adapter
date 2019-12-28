using System;
using System.Diagnostics;
using System.Numerics;
using PS4RemotePlayInterceptor;
using SFML.System;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter
{
    public class MainViewModel
    {
        // Constants
        private string TARGET_PROCESS_NAME = "RemotePlay";
        private string INJECT_DLL_NAME = "PS4RemotePlayInjection.dll";

        public DualShockState CurrentState { get; private set; }
        public bool EnableMouseInput { get; set; } = true;
        private Stopwatch mouseIdleTimer = Stopwatch.StartNew();
        public double MouseSensitivity { get; set; } = 3;
        public Vector2i MouseDirection { get; set; }
        public Vector2i anchor { get; set; } = new Vector2i(900, 500);
        

        public MainViewModel()
        {
            Injector.Inject(TARGET_PROCESS_NAME, INJECT_DLL_NAME);

            Injector.Callback += OnReceiveData;

        }

        public void OnReceiveData(ref DualShockState state)
        {

            // Create the default state to modify
            if (true)//CurrentState == null)
            {
                CurrentState = new DualShockState() { Battery = 255 };
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                CurrentState.LX = 0;

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                CurrentState.LX = 255;

            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                CurrentState.LY = 0;

            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                CurrentState.LY = 255;

            if (Keyboard.IsKeyPressed(Keyboard.Key.F))
                CurrentState.Triangle = true;

            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
                CurrentState.Circle = true;

            if (Keyboard.IsKeyPressed(Keyboard.Key.V))
                CurrentState.Cross = true;

            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                CurrentState.Square = true;

            // Mouse Input
            if (EnableMouseInput)
            {
                var checkState = new DualShockState();

                // Left mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    CurrentState.R2 = 255;
                }
                else
                {
                    CurrentState.R2 = 0;
                }

                // Right mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Right))
                {
                    CurrentState.L2 = 255;
                }
                else
                {
                    CurrentState.L2 = 0;
                }

                // Middle mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Middle))
                {
                    CurrentState.L1 = true;
                }
                else
                {
                    CurrentState.L1 = false;
                }

                var mousePos = Mouse.GetPosition();

                // Mouse moved
                if (mousePos != anchor)
                {
                    MouseDirection = mousePos - anchor;
                    mouseIdleTimer.Restart();
                    Mouse.SetPosition(anchor);
                }

                // Mouse idle
                else
                {
                    if (mouseIdleTimer.ElapsedMilliseconds > 50)
                    {
                        MouseDirection = new Vector2i(0, 0);
                    }
                }

                //string analogProperty = MouseMovementAnalog == AnalogStick.Left ? "L" : "R";

                var direction = new Vector2((float)MouseDirection.X, (float)MouseDirection.Y);
                //Console.WriteLine(direction.Length());
                int maxLength = 1;
                var length = direction.Length() > maxLength ? maxLength : direction.Length();
                direction = Vector2.Normalize(direction);
                //Console.Write("\r{0}".PadRight(8) + "{1}".PadRight(8), scaledX, scaledY);

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

                Console.WriteLine("{0:000}, {1:000}", scaledX, scaledY);
                //if(scaledX != 128 && scaledY != 128) Console.WriteLine("{0}  {1}", MouseSpeedX, MouseSpeedY);

                // Invoke callback
                //OnMouseAxisChanged?.Invoke(scaledX, scaledY);
            }

            // Assign the state
            state = CurrentState;
        }

        double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}
