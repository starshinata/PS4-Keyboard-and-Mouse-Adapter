using System;
using Grpc.Core;
using GreeterProtos;
using System.Threading.Tasks;

namespace PS4RemotePlayInjection
{
    public class IpcClient
    {

        const string DefaultHost = "localhost";
        const int Port = 50051;

        private Channel channel;
        private GreetingService.GreetingServiceClient client;


        public async Task Setup()
        {
            try
            {
                var channelTarget = $"{DefaultHost}:{Port}";

                Console.WriteLine($"Target: {channelTarget}");

                //TODO insecure?!
                // Create a channel
                channel = new Channel(channelTarget, ChannelCredentials.Insecure);

                // Create a client with the channel
                client = new GreetingService.GreetingServiceClient(channel);

                // Create a request
                var request = new HelloRequest
                {
                    Name = "Mete - on C#",
                    Age = 34,
                    Sentiment = Sentiment.Happy
                };

                // Send the request
                Console.WriteLine("GreeterClient sending request");
                var response = await client.GreetingAsync(request);

                Console.WriteLine("GreeterClient received response: " + response.Greeting);
            }
            finally { await Shutdown(); }
        }

        //TODO do proper shutdown
        private async Task Shutdown()
        {
            // Shutdown
            await channel.ShutdownAsync();

        }
    }
}

