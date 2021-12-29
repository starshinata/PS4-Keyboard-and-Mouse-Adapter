using System;
using System.Runtime.InteropServices;

namespace PS4KeyboardAndMouseAdapter.Dll
{

    public class User32
    {

        // dont assume you can use C#'s Rect it uses Doubles, and this api returns Ints
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        // Define the FindWindow API function.
        [DllImport("user32.dll", EntryPoint = "FindWindow",
            SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

    }
}
