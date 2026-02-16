using Serilog;
using System.Diagnostics;
using System.Net;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class RemotePlayInstaller
    {
        public static void RunRemotePlaySetup()
        {
            string installerName = "RemotePlayInstaller.exe";

            //TODO deprecated, update with newer thing
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://remoteplay.dl.playstation.net/remoteplay/module/win/RemotePlayInstaller.exe", installerName);
            }

            Log.Information("RunRemotePlaySetup - RemotePlay installer started");
            Process installerProcess = Process.Start(installerName);
            installerProcess.EnableRaisingEvents = true;
        }
    }
}
