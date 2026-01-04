using System.IO;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class PathUtil
    {
        public static string GetApplicationPath()
        {
            return Path.GetFullPath(Path.GetFullPath("."));
        }
    }
}
