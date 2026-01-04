using NuGet.Versioning;
using Serilog;
using System;
using System.Reflection;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class VersionUtil
    {
        public static string GetVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public static SemanticVersion GetSemanticVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            SemanticVersion version =  new SemanticVersion(v.Major, v.Minor, v.Build);
            //TODO what does this print ?!
            Log.Information("version" + version);
            return version;
        }

        public static string GetVersionWithBuildDate()
        {
            return $"{GetVersion()}.{Properties.Resources.BuildDate}".Trim();
        }
    }
}
