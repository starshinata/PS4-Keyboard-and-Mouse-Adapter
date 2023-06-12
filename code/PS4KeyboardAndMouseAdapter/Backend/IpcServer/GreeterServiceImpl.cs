using GreeterProtos;
using Grpc.Core;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using System;
using System.Threading.Tasks;
using static GreeterProtos.GreetingService;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Remote
{

    public class GreeterServiceImpl : GreetingServiceBase
    {
        public override Task<HelloResponse> Greeting(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Greeter Service Received: Name {request.Name} Sentiment {request.Sentiment} Age{request.Age}");
            return Task.FromResult(new HelloResponse { Greeting = "Hello " + InstanceSettings.GetInstance().GetRemotePlayProcess().Id });
        }
    }
}
