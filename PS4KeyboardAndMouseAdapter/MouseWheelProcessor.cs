using Gma.System.MouseKeyHook;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInterceptor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter
{
    class MouseWheelProcessor
    {
        // timer to know how long it has been since Aim button has been released
        private readonly Stopwatch AimToggleTimer = new Stopwatch();

        private readonly IKeyboardMouseEvents HookGlobalEvents;

        private readonly Queue<MouseEventArgs> MouseWheelQueue;

        private VirtualKey LastVirtualKey;

        public MouseWheelProcessor()
        {
            AimToggleTimer.Start();

            HookGlobalEvents = Hook.GlobalEvents();
            HookGlobalEvents.MouseWheel += HandleMouseWheel;

            LastVirtualKey = VirtualKey.NULL;

            MouseWheelQueue = new Queue<MouseEventArgs>();
        }

        private ExtraButtons GetScrollAction(MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Delta > 0)
                {
                    Console.WriteLine("Wheel positive");
                    return ExtraButtons.MouseWheelUp;
                }

                if (e.Delta == 0)
                {
                    Console.WriteLine("Wheel nuetral ");
                }

                if (e.Delta < 0)
                {
                    Console.WriteLine("Wheel negative");
                    return ExtraButtons.MouseWheelDown;
                }
            }

            return ExtraButtons.Unknown;
        }

        private VirtualKey GetVirtualKey(ExtraButtons scrollAction)
        {
            Console.WriteLine("scrollAction   " + scrollAction);
            if (ExtraButtons.Unknown != scrollAction)
            {
                if (scrollAction == ExtraButtons.MouseWheelUp)
                {
                    Console.WriteLine("Wheel Triangle");
                    return VirtualKey.Triangle;
                }


                if (scrollAction == ExtraButtons.MouseWheelDown)
                {
                    Console.WriteLine("Wheel X");
                    return VirtualKey.Cross;
                }
            }

            Console.WriteLine("Wheel NULL");
            return VirtualKey.NULL;
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            // we only want mouse wheel events when remoteplay is foreground application
            if (ProcessUtil.IsRemotePlayInForeground())
            {
                Console.WriteLine(DateTime.Now + string.Format("   Wheel={0:000}", e.Delta));
                MouseWheelQueue.Enqueue(e);
            }
        }

        private void HandleVirtualKey(DualShockState dualShockState, VirtualKey virtualKey)
        {
            if (VirtualKey.NULL == virtualKey)
            {
                return;
            }

            // ORDER
            // LEFT -- MIDDLE -- RIGHT of Controller

            ////////////////////////////////////////////

            //left face
            if (VirtualKey.DPadUp == virtualKey)
                dualShockState.DPad_Up = true;

            if (VirtualKey.DPadLeft == virtualKey)
                dualShockState.DPad_Left = true;

            if (VirtualKey.DPadDown == virtualKey)
                dualShockState.DPad_Down = true;

            if (VirtualKey.DPadRight == virtualKey)
                dualShockState.DPad_Right = true;

            //left stick Analog
            if (VirtualKey.LeftStickLeft == virtualKey)
                dualShockState.LX = 0;

            if (VirtualKey.LeftStickRight == virtualKey)
                dualShockState.LX = 255;

            if (VirtualKey.LeftStickUp == virtualKey)
                dualShockState.LY = 0;

            if (VirtualKey.LeftStickDown == virtualKey)
                dualShockState.LY = 255;

            //left stick Buttons
            if (VirtualKey.L1 == virtualKey)
                dualShockState.L1 = true;

            if (VirtualKey.L2 == virtualKey)
                dualShockState.L2 = 255;

            if (VirtualKey.L3 == virtualKey)
                dualShockState.L3 = true;

            ////////////////////////////////////////////

            // middle
            if (VirtualKey.Share == virtualKey)
                dualShockState.Share = true;

            if (VirtualKey.TouchButton == virtualKey)
                dualShockState.TouchButton = true;

            if (VirtualKey.Options == virtualKey)
                dualShockState.Options = true;

            if (VirtualKey.PlaystationButton == virtualKey)
                dualShockState.PS = true;


            ////////////////////////////////////////////

            //right face
            if (VirtualKey.Triangle == virtualKey)
                dualShockState.Triangle = true;

            if (VirtualKey.Circle == virtualKey)
                dualShockState.Circle = true;

            if (VirtualKey.Cross == virtualKey)
                dualShockState.Cross = true;

            if (VirtualKey.Square == virtualKey)
                dualShockState.Square = true;

            //right stick Analog
            if (VirtualKey.RightStickLeft == virtualKey)
                dualShockState.RX = 0;

            if (VirtualKey.RightStickRight == virtualKey)
                dualShockState.RX = 255;

            if (VirtualKey.RightStickUp == virtualKey)
                dualShockState.RY = 0;

            if (VirtualKey.RightStickDown == virtualKey)
                dualShockState.RY = 255;

            //right stick Buttons
            if (VirtualKey.R1 == virtualKey)
                dualShockState.R1 = true;

            if (VirtualKey.R2 == virtualKey)
                dualShockState.R2 = 255;

            if (VirtualKey.R3 == virtualKey)
                dualShockState.R3 = true;
        }

        public void Process(DualShockState state)
        {
            if (AimToggleTimer.ElapsedMilliseconds > UserSettings.GetInstance().MouseWheelScrollHoldDuration)
            {
                LastVirtualKey = VirtualKey.NULL;
            }

            if (LastVirtualKey == VirtualKey.NULL)
            {
                // Queue.Count because "Dequeue" and "Peek" will block/wait until there is an event in the queue
                if (MouseWheelQueue.Count > 0)
                {
                    MouseEventArgs e = MouseWheelQueue.Dequeue();
                    ExtraButtons scrollAction = GetScrollAction(e);
                    LastVirtualKey = GetVirtualKey(scrollAction);
                    Console.WriteLine(DateTime.Now + " Process SET " + LastVirtualKey);
                    AimToggleTimer.Restart();
                }
            }

            HandleVirtualKey(state, LastVirtualKey);
        }
    }
}
