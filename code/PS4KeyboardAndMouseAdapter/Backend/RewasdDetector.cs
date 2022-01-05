using Serilog;
using System;
using System.IO;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class RewasdDetector
    {
        // this has to be the before PATHS, or PATHs will be incomplete
        private static readonly string REWASD_SUBPATH = @"\reWASD\reWASD.exe";

        public static readonly string[] PATHS = new string[] {
            // Environment.SpecialFolder.ProgramFiles depends on the projects target platform
            // https://stackoverflow.com/questions/23304823/environment-specialfolder-programfiles-returns-the-wrong-directory
            Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\..\..\Program Files" + REWASD_SUBPATH),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + REWASD_SUBPATH,
        };

        private static readonly ILogger StaticLogger = Log.ForContext(typeof(RewasdDetector));

        public static string GetExistingRewadPath()
        {
            foreach (string path in PATHS)
            {
                StaticLogger.Information("RewasdDetector check path {0}", path);
                if (File.Exists(path))
                {
                    StaticLogger.Information("RewasdDetector found existing path {0}", path);
                    return path;
                }
            }
            StaticLogger.Information("RewasdDetector could not find an existing path");
            return null;
        }

    }
}
