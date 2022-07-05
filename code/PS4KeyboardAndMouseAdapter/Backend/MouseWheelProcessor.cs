using Gma.System.MouseKeyHook;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using PS4RemotePlayInterceptor;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class MouseWheelProcessor
    {
        // timer to know how long the button associated with mouse scroll has been held for
        private readonly Stopwatch MouseButtonHoldTimer;

        private readonly IKeyboardMouseEvents HookGlobalEvents;

        private readonly Queue<MouseEventArgs> MouseWheelQueue;

        private List<VirtualKey> LastVirtualKeys;

        public MouseWheelProcessor()
        {
            MouseButtonHoldTimer = new Stopwatch();
            MouseButtonHoldTimer.Start();

            MouseWheelQueue = new Queue<MouseEventArgs>();

            HookGlobalEvents = Hook.GlobalEvents();

            // adding mouse wheel handler is intentionally last
            // we only want to listen for stuff when we are ready
            HookGlobalEvents.MouseWheel += HandleMouseWheel;
        }

        private List<VirtualKey> GetVirtualKeys(ExtraButtons scrollAction)
        {
            List<VirtualKey> foundVirtualKeys = new List<VirtualKey>();

            if (ExtraButtons.Unknown != scrollAction)
            {
                UserSettingsV3 settings = UserSettingsContainer.GetInstance();
                if (settings.Mappings != null)
                {
                    foreach (Mapping mapping in settings.Mappings)
                    {

                        if (mapping != null && mapping.physicalKeys != null && mapping.virtualKeys != null)
                        {
                            foreach (PhysicalKey physicalKey in mapping.physicalKeys)
                            {

                                if (physicalKey.ExtraValue == scrollAction)
                                {
                                    foreach (VirtualKey virtualKey in mapping.virtualKeys)
                                    {

                                        if (!foundVirtualKeys.Contains(virtualKey))
                                        {
                                            foundVirtualKeys.Add(virtualKey);
                                        }
                                    }
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
            // we only want mouse wheel events when remote play is foreground application
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
            if (MouseButtonHoldTimer.ElapsedMilliseconds > UserSettingsContainer.GetInstance().MouseWheelScrollHoldDuration)
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
                    MouseButtonHoldTimer.Restart();
                }
            }

            HandleVirtualKeys(state, LastVirtualKeys);
        }
    }
}
