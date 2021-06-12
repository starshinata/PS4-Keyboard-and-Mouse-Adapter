using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using mscoree;
using Serilog;

namespace PS4RemotePlayInjection
{
    public class Utility
    {
        public static bool IsCursorVisible = true;

        //TODO make a toggle for this
        public static bool IsToolBarVisible = false;

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        private static extern bool SetSystemCursor(IntPtr hcur, uint id);

        public enum SPIF
        {
            None = 0x00,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDCHANGE = 0x02,
            SPIF_SENDWININICHANGE = 0x02
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, SPIF fWinIni);

        public static void ShowCursor(bool show)
        {
            // pancakeslp 2020.11.19
            // these actions are expensive - it was the cause of issue https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/issues/27
            // so we should check to see if there is a difference between requested value and current value
            // then execute if there is a difference
            if (show != IsCursorVisible)
            {
                if (!show)
                {
                    // Make the cursor transparent
                    IntPtr cursor = LoadCursorFromFile("cursor.cur");
                    SetSystemCursor(cursor, 32512);
                }
                else
                {
                    // Reset to default cursor settings
                    SystemParametersInfo(0x0057, 0, null, 0);
                }

                IsCursorVisible = show;
            }
        }

        public static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static double mapcap(double x, double in_min, double in_max, double out_min, double out_max)
        {
            if (x < in_min)
                x = in_min;

            if (x > in_max)
                x = in_max;

            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static IList<AppDomain> GetAppDomains()
        {
            IList<AppDomain> _IList = new List<AppDomain>();
            IntPtr enumHandle = IntPtr.Zero;
            CorRuntimeHostClass host = new mscoree.CorRuntimeHostClass();
            try
            {
                host.EnumDomains(out enumHandle);
                object domain = null;
                while (true)
                {
                    host.NextDomain(enumHandle, out domain);
                    if (domain == null) break;
                    AppDomain appDomain = (AppDomain)domain;
                    _IList.Add(appDomain);
                }
                return _IList;
            }
            catch (Exception e)
            {
                Log.Logger.Error("GetAppDomains" + e.ToString());
                Log.Logger.Error("GetAppDomains" + e.Message);
                Console.WriteLine(e.ToString());
                return null;
            }
            finally
            {
                host.CloseEnum(enumHandle);
                Marshal.ReleaseComObject(host);
            }
        }
    }
}
