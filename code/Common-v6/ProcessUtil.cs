using System.Diagnostics;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class ProcessUtil
    {
        public static Process FindProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                return process;
            }

            return null;

        }

    }
}
