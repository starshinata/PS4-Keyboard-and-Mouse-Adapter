using Serilog;
using System;
using System.Diagnostics;

namespace PS4KeyboardAndMouseAdapter.backend.DebugLogging
{
    class GetOsVersion
    {
        private static readonly ILogger StaticLogger = Log.ForContext(typeof(GetOsVersion));

        public static string Get()
        {
            // 2021.05.20 pancakeslp
            // originally used `Environment.OSVersion`
            // but it is trash as it will report windows 8 while on windows 10
            // this is because we manifested for windows 8 (aka what compatibility level we set)
            // see https://docs.microsoft.com/en-us/windows/win32/sysinfo/getting-the-system-version

            // Sourced from 
            // https://stackoverflow.com/questions/1469764/run-command-prompt-commands


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";

            startInfo.RedirectStandardOutput = true;

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            // Important is that the argument begins with /C 
            // - "Carries out the command specified by the string and then terminates."
            // `ver` is a command for version information on windows
            // `winver` if you want a gui
            startInfo.Arguments = "/C ver";

            try
            {
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();
                return process.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                StaticLogger.Error("GetOsVersion.Get() failed: " + e.Message);
                StaticLogger.Error(e.StackTrace);
                return "unknown";
            }
        }

    }
}
