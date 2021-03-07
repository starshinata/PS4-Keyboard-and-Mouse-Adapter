using PS4KeyboardAndMouseAdapter.Config;
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

            IntPtr activeWindow = GetForegroundWindow();

            if (activeWindow == IntPtr.Zero)
                return false;

            if (activeWindow != process.MainWindowHandle)
                return false;

            return true;
        }

        public static bool IsRemotePlayInForeground()
        {
            return IsInForeground(InstanceSettings.GetInstance().RemotePlayProcess);
        }
    }
}
