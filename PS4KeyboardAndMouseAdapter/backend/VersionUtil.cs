using System;
using System.Reflection;

namespace PS4KeyboardAndMouseAdapter
{
    class VersionUtil
    {
        public static string GetVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }
    }
}
