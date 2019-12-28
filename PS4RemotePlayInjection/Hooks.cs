// PS4RemotePlayInterceptor (File: Classes/Hooks.cs)
//
// Copyright (c) 2018 Komefai
//
// Visit http://komefai.com for more information
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using EasyHook;

namespace PS4RemotePlayInterceptor
{
    /// <summary>
    /// EasyHook will look for a class implementing <see cref="EasyHook.IEntryPoint"/> during injection. This
    /// becomes the entry point within the target process after injection is complete.
    /// </summary>
    public class Hooks : EasyHook.IEntryPoint
    {
        /// <summary>
        /// Reference to the server interface
        /// </summary>
        private readonly InjectionInterface _server = null;

        /// <summary>
        /// Dummy handle used for controller emulation
        /// </summary>
        private readonly IntPtr _dummyHandle = new IntPtr(0xDABDAB);

        private static byte[] ToManagedArray(IntPtr pointer, int size)
        {
            byte[] managedArray = new byte[size];
            Marshal.Copy(pointer, managedArray, 0, size);
            return managedArray;
        }

        private static void RestoreUnmanagedArray(IntPtr pointer, int size, byte[] managedArray)
        {
            unsafe
            {
                byte* ptr = (byte*)pointer.ToPointer();
                for (var i = 0; i < size; i++)
                {
                    ptr[i] = managedArray[i];
                }
            }
        }

        #region Setup
        /// <summary>
        /// EasyHook requires a constructor that matches <paramref name="context"/> and any additional parameters as provided
        /// in the original call to <see cref="EasyHook.RemoteHooking.Inject(int, EasyHook.InjectionOptions, string, string, object[])"/>.
        /// 
        /// Multiple constructors can exist on the same <see cref="EasyHook.IEntryPoint"/>, providing that each one has a corresponding Run method (e.g. <see cref="Run(EasyHook.RemoteHooking.IContext, string)"/>).
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public Hooks(
            EasyHook.RemoteHooking.IContext context,
            string channelName)
        {
            // Connect to server object using provided channel name
            _server = EasyHook.RemoteHooking.IpcConnectClient<InjectionInterface>(channelName);

            // If Ping fails then the Run method will be not be called
            _server.Ping();
        }

        /// <summary>
        /// The main entry point for our logic once injected within the target process. 
        /// This is where the hooks will be created, and a loop will be entered until host process exits.
        /// EasyHook requires a matching Run method for the constructor
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public void Run(
            EasyHook.RemoteHooking.IContext context,
            string channelName)
        {
            // Injection is now complete and the server interface is connected
            _server.OnInjectionSuccess(EasyHook.RemoteHooking.GetCurrentProcessId());

            // Install hooks
            List<EasyHook.LocalHook> hooks = new List<LocalHook>();

            // With controller emulation
            if (_server.ShouldEmulateController())
            {
                // CreateFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx
                var createFileHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("kernel32.dll", "CreateFileW"),
                    new CreateFile_Delegate(CreateFile_Hook),
                    this);

                // ReadFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa365467(v=vs.85).aspx
                var readFileHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("kernel32.dll", "ReadFile"),
                    new ReadFile_Delegate(ReadFile_Hook),
                    this);

                // WriteFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa365747(v=vs.85).aspx
                var writeFileHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("kernel32.dll", "WriteFile"),
                    new WriteFile_Delegate(WriteFile_Hook),
                    this);

