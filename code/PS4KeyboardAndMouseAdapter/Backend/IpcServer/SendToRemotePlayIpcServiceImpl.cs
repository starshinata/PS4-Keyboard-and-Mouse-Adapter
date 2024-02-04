using GreeterProtos;
using Grpc.Core;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using PS4RemotePlayInjection;
using Serilog;
using System.Threading.Tasks;
using System.Windows;
using static GreeterProtos.GreetingService;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Remote
{

    public class SendToRemotePlayIpcServiceImpl : GreetingServiceBase
    {

        public override Task<UpdateResponse> Greeting(UpdateRequest request, ServerCallContext context)
        {

            Log.Information($"Greeter Service Received: ");

            
            DualShockProto dualShockProto = null;
            if (ApplicationSettings.GetInstance().EmulationMode == EmulationConstants.ONLY_PROCESS_INJECTION)
            {
                dualShockProto = GetDualShockStateAsProto();
            }

            // we can argue we dont need to send emulationMode
            // if we say dualShockProto === null means no emulation
            // BUT what should we send when dualShockProto is null because the user isnt focused on remoteplay ?
            int emulationMode = ApplicationSettings.GetInstance().EmulationMode;

            UpdateResponse x = new UpdateResponse
            {
                ProcessId = InstanceSettings.GetInstance().GetRemotePlayProcess().Id,
                IsToolBarVisible = UtilityData.IsToolBarVisible,
                EmulationMode = emulationMode,
                DualShockProto = dualShockProto
            };

            return Task.FromResult(x);
        }

        private DualShockState GetDualShockState()
        {

            // TODO this smells
            // refactor GamepadProcessor to be called without needing to get it via MainViewModel

            return InstanceSettings.GetInstance().DualShockState;
            
            
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

            proto.DPadUp = dualShockState.DPadUp;
            proto.DPadDown = dualShockState.DPadDown;
            proto.DPadLeft = dualShockState.DPadLeft;
            proto.DPadRight = dualShockState.DPadRight;

            proto.Share = dualShockState.Share;
            proto.Options = dualShockState.Options;

            return proto;
        }
    }
}
