using System;
using System.Runtime.InteropServices;

//TODO rename
namespace PS4RemotePlayInjection
{
    public class CursorUtility
    {

        public enum SPIF
        {
            None = 0x00,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDCHANGE = 0x02,
            SPIF_SENDWININICHANGE = 0x02
        }

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        private static extern bool SetSystemCursor(IntPtr hcur, uint id);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, SPIF fWinIni);


        public static void ShowCursor(bool show)
        {
            // pancakeslp 2020.11.19
            // these actions are expensive - it was the cause of issue https://github.com/starshinata/PS4-Keyboard-and-Mouse-Adapter/issues/27
            // so we should check to see if there is a difference between requested value and current value
            // then execute if there is a difference
            if (show != UtilityData.IsCursorVisible)
            {
                if (!show)
                {
                    ShowCursorTransparent();
                }
                else
                {
                    ShowCursorReset();
                }

                UtilityData.IsCursorVisible = show;
            }
        }

        public static void ShowCursorReset()
        {
            // Reset to default cursor settings
            SystemParametersInfo(0x0057, 0, null, 0);
        }

        public static void ShowCursorTransparent()
        {
            // Make the cursor transparent
            IntPtr cursor = LoadCursorFromFile("cursor.cur");
            SetSystemCursor(cursor, 32512);
        }

    }
}
