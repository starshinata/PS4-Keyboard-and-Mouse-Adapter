using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace junk
{
    public class DllMain
    {

        static DllMain()
        {
            Console.WriteLine("DllMain static constructor");
        }


        public static bool DllMain2(int hModule, int reason, IntPtr lpReserved)
        {
            Console.WriteLine("DllMain.DllMain19");
            return false;
        }
    }
}
