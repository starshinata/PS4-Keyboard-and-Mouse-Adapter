using GreeterProtos;
using Grpc.Core;
using RemotePlayInjected.Injected.ControllerState;
using Serilog;
using System.Threading.Tasks;

namespace RemotePlayInjected.Injected
{
    public class IpcKmaClient
    {

        const string DefaultHost = "localhost";
        const int Port = 50051;

        private Channel channel;
        private GreetingService.GreetingServiceClient client;

        public void Setup()
        {
            Log.Information("IpcClient Setup 21");
            string hostAndPort = $"{DefaultHost}:{Port}";

            Log.Information("IpcClient Setup 24");
            //TODO insecure?!
            channel = new Channel(hostAndPort, ChannelCredentials.Insecure);

            Log.Information("IpcClient Setup 28");
            client = new GreetingService.GreetingServiceClient(channel);
            Log.Information("IpcClient Setup 30");
        }

        public async Task DoWork()
        {
            Log.Information("IpcClient DoWork");
            // Create a request
            var request = new UpdateRequest();

            // Send the request
            Log.Information("GreeterClient sending request");
            var response = await client.GreetingAsync(request);


            Log.Information("GreeterClient current process: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            Log.Information("GreeterClient received response.Stamp: " + response.Stamp);
            // Log.Information("GreeterClient received response.IsToolBarVisible: " + response.IsToolBarVisible);
            // Log.Information("GreeterClient received response.EmulationMode: " + response.EmulationMode);
            // Log.Information("GreeterClient received response.DualShockProto: " + response.DualShockProto);

            UtilityData.IsToolBarVisible = response.IsToolBarVisible;
            UtilityData.DualShockState = ProtoToController.GetProtoAsDualShockState(response.DualShockProto);

            Log.Information("gawd UtilityData.IsToolBarVisible: " + UtilityData.IsToolBarVisible);
            Log.Information("gawd UtilityData.DualShockState");

            if (UtilityData.DualShockState != null)
            {
                Log.Information(UtilityData.DualShockState.ToString());
                Log.Information("gawd UtilityData.DualShockState.X");
                Log.Information(""+UtilityData.DualShockState.Cross);

            }
            else
            {
                Log.Information("null");

            }
        }



        public async Task Shutdown()
        {
            if (channel != null)
            {
                await channel.ShutdownAsync();
            }
        }
    }
}

