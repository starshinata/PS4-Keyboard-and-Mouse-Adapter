using System.Diagnostics;
using System.IO;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class LogUtility
    {
        private static readonly string LOGS_DIRECTORY = "logs";

        public static string GetLogDirectoryAbsolute()
        {
            return Directory.GetCurrentDirectory() + @"\" + LOGS_DIRECTORY;
        }

        public static string GetLogFile()
        {
            return $"{LOGS_DIRECTORY}/log.txt";
        }

        public static void LogFileLocationOpen()
        {
            Process.Start("explorer.exe", GetLogDirectoryAbsolute());
        }
       
    }
}
