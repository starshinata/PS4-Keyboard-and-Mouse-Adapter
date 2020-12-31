using System;
using PS4KeyboardAndMouseAdapter.Dll;
using Serilog;
using SFML.System;

namespace PS4KeyboardAndMouseAdapter
{

    public class MouseAnchor
    {
        // Anchor is relative to the top left of the primary monitor
        private static Vector2i AnchorFallback { get; set; } = new Vector2i(900, 500);

        private static readonly ILogger StaticLogger = Log.ForContext(typeof(MouseAnchor));

        public static Vector2i CalculateAnchor()
        {
            IntPtr target_hwnd = User32.FindWindowByCaption(IntPtr.Zero, RemotePlayConstants.WINDOW_NAME);
            if (target_hwnd == IntPtr.Zero)
            {
                Console.WriteLine("Could not find a window with the title - " + RemotePlayConstants.WINDOW_NAME);
                StaticLogger.Information("Could not find a window with the title - " + RemotePlayConstants.WINDOW_NAME);
                return AnchorFallback;
            }

            RECT RectResult = new RECT();
            bool success = User32.GetWindowRect(target_hwnd, ref RectResult);
            if (!success)
            {
                Console.WriteLine("using AnchorFallback - GetWindowRect failed");
                StaticLogger.Information("using AnchorFallback - GetWindowRect failed");
                return AnchorFallback;
            }

            if (RectResult.Top == 0 && RectResult.Bottom == 0 &&
                    RectResult.Left == 0 && RectResult.Right == 0)
            {
                Console.WriteLine("using AnchorFallback - empty RECT");
                StaticLogger.Information("using AnchorFallback -empty RECT");
                return AnchorFallback;
            }

            return new Vector2i(
                GetMiddle(RectResult.Left, RectResult.Right),
                GetMiddle(RectResult.Top, RectResult.Bottom));
        }

        private static int GetMiddle(int small, int big)
        {
            return ((big - small) / 2) + small;
        }

        private static void printRect(RECT R)
        {
            Console.WriteLine("R.Top    " + R.Top);
            Console.WriteLine("R.Bottom " + R.Bottom);
            Console.WriteLine("R.Left   " + R.Left);
            Console.WriteLine("R.Right  " + R.Right);
        }

    }
}