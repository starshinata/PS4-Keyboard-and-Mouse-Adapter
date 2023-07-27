using GreeterProtos;
using Grpc.Core;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using PS4RemotePlayInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using static GreeterProtos.GreetingService;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Remote
{

    public class GreeterServiceImpl : GreetingServiceBase
    {

        public override Task<UpdateResponse> Greeting(UpdateRequest request, ServerCallContext context)
        {

            Console.WriteLine($"Greeter Service Received: ");

            DualShockProto dualShockProto = null;
            if (ApplicationSettings.GetInstance().EmulationMode == EmulationConstants.ONLY_PROCESS_INJECTION)
            {
                dualShockProto = GetDualShockStateAsProto();
            }

            // TODO
            // do i need to pass EmulationMode ?
            // EmulationMode = ApplicationSettings.GetInstance().EmulationMode,

            var x = new UpdateResponse
            {
                ProcessId = InstanceSettings.GetInstance().GetRemotePlayProcess().Id,
                IsToolBarVisible = UtilityData.IsToolBarVisible,
                EmulationMode = 999,
                DualShockProto = dualShockProto
            };

            return Task.FromResult(x);
        }

        private DualShockState GetDualShockState()
        {
            var window = Application.Current.MainWindow;
            if (window == null) { return null; }

            var dataContext = window.DataContext;
            if (dataContext == null) { return null; }

            var mainViewModel = (MainViewModel)dataContext;
            if (mainViewModel.GamepadProcessor == null) { return null; }

            return mainViewModel.GamepadProcessor.GetState();
        }

        private DualShockProto GetDualShockStateAsProto()
        {
            DualShockState dualShockState = GetDualShockState();
            if (dualShockState == null) { return null; }

            DualShockProto proto = new DualShockProto();

            proto.L1 = dualShockState.L1;
            proto.L2 = dualShockState.L2;
            proto.L3 = dualShockState.L3;
            proto.LX = dualShockState.LX;
            proto.LY = dualShockState.LY;

            proto.R1 = dualShockState.R1;
            proto.R2 = dualShockState.R2;
            proto.R3 = dualShockState.R3;
            proto.RX = dualShockState.RX;
            proto.RY = dualShockState.RY;

            proto.Triangle = dualShockState.Triangle;
            proto.Circle = dualShockState.Circle;
            proto.Cross = dualShockState.Cross;
            proto.Square = dualShockState.Square;

            proto.DPadUp = dualShockState.DPad_Up;
            proto.DPadDown = dualShockState.DPad_Down;
            proto.DPadLeft = dualShockState.DPad_Left;
            proto.DPadRight = dualShockState.DPad_Right;

            proto.Share = dualShockState.Share;
            proto.Options = dualShockState.Options;

            return proto;
        }
    }
}
