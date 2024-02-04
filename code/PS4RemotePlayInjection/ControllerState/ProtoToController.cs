using GreeterProtos;


namespace PS4RemotePlayInjection.ControllerState
{
    public class ProtoToController
    {

        public static DualShockState GetProtoAsDualShockState(DualShockProto proto)
        {
            if (proto == null) { return null; }

            DualShockState dualShockState = new DualShockState();

            dualShockState.L1 = proto.L1;
            dualShockState.L2 = (byte)proto.L2;
            dualShockState.L3 = proto.L3;
            dualShockState.LX = (byte)proto.LX;
            dualShockState.LY = (byte)proto.LY;

            dualShockState.R1 = proto.R1;
            dualShockState.R2 = (byte)proto.R2;
            dualShockState.R3 = proto.R3;
            dualShockState.RX = (byte)proto.RX;
            dualShockState.RY = (byte)proto.RY;

            dualShockState.Triangle = proto.Triangle;
            dualShockState.Circle = proto.Circle;
            dualShockState.Cross = proto.Cross;
            dualShockState.Square = proto.Square;

            dualShockState.DPadUp = proto.DPadUp;
            dualShockState.DPadDown = proto.DPadDown;
            dualShockState.DPadLeft = proto.DPadLeft;
            dualShockState.DPadRight = proto.DPadRight;

            dualShockState.Share = proto.Share;
            dualShockState.Options = proto.Options;

            return dualShockState;

        }
    }
}
