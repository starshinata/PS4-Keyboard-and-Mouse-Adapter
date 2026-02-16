using System;
using Grpc.Core;
using System.Threading.Tasks;
using PS4RemotePlayInjection.Injected.ControllerState;
using GreeterProtos47;

namespace Pizza.PS4RemotePlayDll.Stuff
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

            //TODO make all these console methods LOG methods

            Console.WriteLine("IpcClient DoWork");
            // Create a request
            var request = new UpdateRequest();

            // Send the request
            Console.WriteLine("GreeterClient sending request");
            var response = await client.GreetingAsync(request);


            Console.WriteLine("GreeterClient current process: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            Console.WriteLine("GreeterClient received response.Stamp: " + response.Stamp);
            // Console.WriteLine("GreeterClient received response.IsToolBarVisible: " + response.IsToolBarVisible);
            // Console.WriteLine("GreeterClient received response.EmulationMode: " + response.EmulationMode);
            // Console.WriteLine("GreeterClient received response.DualShockProto: " + response.DualShockProto);

            UtilityData.IsToolBarVisible = response.IsToolBarVisible;
            UtilityData.DualShockState = ProtoToController.GetProtoAsDualShockState(response.DualShockProto);

            Console.WriteLine("gawd UtilityData.IsToolBarVisible: " + UtilityData.IsToolBarVisible);
            Console.WriteLine("gawd UtilityData.DualShockState");

            if (UtilityData.DualShockState != null)
            {
                Console.WriteLine(UtilityData.DualShockState.ToString());
                Console.WriteLine("gawd UtilityData.DualShockState.X");
                Console.WriteLine(UtilityData.DualShockState.Cross);

            }
            else
            {
                Console.WriteLine("null");

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

