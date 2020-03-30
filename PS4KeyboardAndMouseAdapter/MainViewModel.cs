using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PS4KeyboardAndMouseAdapter.Annotations;
using PS4RemotePlayInterceptor;
using SFML.System;
using SFML.Window;

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
        R1,
        R2,
        Options,
    }

    public class MainViewModel : INotifyPropertyChanged
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

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; }
        public Dictionary<Keyboard.Key, VirtualKey> ReverseMappings { get; }


        public void SetMapping(VirtualKey key, Keyboard.Key value)
        {
            Mappings[key] = value;
            OnPropertyChanged(nameof(Mappings));

            string json = JsonConvert.SerializeObject(Mappings, Formatting.Indented);
            File.WriteAllText("mappings.json", json);
        }

        public MainViewModel()
        {
            Injector.Inject(TARGET_PROCESS_NAME, INJECT_DLL_NAME);

            string json = File.ReadAllText("mappings.json");
            Mappings = JsonConvert.DeserializeObject<Dictionary<VirtualKey, Keyboard.Key>>(json);

            Injector.Callback += OnReceiveData;

            //ReverseMappings = Mappings.ToDictionary((i) => i.Value, (i) => i.Key);

            //Mappings.Add(VirtualKey.Left, Keyboard.Key.A);
            //Mappings.Add(VirtualKey.Right, Keyboard.Key.D);
            //Mappings.Add(VirtualKey.Up, Keyboard.Key.W);
            //Mappings.Add(VirtualKey.Down, Keyboard.Key.S);
            //Mappings.Add(VirtualKey.Triangle, Keyboard.Key.F);
            //Mappings.Add(VirtualKey.Circle, Keyboard.Key.C);
            //Mappings.Add(VirtualKey.Cross, Keyboard.Key.V);
            //Mappings.Add(VirtualKey.Square, Keyboard.Key.R);
        }

        public void OnReceiveData(ref DualShockState state)
        {
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

            // Mouse Input
            if (false)//EnableMouseInput)
            {
                var checkState = new DualShockState();

                // Left mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Left))
                    CurrentState.R2 = 255;
                else
                    CurrentState.R2 = 0;

                // Right mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Right))
                    CurrentState.L2 = 255;
                else
                    CurrentState.L2 = 0;

                // Middle mouse
                if (SFML.Window.Mouse.IsButtonPressed(Mouse.Button.Middle))
                    CurrentState.L1 = true;
                else
                    CurrentState.L1 = false;

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
                        MouseDirection = new Vector2i(0, 0);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
