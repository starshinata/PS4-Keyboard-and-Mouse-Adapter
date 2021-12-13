
using Pizza;
using Serilog;
using System;
using System.Collections.Generic;
using System.Management;

namespace PS4KeyboardAndMouseAdapter.backend.DebugLogging
{
    class HidFacade
    {
        public static void get()
        {
            try { 
                List<DeviceInfo> usbDevices = GetUSBDevices();

                foreach (DeviceInfo device in usbDevices)
                {
                    Log.Verbose("Device ID: {0}, PNP Device ID: {1}, Description: {2}, InstallDate: {3}, LastErrorCode: {4}, Manufacturer: {5}",
                        device.DeviceID,
                        device.PnpDeviceID,
                        device.Description,
                        device.InstallDate,
                        device.LastErrorCode,
                        device.Manufacturer);
                }
    
            }
            catch (Exception ex)
            {
                 ExceptionLogger.LogException("HidFacade.get failed " , ex);
            }
        }

        static List<DeviceInfo> GetUSBDevices()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            ManagementObjectCollection collection;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            foreach (ManagementBaseObject device in collection)
            {

                if ("HIDClass" == (string)device.GetPropertyValue("PNPClass"))
                {
                    PropertyDataCollection x = device.Properties;
                    foreach (PropertyData prop in x)
                    {
                        Log.Verbose("prop.Name " + prop.Name + " : " + prop.Value);
                    }

                    /*
                    devices.Add(new DeviceInfo(
                        (string)device.GetPropertyValue("DeviceID"),
                        (string)device.GetPropertyValue("PNPDeviceID"),
                        (string)device.GetPropertyValue("Description"),
                        (string)device.GetPropertyValue("InstallDate"),
                        (string)device.GetPropertyValue("LastErrorCode"),
                        (string)device.GetPropertyValue("Manufacturer")
                    ));
                    */
                }
            }

            collection.Dispose();
            return devices;
        }
    }

    class DeviceInfo
    {
        public DeviceInfo(string deviceID,
            string pnpDeviceID,
            string description,
            string InstallDate,

            string LastErrorCode,
            string Manufacturer)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.InstallDate = InstallDate;

            this.LastErrorCode = LastErrorCode;
            this.Manufacturer = Manufacturer;
        }

        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string InstallDate { get; private set; }
        public string LastErrorCode { get; private set; }
        public string Manufacturer { get; private set; }

    }
}
