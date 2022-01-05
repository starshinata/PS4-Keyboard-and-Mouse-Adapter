using System;
using System.Runtime.InteropServices;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class WindowUtil
    {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

        public static void ResetWindowLocation(string windowTitle)
        {
            // Find (the first-in-Z-order) window.
            IntPtr hWnd = FindWindow(null, windowTitle);

            // If found, position it.
            if (hWnd != IntPtr.Zero)
            {
                // Move the window to (0,0) without changing its size or
                // position in the Z order.
                SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            }
        }

    }
}
