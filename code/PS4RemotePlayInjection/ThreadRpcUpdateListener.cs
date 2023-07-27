using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PS4RemotePlayInjection
{

    public class ThreadRpcUpdateListener
    {
        private bool keepWorking = true;
        private IpcClient ipcClient;

        
        public ThreadRpcUpdateListener()
        {
            Log.Logger.Information("IPC a");
            ipcClient = new IpcClient();
            Log.Logger.Information("IPC b");
            ipcClient.Setup();
            Log.Logger.Information("IPC c");
        }


        public async void DoWork()
        {
            while (keepWorking)
            {
                await ipcClient.DoWork();
                Thread.Sleep(100);
            }
        }

        public async Task ShutdownAsync()
        {
            Console.WriteLine("ShutdownAsync");
            keepWorking = false;
            await ipcClient.Shutdown();
        }

    }
}
