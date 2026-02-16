using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotePlayInjected.Injected
{

    public class ThreadRpcUpdateListener
    {
        private bool keepWorking = true;
        private IpcKmaClient ipcClient;

        public ThreadRpcUpdateListener()
        {
            Log.Information("IPC a");
            ipcClient = new IpcKmaClient();
            Log.Information("IPC b");
            ipcClient.Setup();
            Log.Information("IPC c");
        }

        public async void DoWork()
        {
            while (keepWorking)
            {
                Log.Information("awake to work");
                await ipcClient.DoWork();
                Thread.Sleep(100);
            }
        }

        //TODO make sure i am called from the APP shutdown method
        public async Task ShutdownAsync()
        {
            Log.Information("ShutdownAsync");
            keepWorking = false;
            await ipcClient.Shutdown();
        }

    }
}
