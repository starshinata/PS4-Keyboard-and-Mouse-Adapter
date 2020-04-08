using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PS4RemotePlayInjection
{
    public class Utility
    {
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

        public static bool ShowCursor(bool show)
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

            //TODO: getLastError check
            return true;
        }
    }
}
