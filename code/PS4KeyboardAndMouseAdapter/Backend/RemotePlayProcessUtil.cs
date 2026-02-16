using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class RemotePlayProcessUtil
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static bool IsInForeground(Process process)
        {
            try
            {
                if (process == null)
                    return false;

                Log.Verbose(" ProcessUtil.IsInForeground process a " + process);

                if (process.MainWindowHandle == null)
                    return false;

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
            catch (Exception e)
            {
                ExceptionLogger.LogException("ProcessUtil.IsInForeground failed", e);
                return false;
            }
        }

        public static bool IsRemotePlayInForeground()
        {
            return IsInForeground(InstanceSettings.GetInstance().GetRemotePlayProcess());
        }

    }
}
