using NuGet;
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
            return new SemanticVersion(v.Major, v.Minor, v.Build, v.Revision);
        }

        public static string GetVersionWithBuildDate()
        {
            return $"{GetVersion()}.{Properties.Resources.BuildDate}".Trim();
        }
    }
}
