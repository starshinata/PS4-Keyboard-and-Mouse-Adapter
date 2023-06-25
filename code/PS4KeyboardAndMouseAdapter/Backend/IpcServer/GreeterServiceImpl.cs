using GreeterProtos;
using Grpc.Core;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using PS4RemotePlayInjection;
using System;
using System.Threading.Tasks;
using static GreeterProtos.GreetingService;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Remote
{

    public class GreeterServiceImpl : GreetingServiceBase
    {
        public override Task<HelloResponse> Greeting(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Greeter Service Received: ");

            var x = new HelloResponse
            {
                ProcessId = InstanceSettings.GetInstance().GetRemotePlayProcess().Id,
                IsToolBarVisible = UtilityData.IsToolBarVisible
            };

            return Task.FromResult(x);
        }
    }
}
