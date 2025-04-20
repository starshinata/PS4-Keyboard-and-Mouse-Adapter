using System.IO;

namespace Pizza.Common
{
    public class PathUtil
    {
        public static string GetApplicationPath()
        {
            return Path.GetFullPath(Path.GetFullPath("."));
        }
    }
}
