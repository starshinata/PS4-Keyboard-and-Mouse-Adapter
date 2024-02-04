using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GreeterProtos;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Remote
{
    class ServerThing
    {

        const string Host = "0.0.0.0";
        const int Port = 50051;

        private Task serverTask;
        private CancellationTokenSource tokenSource;

        public void Moooain()
        {
            // Build a server
            var server = new Server
            {
                Services = { GreetingService.BindService(new SendToRemotePlayIpcServiceImpl()) },
                Ports = { new ServerPort(Host, Port, ServerCredentials.Insecure) }
            };

            tokenSource = new CancellationTokenSource();
            serverTask = RunServiceAsync(server, tokenSource.Token);
        }

        //todo
        public void Shutdown()
        {
            tokenSource.Cancel();
            Console.WriteLine("Shutting down...");
            serverTask.Wait();
        }

        private static Task AwaitCancellation(CancellationToken token)
        {
            var taskSource = new TaskCompletionSource<bool>();
            token.Register(() => taskSource.SetResult(true));
            return taskSource.Task;
        }

        private static async Task RunServiceAsync(Server server, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Start server
            server.Start();

            await AwaitCancellation(cancellationToken);
            await server.ShutdownAsync();
        }
    }
}
