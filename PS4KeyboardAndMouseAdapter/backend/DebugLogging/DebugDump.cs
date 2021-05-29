using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace PS4KeyboardAndMouseAdapter.backend.DebugLogging
{
    class DebugDump
    {

        private static void Print(string message)
        {
            Log.Information(message);
            //Console.WriteLine(message);
        }

        public static void Dump()
        {

            Print("");
            Print("DebugDump");
            Print("DebugDump");
            Print("DebugDump");

            Print("");

            Print("OS " + GetOsVersion.Get());
            Print("Is64BitOperatingSystem " + Environment.Is64BitOperatingSystem);

            Dump_RemotePlay();

            Print("");
            CultureInfo cc = CultureInfo.CurrentCulture;
            Print("Default Language Info:");
            Print("* Name: " + cc.Name);
            Print("* Display Name: " + cc.DisplayName);
            Print("* English Name: " + cc.EnglishName);
            Print("* 2-letter ISO Name: " + cc.TwoLetterISOLanguageName);
            Print("* 3-letter ISO Name: " + cc.ThreeLetterISOLanguageName);
            Print("* 3-letter Win32 API Name: " + cc.ThreeLetterWindowsLanguageName);

            Dump_EnvironmentVariables();

            Print("");

            Process[] processCollection = Process.GetProcesses();
            Array.Sort(processCollection, new ProcessComparerByName());
            foreach (Process p in processCollection)
            {
                ProcessCommandLine.Retrieve(p, out string commandLine, ProcessCommandLine.Parameter.CommandLine);
                Print("{ 'processName': '" + p.ProcessName +
                    "', 'pid': '" + p.Id +
                    "', 'mainWindowTitle': '" + p.MainWindowTitle +
                    "', 'commandLine': '" + commandLine + "' }");
            }

            Print("");

            System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
            Array.Sort(services, new ServiceComparerByName());
            foreach (System.ServiceProcess.ServiceController service in services)
            {
                Print("{ 'ServiceName':'" + service.ServiceName + "', 'DisplayName': '" + service.DisplayName + "', 'Status':'" + service.Status + "' }");
            }
        }

        private static void Dump_EnvironmentVariables()
        {
            Print("");

            List<KeyValuePair<string, string>> envArray = new List<KeyValuePair<string, string>>();

            IDictionaryEnumerator envEnumerator = Environment.GetEnvironmentVariables().GetEnumerator();
            while (envEnumerator.MoveNext())
            {
                KeyValuePair<string, string> envKeyValuePair = new KeyValuePair<string, string>(
                    (string)envEnumerator.Key, (string)envEnumerator.Value);
                envArray.Add(envKeyValuePair);
            }

            envArray.Sort(delegate (KeyValuePair<string, string> pair1, KeyValuePair<string, string> pair2)
            {
                return pair1.Key.CompareTo(pair2.Key);
            });

            foreach (KeyValuePair<string, string> e in envArray)
            {
                Print("env " + e.Key + "=" + e.Value);
            }
        }

        private static void Dump_RemotePlay()
        {
            Print("");
            string remotePlayVersion = "unknown";

            try
            {
                string remotePlayPath = ApplicationSettings.GetInstance().RemotePlayPath;
                if (File.Exists(remotePlayPath))
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(remotePlayPath);
                    remotePlayVersion = versionInfo.FileVersion;
                }
                else
                {
                    remotePlayVersion = "unknown - remoteplay file doesnt exist";

                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("DebugDump.Dump_RemotePlay failed" + ex.Message);
            }

            Print("remote play version " + remotePlayVersion);
        }
    }
}
