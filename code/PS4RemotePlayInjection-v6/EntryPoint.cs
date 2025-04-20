using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PS4RemotePlayInjection
{
    public class EntryPoint
    {
        static EntryPoint()
        {
            Console.WriteLine("EntryPoint static constructor");
        }

        public static bool DllMain(int hModule, int reason, IntPtr lpReserved)
        {
            Console.WriteLine("EntryPoint.DllMain12");
            return false;

        }

        [ModuleInitializer]
        internal static void M1()
        {
            Console.WriteLine("EntryPoint.ModuleInitializer");
            // ...
        }

        private const uint DLL_PROCESS_DETACH = 0,
                         DLL_PROCESS_ATTACH = 1,
                         DLL_THREAD_ATTACH = 2,
                         DLL_THREAD_DETACH = 3;

        [UnmanagedCallersOnly(EntryPoint = "DllMain", CallConvs = new[] { typeof(CallConvStdcall) })]
        public static bool DllMain(IntPtr hModule, uint ul_reason_for_call, IntPtr lpReserved)
        {
            Console.WriteLine("EntryPoint.DllMain");
            switch (ul_reason_for_call)
            {
                case DLL_PROCESS_ATTACH:
                    Console.WriteLine("EntryPoint Proccess attach");
                    break;
                default:
                    Console.WriteLine("EntryPoint DEFAULT ");
                    break;
            }
            return true;
        }
    }
}
