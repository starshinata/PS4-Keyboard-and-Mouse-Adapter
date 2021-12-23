using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

using PS4RemotePlayInterceptor;

namespace Pizza.backend.vigem
{
    public class GamepadConverter
    {

        // DS4State from Nefarius.ViGEm.Client.Targets
        public static void ConvertandSendReport(IDualShock4Controller controller, DS4State state)
        {
            // 2021.12.21 pancakeslp
            // ` controller.ResetReport(); `
            // it might make sense to reset the controller/report
            // but sometimes the report might auto submit, resulting in what looks like a twitch 
            //
            // this is most easilly observed with having the ADS toggle on,
            // and then ADSing and taking your hand off the mouse 

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

                controller.SetDPadDirection(tempDPad);
            }

            controller.SubmitReport();
        }

        // DualShockState from PS4RemotePlayInjection
        public static void ConvertandSendReport(IDualShock4Controller controller, DualShockState state)
        {

            // 2021.12.21 pancakeslp
            // ` controller.ResetReport(); `
            // it might make sense to reset the controller/report
            // but sometimes the report might auto submit, resulting in what looks like a twitch 
            //
            // this is most easilly observed with having the ADS toggle on,
            // and then ADSing and taking your hand off the mouse 

            //Left 
            controller.SetButtonState(DualShock4Button.ShoulderLeft, state.L1);
            SetTriggerLeft(controller, state.L2);
            controller.SetButtonState(DualShock4Button.ThumbLeft, state.L3);

            DualShock4DPadDirection tempDPad = DualShock4DPadDirection.None;
            if (state.DPad_Up && state.DPad_Right) tempDPad = DualShock4DPadDirection.Northeast;
            else if (state.DPad_Up && state.DPad_Left) tempDPad = DualShock4DPadDirection.Northwest;
            else if (state.DPad_Up) tempDPad = DualShock4DPadDirection.North;
            else if (state.DPad_Right && state.DPad_Down) tempDPad = DualShock4DPadDirection.Southeast;
            else if (state.DPad_Right) tempDPad = DualShock4DPadDirection.East;
            else if (state.DPad_Down && state.DPad_Left) tempDPad = DualShock4DPadDirection.Southwest;
            else if (state.DPad_Down) tempDPad = DualShock4DPadDirection.South;
            else if (state.DPad_Left) tempDPad = DualShock4DPadDirection.West;
            controller.SetDPadDirection(tempDPad);

            controller.SetAxisValue(DualShock4Axis.LeftThumbX, state.LX);
            controller.SetAxisValue(DualShock4Axis.LeftThumbY, state.LY);


            //middle
            controller.SetButtonState(DualShock4Button.Options, state.Options);
            controller.SetButtonState(DualShock4Button.Share, state.Share);

            controller.SetButtonState(DualShock4SpecialButton.Ps, state.PS);
            controller.SetButtonState(DualShock4SpecialButton.Touchpad, state.TouchButton);


            //Right
            controller.SetButtonState(DualShock4Button.ShoulderRight, state.R1);
            SetTriggerRight(controller, state.R2);
            controller.SetButtonState(DualShock4Button.ThumbRight, state.R3);

            controller.SetButtonState(DualShock4Button.Circle, state.Circle);
            controller.SetButtonState(DualShock4Button.Cross, state.Cross);
            controller.SetButtonState(DualShock4Button.Square, state.Square);
            controller.SetButtonState(DualShock4Button.Triangle, state.Triangle);

            controller.SetAxisValue(DualShock4Axis.RightThumbX, state.RX);
            controller.SetAxisValue(DualShock4Axis.RightThumbY, state.RY);

            controller.SubmitReport();
        }


        public static bool GetTriggerPressedFromByte(byte bite)
        {
            if (bite > 0) { return true; }
            return false;
        }

        public static void SetTriggerLeft(IDualShock4Controller controller, byte bite)
        {
            // triggers need to be set twice as per https://github.com/ViGEm/ViGEm.NET/issues/14
            bool triggerBool = GetTriggerPressedFromByte(bite);
            controller.SetButtonState(DualShock4Button.TriggerLeft, triggerBool);
            controller.SetSliderValue(DualShock4Slider.LeftTrigger, bite);
        }

        public static void SetTriggerRight(IDualShock4Controller controller, byte bite)
        {
            // triggers need to be set twice as per https://github.com/ViGEm/ViGEm.NET/issues/14
            bool triggerBool = GetTriggerPressedFromByte(bite);
            controller.SetButtonState(DualShock4Button.TriggerRight, triggerBool);
            controller.SetSliderValue(DualShock4Slider.RightTrigger, bite);
        }

    }
}