                // HidD_GetAttributes http://www.pinvoke.net/default.aspx/hid.hidd_getattributes
                var HidD_GetAttributesHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_GetAttributes"),
                    new HidD_GetAttributes_Delegate(HidD_GetAttributes_Hook),
                    this);

                // HidD_GetFeature http://www.pinvoke.net/default.aspx/hid.hidd_getfeature
                var HidD_GetFeatureHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_GetFeature"),
                    new HidD_GetFeature_Delegate(HidD_GetFeature_Hook),
                    this);

                // HidD_SetFeature https://msdn.microsoft.com/en-us/library/windows/hardware/ff539684(v=vs.85).aspx
                var HidD_SetFeatureHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_SetFeature"),
                    new HidD_SetFeature_Delegate(HidD_SetFeature_Hook),
                    this);

                // HidD_GetPreparsedData http://www.pinvoke.net/default.aspx/hid.hidd_getfeature
                var HidD_GetPreparsedDataHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_GetPreparsedData"),
                    new HidD_GetPreparsedData_Delegate(HidD_GetPreparsedData_Hook),
                    this);

                // HidD_FreePreparsedData http://www.pinvoke.net/default.aspx/hid.hidd_freepreparseddata
                var HidD_FreePreparsedDataHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_FreePreparsedData"),
                    new HidD_FreePreparsedData_Delegate(HidD_FreePreparsedData_Hook),
                    this);

                // HidD_GetManufacturerString https://msdn.microsoft.com/en-us/library/windows/hardware/ff538959(v=vs.85).aspx
                var HidD_GetManufacturerStringHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_GetManufacturerString"),
                    new HidD_GetManufacturerString_Delegate(HidD_GetManufacturerString_Hook),
                    this);

                // HidD_GetProductString https://msdn.microsoft.com/en-us/library/windows/hardware/ff539681(v=vs.85).aspx
                var HidD_GetProductStringHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_GetProductString"),
                    new HidD_GetProductString_Delegate(HidD_GetProductString_Hook),
                    this);

                // HidD_GetSerialNumberString https://msdn.microsoft.com/en-us/library/windows/hardware/ff539683(v=vs.85).aspx
                var HidD_GetSerialNumberStringHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidD_GetSerialNumberString"),
                    new HidD_GetSerialNumberString_Delegate(HidD_GetSerialNumberString_Hook),
                    this);

                // HidP_GetCapsHook http://www.pinvoke.net/default.aspx/hid.hidp_getcaps
                var HidP_GetCapsHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidP_GetCaps"),
                    new HidP_GetCaps_Delegate(HidP_GetCaps_Hook),
                    this);

                // HidP_GetValueCapsHook https://msdn.microsoft.com/en-us/library/windows/hardware/ff539754(v=vs.85).aspx
                var HidP_GetValueCapsHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("hid.dll", "HidP_GetValueCaps"),
                    new HidP_GetValueCaps_Delegate(HidP_GetValueCaps_Hook),
                    this);

                hooks.Add(createFileHook);
                hooks.Add(readFileHook);
                hooks.Add(writeFileHook);
                hooks.Add(HidD_GetAttributesHook);
                hooks.Add(HidD_GetFeatureHook);
                hooks.Add(HidD_SetFeatureHook);
                hooks.Add(HidD_GetPreparsedDataHook);
                hooks.Add(HidD_FreePreparsedDataHook);
                hooks.Add(HidD_GetManufacturerStringHook);
                hooks.Add(HidD_GetProductStringHook);
                hooks.Add(HidD_GetSerialNumberStringHook);
                hooks.Add(HidP_GetCapsHook);
                hooks.Add(HidP_GetValueCapsHook);
            }
            // Without controller emulation
            else
            {
                // ReadFile https://msdn.microsoft.com/en-us/library/windows/desktop/aa365467(v=vs.85).aspx
                var readFileHook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("kernel32.dll", "ReadFile"),
                    new ReadFile_Delegate(ReadFile_Hook),
                    this);

                hooks.Add(readFileHook);
            }

            // Activate hooks on all threads except the current thread
            foreach (var h in hooks)
            {
                h.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }

            // Wake up the process (required if using RemoteHooking.CreateAndInject)
            EasyHook.RemoteHooking.WakeUpProcess();

            try
            {
                // Loop until injector closes (i.e. IPC fails)
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    _server.Ping();
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }

            // Remove hooks
            foreach (var h in hooks)
            {
                h.Dispose();
            }

            // Finalise cleanup of hooks
            EasyHook.LocalHook.Release();
        }

        /// <summary>
        /// P/Invoke to determine the filename from a file handle
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa364962(v=vs.85).aspx
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpszFilePath"></param>
        /// <param name="cchFilePath"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetFinalPathNameByHandle(IntPtr hFile, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszFilePath, uint cchFilePath, uint dwFlags);

        #endregion

        #region CreateFileW Hook
        /// <summary>
        /// The CreateFile delegate, this is needed to create a delegate of our hook function <see cref="CreateFile_Hook(string, uint, uint, IntPtr, uint, uint, IntPtr)"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="desiredAccess"></param>
        /// <param name="shareMode"></param>
        /// <param name="securityAttributes"></param>
        /// <param name="creationDisposition"></param>
        /// <param name="flagsAndAttributes"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall,
                    CharSet = CharSet.Unicode,
                    SetLastError = true)]
        delegate IntPtr CreateFile_Delegate(
                    String filename,
                    UInt32 desiredAccess,
                    UInt32 shareMode,
                    IntPtr securityAttributes,
                    UInt32 creationDisposition,
                    UInt32 flagsAndAttributes,
                    IntPtr templateFile);

        /// <summary>
        /// Using P/Invoke to call original method.
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363858(v=vs.85).aspx
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="desiredAccess"></param>
        /// <param name="shareMode"></param>
        /// <param name="securityAttributes"></param>
        /// <param name="creationDisposition"></param>
        /// <param name="flagsAndAttributes"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll",
            CharSet = CharSet.Unicode,
            SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr CreateFileW(
            String filename,
            UInt32 desiredAccess,
            UInt32 shareMode,
            IntPtr securityAttributes,
            UInt32 creationDisposition,
            UInt32 flagsAndAttributes,
            IntPtr templateFile);

        /// <summary>
        /// The CreateFile hook function. This will be called instead of the original CreateFile once hooked.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="desiredAccess"></param>
        /// <param name="shareMode"></param>
        /// <param name="securityAttributes"></param>
        /// <param name="creationDisposition"></param>
        /// <param name="flagsAndAttributes"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        IntPtr CreateFile_Hook(
            String filename,
            UInt32 desiredAccess,
            UInt32 shareMode,
            IntPtr securityAttributes,
            UInt32 creationDisposition,
            UInt32 flagsAndAttributes,
            IntPtr templateFile)
        {
            // SPOOF
            if (filename != null && filename.StartsWith(@"\\?\hid#"))
            {
                return _dummyHandle;
            }

            IntPtr result = CreateFileW(
                filename,
                desiredAccess,
                shareMode,
                securityAttributes,
                creationDisposition,
                flagsAndAttributes,
                templateFile
            );

            try
            {
                string mode = string.Empty;
                switch (creationDisposition)
                {
                    case 1:
                        mode = "CREATE_NEW";
                        break;
                    case 2:
                        mode = "CREATE_ALWAYS";
                        break;
                    case 3:
                        mode = "OPEN_ALWAYS";
                        break;
                    case 4:
                        mode = "OPEN_EXISTING";
                        break;
                    case 5:
                        mode = "TRUNCATE_EXISTING";
                        break;
                }

                // Send to server
                _server.OnCreateFile(filename.ToString(), result.ToString());
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            // now call the original API...
            return result;
        }
        #endregion

        #region ReadFile Hook

        // FrameCounter
        private static int __frameCounter = -1;

        /// <summary>
        /// The ReadFile delegate, this is needed to create a delegate of our hook function <see cref="ReadFile_Hook(IntPtr, IntPtr, uint, out uint, IntPtr)"/>.
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToRead"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate bool ReadFile_Delegate(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        /// <summary>
        /// Using P/Invoke to call the orginal function
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToRead"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ReadFile(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        /// <summary>
        /// The ReadFile hook function. This will be called instead of the original ReadFile once hooked.
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToRead"></param>
        /// <param name="lpNumberOfBytesRead"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        bool ReadFile_Hook(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped)
        {
            bool result = false;
            lpNumberOfBytesRead = 0;

            const int bufferSize = 64;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;
                    try
                    {
                        // Call original for any other files
                        if (hFile != _dummyHandle)
                        {
                            result = ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, out lpNumberOfBytesRead, lpOverlapped);
                        }

                        // Retrieve filename from the file handle
                        StringBuilder filename = new StringBuilder(255);
                        GetFinalPathNameByHandle(hFile, filename, 255, 0);

                        if (hFile == _dummyHandle && nNumberOfBytesToRead == 2048 && lpNumberOfBytesRead == 0)
                        {
                            lpNumberOfBytesRead = bufferSize;

                            // Create fake report buffer
                            byte[] fakeReport = fakeReport = new byte[bufferSize]
                            {
                                1, 128, 126, 125, 125, 8, 0, 0, 0, 0, 151, 201, 14, 7, 0, 5, 0, 255, 255, 179, 7, 74, 31,
                                113, 255, 0, 0, 0, 0, 0, 27, 0, 0, 0, 0, 128, 0, 0, 0, 128, 0, 0, 0, 0, 128, 0, 0, 0, 128, 0,
                                0, 0, 0, 128, 0, 0, 0, 128, 0, 0, 0, 0, 128, 0
                            };

                            // Assign the spoofed frame counter
                            __frameCounter++;
                            fakeReport[7] = (byte)((__frameCounter << 2) & 0xFF);

                            // Send to server
                            _server.OnReadFile(filename.ToString(), ref fakeReport);

                            // Restore managedArray back to unmanaged array
                            RestoreUnmanagedArray(lpBuffer, fakeReport.Length, fakeReport);

                            return result;
                        }
                    }
                    catch
                    {
                        // swallow exceptions so that any issues caused by this code do not crash target process
                    }
                }
                else
                {
                    // Call original first so we have a value for lpNumberOfBytesRead
                    result = ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, out lpNumberOfBytesRead, lpOverlapped);

                    try
                    {
                        // Retrieve filename from the file handle
                        StringBuilder filename = new StringBuilder(255);
                        GetFinalPathNameByHandle(hFile, filename, 255, 0);

                        //// Log for debug
                        //_server.ReportLog(
                        //    string.Format("[{0}:{1}]: READ ({2} bytes) \"{3}\"",
                        //    EasyHook.RemoteHooking.GetCurrentProcessId(), EasyHook.RemoteHooking.GetCurrentThreadId()
                        //    , lpNumberOfBytesRead, filename));

                        // Only respond if it is a device stream
                        if (string.IsNullOrWhiteSpace(filename.ToString()) && lpNumberOfBytesRead == bufferSize)
                        {
                            // Copy unmanaged array for server
                            byte[] managedArray = ToManagedArray(lpBuffer, bufferSize);

                            // Make sure it is a input report (USB type)
                            if (managedArray[0] == 0x1)
                            {
                                // Send to server
                                _server.OnReadFile(filename.ToString(), ref managedArray);

                                // Restore managedArray back to unmanaged array
                                RestoreUnmanagedArray(lpBuffer, bufferSize, managedArray);
                            }
                        }
                    }
                    catch
                    {
                        // swallow exceptions so that any issues caused by this code do not crash target process
                    }
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region WriteFile Hook

        /// <summary>
        /// The WriteFile delegate, this is needed to create a delegate of our hook function <see cref="WriteFile_Hook(IntPtr, IntPtr, uint, out uint, IntPtr)"/>.
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToWrite"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        delegate bool WriteFile_Delegate(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        /// <summary>
        /// Using P/Invoke to call original WriteFile method
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToWrite"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WriteFile(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        /// <summary>
        /// The WriteFile hook function. This will be called instead of the original WriteFile once hooked.
        /// </summary>
        /// <param name="hFile"></param>
        /// <param name="lpBuffer"></param>
        /// <param name="nNumberOfBytesToWrite"></param>
        /// <param name="lpNumberOfBytesWritten"></param>
        /// <param name="lpOverlapped"></param>
        /// <returns></returns>
        bool WriteFile_Hook(
            IntPtr hFile,
            IntPtr lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped)
        {
            bool result = false;

            // Call original first so we get lpNumberOfBytesWritten
            result = WriteFile(hFile, lpBuffer, nNumberOfBytesToWrite, out lpNumberOfBytesWritten, lpOverlapped);

            try
            {
                // Retrieve filename from the file handle
                StringBuilder filename = new StringBuilder(255);
                GetFinalPathNameByHandle(hFile, filename, 255, 0);
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }

        #endregion

        #region HidD_GetAttributes Hook
        [StructLayout(LayoutKind.Sequential)]
        internal struct HIDD_ATTRIBUTES
        {
            public Int32 Size;
            public Int16 VendorID;
            public Int16 ProductID;
            public Int16 VersionNumber;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_GetAttributes_Delegate(IntPtr hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

        bool HidD_GetAttributes_Hook(IntPtr hidDeviceObject, ref HIDD_ATTRIBUTES attributes)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;
                    attributes.Size = 12;
                    attributes.VendorID = 1356;
                    attributes.ProductID = 2508;
                    attributes.VersionNumber = 256;
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_GetAttributes(hidDeviceObject, ref attributes);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_GetFeature Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_GetFeature_Delegate(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_GetFeature(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        bool HidD_GetFeature_Hook(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;

                    // Copy unmanaged array for server
                    unsafe
                    {
                        fixed (byte* p = &lpReportBuffer)
                        {
                            IntPtr ptr = (IntPtr)p;

                            if (lpReportBuffer == 0x12)
                            {
                                byte[] report =
                                {
                                    18, 198, 3, 170, 95, 27, 64, 8, 37, 0, 172, 252, 74, 74, 71, 168
                                };
                                RestoreUnmanagedArray(ptr, report.Length, report);
                            }
                            else if (lpReportBuffer == 0xA3)
                            {
                                byte[] report =
                                {
                                    163, 77, 97, 121, 32, 49, 55, 32, 50, 48, 49, 54, 0, 0, 0, 0, 0, 48, 54, 58,
                                    51, 54, 58, 50, 54, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 4, 100, 1, 0, 0, 0, 8, 112, 0, 2, 0,
                                    128, 3, 0
                                };
                                RestoreUnmanagedArray(ptr, report.Length, report);
                            }
                            else if (lpReportBuffer == 0x02)
                            {
                                byte[] report =
                                {
                                    2, 6, 0, 3, 0, 252, 255, 133, 34, 133, 221, 237, 34, 31, 221, 228, 35, 13,
                                    220, 28, 2, 28, 2, 250, 31, 5, 224, 83, 32, 173, 223, 211, 31, 46, 224, 7, 0
                                };
                                RestoreUnmanagedArray(ptr, report.Length, report);
                            }
                        }
                    }
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_GetFeature(hidDeviceObject, ref lpReportBuffer, reportBufferLength);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_SetFeature Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_SetFeature_Delegate(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_SetFeature(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        bool HidD_SetFeature_Hook(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_SetFeature(hidDeviceObject, ref lpReportBuffer, reportBufferLength);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_GetPreparsedData Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_GetPreparsedData_Delegate(IntPtr hidDeviceObject, ref IntPtr preparsedData);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, ref IntPtr preparsedData);

        bool HidD_GetPreparsedData_Hook(IntPtr hidDeviceObject, ref IntPtr preparsedData)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_GetPreparsedData(hidDeviceObject, ref preparsedData);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_FreePreparsedData Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_FreePreparsedData_Delegate(IntPtr preparsedData);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        bool HidD_FreePreparsedData_Hook(IntPtr preparsedData)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_FreePreparsedData(preparsedData);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_GetManufacturerString Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_GetManufacturerString_Delegate(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_GetManufacturerString(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        bool HidD_GetManufacturerString_Hook(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = true;
                    unsafe
                    {
                        fixed (byte* p = &lpReportBuffer)
                        {
                            IntPtr ptr = (IntPtr)p;
                            byte[] data =
                            {
                                83, 0, 111, 0, 110, 0, 121, 0, 32, 0, 73, 0, 110, 0, 116, 0, 101, 0, 114, 0, 97,
                                0, 99, 0, 116, 0, 105, 0, 118, 0, 101, 0, 32, 0, 69, 0, 110, 0, 116, 0, 101, 0, 114, 0, 116,
                                0, 97, 0, 105, 0, 110, 0, 109, 0, 101, 0, 110, 0, 116, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                            };
                            RestoreUnmanagedArray(ptr, data.Length, data);
                        }
                    }
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_GetManufacturerString(hidDeviceObject, ref lpReportBuffer, reportBufferLength);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_GetProductString Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_GetProductString_Delegate(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_GetProductString(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        bool HidD_GetProductString_Hook(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // Call original first so we get the result
                    result = HidD_GetProductString(hidDeviceObject, ref lpReportBuffer, reportBufferLength);

                    // SPOOF
                    result = true;
                    unsafe
                    {
                        fixed (byte* p = &lpReportBuffer)
                        {
                            IntPtr ptr = (IntPtr)p;
                            byte[] data =
                            {
                                87, 0, 105, 0, 114, 0, 101, 0, 108, 0, 101, 0, 115, 0, 115, 0, 32, 0, 67, 0, 111,
                                0, 110, 0, 116, 0, 114, 0, 111, 0, 108, 0, 108, 0, 101, 0, 114, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                            };
                            RestoreUnmanagedArray(ptr, data.Length, data);
                        }
                    }
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_GetProductString(hidDeviceObject, ref lpReportBuffer, reportBufferLength);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidD_GetSerialNumberString Hook
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate bool HidD_GetSerialNumberString_Delegate(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool HidD_GetSerialNumberString(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength);

        bool HidD_GetSerialNumberString_Hook(IntPtr hidDeviceObject, ref Byte lpReportBuffer, Int32 reportBufferLength)
        {
            bool result = false;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // Call original first so we get the result
                    result = HidD_GetSerialNumberString(hidDeviceObject, ref lpReportBuffer, reportBufferLength);

                    // SPOOF
                    result = true;
                }
                else
                {
                    // Call original first so we get the result
                    result = HidD_GetSerialNumberString(hidDeviceObject, ref lpReportBuffer, reportBufferLength);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidP_GetCaps Hook
        [StructLayout(LayoutKind.Sequential)]
        internal struct HIDP_CAPS
        {
            public Int16 Usage;
            public Int16 UsagePage;
            public Int16 InputReportByteLength;
            public Int16 OutputReportByteLength;
            public Int16 FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public Int16[] Reserved;
            public Int16 NumberLinkCollectionNodes;
            public Int16 NumberInputButtonCaps;
            public Int16 NumberInputValueCaps;
            public Int16 NumberInputDataIndices;
            public Int16 NumberOutputButtonCaps;
            public Int16 NumberOutputValueCaps;
            public Int16 NumberOutputDataIndices;
            public Int16 NumberFeatureButtonCaps;
            public Int16 NumberFeatureValueCaps;
            public Int16 NumberFeatureDataIndices;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int HidP_GetCaps_Delegate(IntPtr preparsedData, ref HIDP_CAPS capabilities);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

        int HidP_GetCaps_Hook(IntPtr preparsedData, ref HIDP_CAPS capabilities)
        {
            int result = 0;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = 0x110000;
                    capabilities.Usage = 5;
                    capabilities.UsagePage = 1;
                    capabilities.InputReportByteLength = 64;
                    capabilities.OutputReportByteLength = 32;
                    capabilities.FeatureReportByteLength = 64;

                    capabilities.NumberLinkCollectionNodes = 1;
                    capabilities.NumberInputButtonCaps = 1;
                    capabilities.NumberInputValueCaps = 9;
                    capabilities.NumberInputDataIndices = 23;
                    capabilities.NumberOutputButtonCaps = 0;
                    capabilities.NumberOutputValueCaps = 1;
                    capabilities.NumberOutputDataIndices = 1;
                    capabilities.NumberFeatureButtonCaps = 0;
                    capabilities.NumberFeatureValueCaps = 48;
                    capabilities.NumberFeatureDataIndices = 48;
                }
                else
                {
                    // Call original first so we get the result
                    result = HidP_GetCaps(preparsedData, ref capabilities);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion

        #region HidP_GetValueCaps Hook
        internal enum HIDP_REPORT_TYPE
        {
            HidP_Input,
            HidP_Output,
            HidP_Feature
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HidP_Range
        {
            public short UsageMin;
            public short UsageMax;
            public short StringMin;
            public short StringMax;
            public short DesignatorMin;
            public short DesignatorMax;
            public short DataIndexMin;
            public short DataIndexMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HidP_NotRange
        {
            public short Usage;
            public short Reserved1;
            public short StringIndex;
            public short Reserved2;
            public short DesignatorIndex;
            public short Reserved3;
            public short DataIndex;
            public short Reserved4;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct HidP_Value_Caps
        {
            [FieldOffset(0)]
            public ushort UsagePage;
            [FieldOffset(2)]
            public byte ReportID;
            [FieldOffset(3), MarshalAs(UnmanagedType.U1)]
            public bool IsAlias;
            [FieldOffset(4)]
            public ushort BitField;
            [FieldOffset(6)]
            public ushort LinkCollection;
            [FieldOffset(8)]
            public ushort LinkUsage;
            [FieldOffset(10)]
            public ushort LinkUsagePage;
            [FieldOffset(12), MarshalAs(UnmanagedType.U1)]
            public bool IsRange;
            [FieldOffset(13), MarshalAs(UnmanagedType.U1)]
            public bool IsStringRange;
            [FieldOffset(14), MarshalAs(UnmanagedType.U1)]
            public bool IsDesignatorRange;
            [FieldOffset(15), MarshalAs(UnmanagedType.U1)]
            public bool IsAbsolute;
            [FieldOffset(16), MarshalAs(UnmanagedType.U1)]
            public bool HasNull;
            [FieldOffset(17)]
            public byte Reserved;
            [FieldOffset(18)]
            public short BitSize;
            [FieldOffset(20)]
            public short ReportCount;
            [FieldOffset(22)]
            public ushort Reserved2a;
            [FieldOffset(24)]
            public ushort Reserved2b;
            [FieldOffset(26)]
            public ushort Reserved2c;
            [FieldOffset(28)]
            public ushort Reserved2d;
            [FieldOffset(30)]
            public ushort Reserved2e;
            [FieldOffset(32)]
            public int UnitsExp;
            [FieldOffset(36)]
            public int Units;
            [FieldOffset(40)]
            public int LogicalMin;
            [FieldOffset(44)]
            public int LogicalMax;
            [FieldOffset(48)]
            public int PhysicalMin;
            [FieldOffset(52)]
            public int PhysicalMax;

            [FieldOffset(56)]
            public HidP_Range Range;
            [FieldOffset(56)]
            public HidP_NotRange NotRange;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int HidP_GetValueCaps_Delegate(HIDP_REPORT_TYPE reportType, ref Byte valueCaps, ref short valueCapsLength, IntPtr preparsedData);

        [DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int HidP_GetValueCaps(HIDP_REPORT_TYPE reportType, ref Byte valueCaps, ref short valueCapsLength, IntPtr preparsedData);

        int HidP_GetValueCaps_Hook(HIDP_REPORT_TYPE reportType, ref Byte valueCaps, ref short valueCapsLength, IntPtr preparsedData)
        {
            int result = 0;

            try
            {
                if (_server.ShouldEmulateController())
                {
                    // SPOOF
                    result = 0x110000;
                    unsafe
                    {
                        fixed (byte* p = &valueCaps)
                        {
                            IntPtr ptr = (IntPtr)p;
                            byte[] managedArray = ToManagedArray(ptr, 3456);
                            managedArray[0] = 0x0;
                            managedArray[1] = 0xFF;
                            managedArray[2] = 0x4;
                            managedArray[3] = 0x0;
                            managedArray[4] = 0x2;
                            managedArray[5] = 0x0;
                            managedArray[6] = 0x0;
                            managedArray[7] = 0x0;
                            managedArray[8] = 0x5;
                            managedArray[9] = 0x0;
                            managedArray[10] = 0x1;
                            managedArray[11] = 0x0;
                            managedArray[12] = 0x0;
                            managedArray[13] = 0x0;
                            managedArray[14] = 0x0;
                            managedArray[15] = 0x1;
                            managedArray[16] = 0x0;
                            //managedArray[17] = managedArray[17];
                            managedArray[18] = 0x8;
                            managedArray[19] = 0x0;
                            managedArray[20] = 0x24;
                            managedArray[21] = 0x0;
                            //managedArray[22] = managedArray[22];
                            //managedArray[23] = managedArray[23];
                            //managedArray[24] = managedArray[24];
                            //managedArray[25] = managedArray[25];
                            //managedArray[26] = managedArray[26];
                            //managedArray[27] = managedArray[27];
                            //managedArray[28] = managedArray[28];
                            //managedArray[29] = managedArray[29];
                            //managedArray[30] = managedArray[30];
                            //managedArray[31] = managedArray[31];
                            managedArray[32] = 0x0;
                            managedArray[33] = 0x0;
                            managedArray[34] = 0x0;
                            managedArray[35] = 0x0;
                            managedArray[36] = 0x0;
                            managedArray[37] = 0x0;
                            managedArray[38] = 0x0;
                            managedArray[39] = 0x0;
                            managedArray[40] = 0x0;
                            managedArray[41] = 0x0;
                            managedArray[42] = 0x0;
                            managedArray[43] = 0x0;
                            managedArray[44] = 0xFF;
                            managedArray[45] = 0x0;
                            managedArray[46] = 0x0;
                            managedArray[47] = 0x0;

                            managedArray[48] = 0x0;
                            managedArray[49] = 0x0;
                            managedArray[50] = 0x0;
                            managedArray[51] = 0x0;
                            managedArray[52] = 0x3B;
                            managedArray[53] = 0x01;
                            managedArray[54] = 0x0;
                            managedArray[55] = 0x23;
                            managedArray[56] = 0x0;
                            managedArray[57] = 0x23;
                            managedArray[58] = 0x0;
                            managedArray[59] = 0x0;
                            managedArray[60] = 0x0;
                            managedArray[61] = 0x0;
                            managedArray[62] = 0x0;
                            managedArray[63] = 0x0;
                            managedArray[64] = 0x0;
                            managedArray[65] = 0x0;
                            managedArray[66] = 0x0;
                            managedArray[67] = 0x0;

                            //managedArray[72] = 0x80; // a3
                            //managedArray[128] = 0x43; // a4
                            //managedArray[74] = 0xA3; // a5
                            //managedArray[92] = 0x30; // a6

                            var i = 72 + 20;
                            while (i + 36 < 3456)
                            {
                                managedArray[i - 20] = 0x80;
                                managedArray[i + 36] = 0x43;
                                managedArray[i] = 0x30;
                                managedArray[i - 18] = 0xA3;
                                managedArray[i - 19] = 0xFF;

                                i += 72;
                            }

                            RestoreUnmanagedArray(ptr, managedArray.Length, managedArray);
                        }
                    }
                }
                else
                {
                    // Call original first so we get the result
                    result = HidP_GetValueCaps_Hook(reportType, ref valueCaps, ref valueCapsLength, preparsedData);
                }
            }
            catch
            {
                // swallow exceptions so that any issues caused by this code do not crash target process
            }

            return result;
        }
        #endregion
    }
}
