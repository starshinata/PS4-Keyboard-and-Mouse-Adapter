using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PS4KeyboardAndMouseAdapter
{
    class ProcessUtil
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static bool IsInForeground(Process process)
        {
            if (process == null)
                return false;

            Log.Verbose(" ProcessUtil.IsInForeground process a " + process);
            Log.Verbose(" ProcessUtil.IsInForeground process b " + process.MainWindowHandle);

            IntPtr activeWindow = GetForegroundWindow();

            Log.Verbose(" ProcessUtil.IsInForeground activeWindow " + activeWindow);

            if (activeWindow == IntPtr.Zero)
                return false;

            if (activeWindow != process.MainWindowHandle)
                return false;

            Log.Verbose(" ProcessUtil.IsInForeground return true");
            return true;
        }

        public static bool IsRemotePlayInForeground()
        {
            return IsInForeground(InstanceSettings.GetInstance().RemotePlayProcess);
        }
    }
}
