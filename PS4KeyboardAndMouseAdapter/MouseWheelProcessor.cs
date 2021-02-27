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

        private List<VirtualKey> LastVirtualKeys;

        public MouseWheelProcessor()
        {
            AimToggleTimer.Start();

            HookGlobalEvents = Hook.GlobalEvents();
            HookGlobalEvents.MouseWheel += HandleMouseWheel;

            MouseWheelQueue = new Queue<MouseEventArgs>();
        }

        private List<VirtualKey> GetVirtualKeys(ExtraButtons scrollAction)
        {
            List<VirtualKey> foundVirtualKeys = new List<VirtualKey>();

            if (ExtraButtons.Unknown != scrollAction)
            {
                UserSettings settings = UserSettings.GetInstance();
                if (settings.Mappings != null)
                {
                    foreach (VirtualKey virtualKey in settings.Mappings.Keys)
                    {
                        if (settings.Mappings[virtualKey].PhysicalKeys != null)
                        {
                            foreach (PhysicalKey physicalKey in settings.Mappings[virtualKey].PhysicalKeys)
                            {
                                if (physicalKey.ExtraValue == scrollAction && !foundVirtualKeys.Contains(virtualKey))
                                {
                                    foundVirtualKeys.Add(virtualKey);
                                }
                            }
                        }
                    }
                }
            }

            return foundVirtualKeys;
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            // we only want mouse wheel events when remoteplay is foreground application
            if (ProcessUtil.IsRemotePlayInForeground())
            {
                MouseWheelQueue.Enqueue(e);
            }
        }

        private void HandleVirtualKeys(DualShockState dualShockState, List<VirtualKey> virtualKeys)
        {
            if (virtualKeys != null)
            {
                foreach (VirtualKey virtualKey in virtualKeys)
                {
                    HandleVirtualKey(dualShockState, virtualKey);
                }
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
                LastVirtualKeys = null;
            }

            if (LastVirtualKeys == null)
            {
                // Queue.Count because "Dequeue" and "Peek" will block/wait until there is an event in the queue
                if (MouseWheelQueue.Count > 0)
                {
                    MouseEventArgs e = MouseWheelQueue.Dequeue();
                    ExtraButtons scrollAction = MouseWheelScrollProcessor.GetScrollAction(e);
                    LastVirtualKeys = GetVirtualKeys(scrollAction);
                    Console.WriteLine(DateTime.Now + " Process SET " + ListUtil.ListToString(LastVirtualKeys));
                    AimToggleTimer.Restart();
                }
            }

            HandleVirtualKeys(state, LastVirtualKeys);
        }
    }
}
