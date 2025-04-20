using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;
using System.Diagnostics;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{

    public class RemotePlayStarter
    {

        public static void OpenRemotePlay()
        {
            Log.Information("RemotePlayStarter.OpenRemotePlay start requested-19");
            string exeLocation = ApplicationSettings.GetInstance().RemotePlayPath;
            var process = Process.Start(exeLocation);
            process.WaitForInputIdle();
            InstanceSettings.GetInstance().SetRemotePlayProcess(process);

            // TODO i think this is UI blocking sleep
            //Thread.Sleep(3100);

        }

    }
}
