using System;
using System.Runtime.InteropServices;

namespace PS4KeyboardAndMouseAdapter
{
    class WindowUtil
    {

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

        public static void ResetWindowLocation(string windowTitle)
        {
            Console.WriteLine("ResetWindowLocation");
            // Find (the first-in-Z-order) Notepad window.
            IntPtr hWnd = FindWindow(null, windowTitle);

            // If found, position it.
            if (hWnd != IntPtr.Zero)
            {
                
                     Console.WriteLine("ResetWindowLocation() found");

                // Move the window to (0,0) without changing its size or position
                // in the Z order.
                SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
            }
        }
    }
}
