using System;
using Grpc.Core;
using GreeterProtos;
using System.Threading.Tasks;
using PS4RemotePlayInjection.ControllerState;

namespace PS4RemotePlayInjection
{
    public class IpcKmaClient
    {

        const string DefaultHost = "localhost";
        const int Port = 50051;

        private Channel channel;
        private GreetingService.GreetingServiceClient client;

        public void Setup()
        {
            string hostAndPort = $"{DefaultHost}:{Port}";

            //TODO insecure?!
            channel = new Channel(hostAndPort, ChannelCredentials.Insecure);
            client = new GreetingService.GreetingServiceClient(channel);
        }

        public async Task DoWork()
        {
            Console.WriteLine("IpcClient DoWork");
            // Create a request
            var request = new UpdateRequest();

            // Send the request
            Console.WriteLine("GreeterClient sending request");
            var response = await client.GreetingAsync(request);

            Console.WriteLine("GreeterClient received response.ProcessId: " + response.ProcessId);
            Console.WriteLine("GreeterClient received response.IsToolBarVisible: " + response.IsToolBarVisible);
            Console.WriteLine("GreeterClient received response.EmulationMode: " + response.EmulationMode);
            Console.WriteLine("GreeterClient received response.DualShockProto: " + response.DualShockProto);
            UtilityData.pid = response.ProcessId;
            UtilityData.IsToolBarVisible = response.IsToolBarVisible;
            UtilityData.DualShockState = ProtoToController.GetProtoAsDualShockState(response.DualShockProto);
            Console.WriteLine("UtilityData.DualShockState");
            Console.WriteLine(UtilityData.DualShockState.ToString());
            Console.WriteLine("UtilityData.DualShockState.X");
            Console.WriteLine(UtilityData.DualShockState.Cross);
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

