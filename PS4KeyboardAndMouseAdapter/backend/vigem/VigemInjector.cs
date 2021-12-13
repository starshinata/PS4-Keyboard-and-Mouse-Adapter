using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

using Pizza.backend;

using Serilog;
using System;
using System.Threading.Tasks;

namespace PS4KeyboardAndMouseAdapter
{
    using PVIGEM_TARGET = IntPtr;

    //thanks https://github.com/Ryochan7/DS4Windows/blob/2f1d8d353253d7f2ab7edadd70888658f1dacd7c/DS4Windows/DS4Control/ControlService.cs

    public class VigemInjector
    {

        public static ushort VENDOR_ID = (ushort)HidConstants.VENDOR_ID_SONY;
        public static ushort PRODUCT_ID = (ushort)HidConstants.PRODUCT_ID_PS4_CONTROLLER_A;

        private ViGEmClient vigemClient;
        private IDualShock4Controller controller;
        private bool connected;

        protected PVIGEM_TARGET NativeHandle { get; set; }

        public void ConvertandSendReport(DS4State state, int device)
        {
            if (!connected) return;

            controller.ResetReport();
            ushort tempButtons = 0;
            DualShock4DPadDirection tempDPad = DualShock4DPadDirection.None;
            ushort tempSpecial = 0;

            unchecked
            {
                if (state.Share) tempButtons |= DualShock4Button.Share.Value;
                if (state.L3) tempButtons |= DualShock4Button.ThumbLeft.Value;
                if (state.R3) tempButtons |= DualShock4Button.ThumbRight.Value;
                if (state.Options) tempButtons |= DualShock4Button.Options.Value;

                if (state.DpadUp && state.DpadRight) tempDPad = DualShock4DPadDirection.Northeast;
                else if (state.DpadUp && state.DpadLeft) tempDPad = DualShock4DPadDirection.Northwest;
                else if (state.DpadUp) tempDPad = DualShock4DPadDirection.North;
                else if (state.DpadRight && state.DpadDown) tempDPad = DualShock4DPadDirection.Southeast;
                else if (state.DpadRight) tempDPad = DualShock4DPadDirection.East;
                else if (state.DpadDown && state.DpadLeft) tempDPad = DualShock4DPadDirection.Southwest;
                else if (state.DpadDown) tempDPad = DualShock4DPadDirection.South;
                else if (state.DpadLeft) tempDPad = DualShock4DPadDirection.West;

                /*if (state.DpadUp) tempDPad = (state.DpadRight) ? DualShock4DPadValues.Northeast : DualShock4DPadValues.North;
                if (state.DpadRight) tempDPad = (state.DpadDown) ? DualShock4DPadValues.Southeast : DualShock4DPadValues.East;
                if (state.DpadDown) tempDPad = (state.DpadLeft) ? DualShock4DPadValues.Southwest : DualShock4DPadValues.South;
                if (state.DpadLeft) tempDPad = (state.DpadUp) ? DualShock4DPadValues.Northwest : DualShock4DPadValues.West;
                */

                if (state.L1) tempButtons |= DualShock4Button.ShoulderLeft.Value;
                if (state.R1) tempButtons |= DualShock4Button.ShoulderRight.Value;
                //if (state.L2Btn) tempButtons |= DualShock4Buttons.TriggerLeft;
                //if (state.R2Btn) tempButtons |= DualShock4Buttons.TriggerRight;
                if (state.L2 > 0) tempButtons |= DualShock4Button.TriggerLeft.Value;
                if (state.R2 > 0) tempButtons |= DualShock4Button.TriggerRight.Value;

                if (state.Triangle) tempButtons |= DualShock4Button.Triangle.Value;
                if (state.Circle) tempButtons |= DualShock4Button.Circle.Value;
                if (state.Cross) tempButtons |= DualShock4Button.Cross.Value;
                if (state.Square) tempButtons |= DualShock4Button.Square.Value;
                if (state.PS) tempSpecial |= DualShock4SpecialButton.Ps.Value;
                if (state.TouchButton) tempSpecial |= DualShock4SpecialButton.Touchpad.Value;
                //controller.SetButtonsFull(tempButtons);
                //controller.SetSpecialButtonsFull((byte)tempSpecial);
                controller.SetDPadDirection(tempDPad);
                //report.Buttons = (ushort)tempButtons;
                //report.SpecialButtons = (byte)tempSpecial;
            }

            // controller.LeftTrigger = state.L2;
            //   controller.RightTrigger = state.R2;

            controller.SubmitReport();
        }


        public void listen()
        {
            Task.Factory.StartNew(() =>
            {
                GamepadProcessor gp = new GamepadProcessor();
                while (true)
                {
                    gp.GetState();
                    
                    //TODO make this config, and more than 10 times a second
                    System.Threading.Thread.Sleep(100);
                }
            });
        }

        public void start()
        {
            start_ViGEm();
            start_controller();
            listen();
        }

        private void start_controller()
        {
            controller = vigemClient.CreateDualShock4Controller(VENDOR_ID, PRODUCT_ID);
            controller.Connect();
            connected = true;
        }

        private void start_ViGEm()
        {

            vigemClient = new ViGEmClient();
            System.Threading.Thread.Sleep(1000);
        }

        public void sendX()
        {
            Log.Information("MouseSettingsSimpleControl.sendX IN");
            controller.SetButtonState(DualShock4Button.Cross, true);
            controller.SubmitReport();

            Log.Information("MouseSettingsSimpleControl.sendX sleep IN");
            System.Threading.Thread.Sleep(1000);
            Log.Information("MouseSettingsSimpleControl.sendX sleep OUT");

            controller.SetButtonState(DualShock4Button.Cross, false);
            controller.SubmitReport();
            Log.Information("MouseSettingsSimpleControl.sendX OUT");
        }

        public void stop()
        {
            stop_controller();
            stop_ViGEm();
        }

        private void stop_controller()
        {
            controller.Disconnect();
        }

        private void stop_ViGEm()
        {
            if (vigemClient != null)
            {
                vigemClient.Dispose();
                vigemClient = null;
            }
        }

    }
}
