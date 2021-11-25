using Pizza;
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

        public static void Dump()
        {
            try
            {
                Log.Debug("");
                Log.Debug("DebugDump");
                Log.Debug("DebugDump");
                Log.Debug("DebugDump");

                Log.Debug("");

                Log.Debug("OS " + GetOsVersion.Get());
                Log.Debug("Is64BitOperatingSystem " + Environment.Is64BitOperatingSystem);

                Dump_RemotePlay();

                Dump_ApplicationFolder();
                Dump_CurrentCulture();
                Dump_EnvironmentVariables();

                Dump_ListProcesses();
                Dump_ListServices();
                HidFacade.get();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("DebugDump.Dump failed " , ex);
            }
        }

        private static void Dump_ApplicationFolder()
        {
            Log.Debug("");

            try
            {
                string applicationDirectory = Path.GetFullPath(".");
                string[] files = Directory.GetFiles(applicationDirectory);
                foreach (string file in files)
                {
                    Log.Debug("local file " + file);
                }
            }
            catch (Exception ex)
            {
                Log.Error("DebugDump.Dump_ApplicationFolder failed " + ex.Message);
            }
        }

        private static void Dump_CurrentCulture()
        {


            Log.Debug("");
            CultureInfo cc = CultureInfo.CurrentCulture;
            Log.Debug("Default Language Info:");
            Log.Debug("* Name: " + cc.Name);
            Log.Debug("* Display Name: " + cc.DisplayName);
            Log.Debug("* English Name: " + cc.EnglishName);
            Log.Debug("* 2-letter ISO Name: " + cc.TwoLetterISOLanguageName);
            Log.Debug("* 3-letter ISO Name: " + cc.ThreeLetterISOLanguageName);
            Log.Debug("* 3-letter Win32 API Name: " + cc.ThreeLetterWindowsLanguageName);
        }

        private static void Dump_EnvironmentVariables()
        {
            Log.Debug("");

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
                Log.Debug("env " + e.Key + "=" + e.Value);
            }
        }

        private static void Dump_ListProcesses()
        {
            Log.Debug("");

            Process[] processCollection = Process.GetProcesses();
            Array.Sort(processCollection, new ProcessComparerByName());
            foreach (Process p in processCollection)
            {
                ProcessCommandLine.Retrieve(p, out string commandLine, ProcessCommandLine.Parameter.CommandLine);
                Log.Debug("{ 'processName': '" + p.ProcessName +
                    "', 'pid': '" + p.Id +
                    "', 'mainWindowTitle': '" + p.MainWindowTitle +
                    "', 'commandLine': '" + commandLine + "' }");
            }
        }

        private static void Dump_ListServices()
        {
            Log.Debug("");

            System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
            Array.Sort(services, new ServiceComparerByName());
            foreach (System.ServiceProcess.ServiceController service in services)
            {
                Log.Debug("{ 'ServiceName':'" + service.ServiceName + "', 'DisplayName': '" + service.DisplayName + "', 'Status':'" + service.Status + "' }");
            }
        }

        private static void Dump_RemotePlay()
        {
            Log.Debug("");
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
                Log.Error("DebugDump.Dump_RemotePlay failed" + ex.Message);
            }

            Log.Debug("remote play version " + remotePlayVersion);
        }
    }
}
