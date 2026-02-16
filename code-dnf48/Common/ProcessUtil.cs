

namespace Pizza.Common
{


    public class ProcessUtil
    {
        public static System.Diagnostics.Process FindProcess(string processName)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
            foreach (System.Diagnostics.Process process in processes)
            {
                return process;
            }

            return null;
        }
    }
}
