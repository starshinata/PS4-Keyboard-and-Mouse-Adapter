using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// sourced from https://github.com/weirenxue/dll-injector-csharp/blob/master/Program.cs
namespace junk
{
    // WRX short for Wei-Ren Xue, the original author of this class
    public class WRXInjector
    {

        const int PROCESS_CREATE_PROCESS = 0x0080;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        const uint MEM_COMMIT = 0x1000;
        const uint MEM_RESERVE = 0x2000;
        const uint PAGE_EXECUTE_READWRITE = 0x0040;

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);


        public static void Inject(string dllAbsoultePath, int processId)
        {
            // TODO inline these variables
            string processName = "remoteplay (hardcoded)";
            var pid = processId;
            string dllPath = dllAbsoultePath;

            // TODO make all console statement log statements
            Console.WriteLine("2. Get the handle to the process");
            IntPtr pHandle = OpenProcess(
                PROCESS_CREATE_PROCESS |
                PROCESS_QUERY_INFORMATION |
                PROCESS_VM_OPERATION |
                PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, processId);
            if (pHandle == null || pHandle == IntPtr.Zero)
            {
                Console.WriteLine("\t**[FAILURE]  Doesn't obtain the handle to the process (" + processName + ", " + pid + ").");
                return;
            }
            Console.WriteLine("\t[SUCCESS]  The handle to the process (" + processName + ", " + pid + ")  is 0x" + pHandle.ToString("X8") + ".");

            Console.WriteLine("3. Allocate the memory for the dll path");

            IntPtr dllAddr = VirtualAllocEx(
                pHandle,
                IntPtr.Zero,
                (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))),
                MEM_RESERVE | MEM_COMMIT,
                PAGE_EXECUTE_READWRITE);
            if (dllAddr == null || dllAddr == IntPtr.Zero)
            {
                Console.WriteLine("\t**[FAILURE]  Allocate memory failed.");
                return;
            }
            Console.WriteLine("\t[SUCCESS]  Successfully allocate memory at 0x" + dllAddr.ToString("X8") + ".");

            Console.WriteLine("4. Write the dll path to the memory");
            UIntPtr bytesWritten;
            if (WriteProcessMemory(pHandle, dllAddr, Encoding.Default.GetBytes(dllPath), (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten) == false)
            {
                Console.WriteLine("\t**[FAILURE]  Failed to write the dll path into memory.");
                return;
            }
            Console.WriteLine("\t[SUCCESS]  The dll path is successfully written into the memory.");

            Console.WriteLine("5. Get address of \"LoadLibraryA\"");
            IntPtr loadLibraryAAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (loadLibraryAAddr == null || loadLibraryAAddr == IntPtr.Zero)
            {
                Console.WriteLine("\t**[FAILURE]  LoadLibraryA is not found.");
                return;
            }
            Console.WriteLine("\t[SUCCESS]  LoadLibraryA is found at 0x" + loadLibraryAAddr.ToString("X8") + "");

            Console.WriteLine("6. Create remote thread");
            IntPtr remoteThreadHandle = CreateRemoteThread(
                pHandle,
                IntPtr.Zero,
                0,
                loadLibraryAAddr,
                dllAddr,
                0,
                IntPtr.Zero);
            if (remoteThreadHandle == null || remoteThreadHandle == IntPtr.Zero)
            {
                Console.WriteLine("\t**[FAILURE]  Remote thread creation failed.");
                return;
            }
            Console.WriteLine("\t[SUCCESS]  The handle to the remote thread is 0x" + remoteThreadHandle.ToString("X8") + ".");

            ////



            return;
        }

    }
}
