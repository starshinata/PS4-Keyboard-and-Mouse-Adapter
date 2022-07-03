using Newtonsoft.Json;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    public class UserSettingsV2
    {
        // Only keep this if this is the MOST RECENT VERSION
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        ////////////////////////////////////////////////////////////////////////


        //Note this property will not be serialised (see the save method)
        public Dictionary<VirtualKey, PhysicalKey> KeyboardMappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();


        public Dictionary<VirtualKey, PhysicalKeyGroup> Mappings { get; set; } = new Dictionary<VirtualKey, PhysicalKeyGroup>();

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        public int AdvancedMappingPage_MappingsToShow { get; set; } = 4;

        public bool AimToggle { get; set; } = false;

        // time in milliseconds, after toggling aim on/off, (and the user releases the aim button) before we retrigger on/off
        public int AimToggleRetoggleDelay { get; set; } = 0;

        public int AnalogStickLowerRange { get; set; } = 40;
        public int AnalogStickUpperRange { get; set; } = 95;

        public bool MouseAimSensitivityEnabled { get; set; } = false;

        public bool MouseControlsL3 { get; set; } = false;
        public bool MouseControlsR3 { get; set; } = false;

        public double MouseDistanceLowerRange { get; set; } = 5;
        public double MouseDistanceUpperRange { get; set; } = VideoMode.DesktopMode.Width / 20f;
        public double MouseMaxDistance { get; set; } = VideoMode.DesktopMode.Width / 2f;

        private int _MousePollingRate;

        // DualShock 4 polling rate
        // 60  Hz Bluetooth
        // 250 Hz Wired
        // so it doesnt make sense for mouse polling rate to be above 250
        public int MousePollingRate
        {
            get
            {
                return _MousePollingRate;
            }
            set
            {
                if (value < 10)
                {
                    _MousePollingRate = 10;
                }
                else if (value > 250)
                {

                    _MousePollingRate = 250;
                }
                else
                {
                    _MousePollingRate = value;
                }
            }
        }

        public double MouseWheelScrollHoldDuration { get; set; } = 40;

        public double MouseXAxisSensitivityAimModifier { get; set; } = 1;
        public double MouseXAxisSensitivityLookModifier { get; set; } = 1;
        public double MouseXAxisSensitivityMax { get; set; } = 100;

        public double MouseYAxisSensitivityAimModifier { get; set; } = 1;
        public double MouseYAxisSensitivityLookModifier { get; set; } = 1;
        public double MouseYAxisSensitivityMax { get; set; } = 100;

        public float RemotePlayVolume { get; set; } = 100;

        //TODO do we still need this ?
        public double XYRatio { get; set; } = 1;


        // false if we need to migrate
        // true means we can ignore
        // default is false until we find a value
        public bool Version_2_0_0_OrGreater { get; set; } = false;

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //



        ///////////////////////////////////////////////////////////////////////
        // INSTANCE
        ///////////////////////////////////////////////////////////////////////

        public UserSettingsV2()
        {
            MousePollingRate = 60;
        }

        public void BroadcastRefresh()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(""));
        }

        public UserSettingsV2 Clone()
        {
            // cloning by (serilise to string then deserialise)
            string json = JsonConvert.SerializeObject(this, Formatting.None);
            return JsonConvert.DeserializeObject<UserSettingsV2>(json);
        }

        public bool MappingsContainsKey(VirtualKey vk)
        {
            return Mappings.ContainsKey(vk) && Mappings[vk] != null;
        }

        public void GetKeyboardMappings()
        {
            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey vk in virtualKeys)
            {
                if (Mappings.ContainsKey(vk))
                {
                    PhysicalKeyGroup pkg = Mappings[vk];
                    if (pkg.PhysicalKeys != null)
                    {
                        foreach (PhysicalKey pk in pkg.PhysicalKeys)
                        {
                            if (pk != null && pk.KeyboardValue != Keyboard.Key.Unknown)
                            {
                                KeyboardMappings[vk] = pk;
                            }
                        }
                    }
                }
            }
        }

        public void Print()
        {
            UserSettingsV2.Print(this);
        }



        ///////////////////////////////////////////////////////////////////////
        // STATIC
        ///////////////////////////////////////////////////////////////////////
        public static UserSettingsV2 ImportValues(string json)
        {
            return JsonConvert.DeserializeObject<UserSettingsV2>(json);
        }

        public static void Print(UserSettingsV2 settings)
        {
            Log.Information("UserSettings.Print()");


            Log.Information("print mappings");
            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                Log.Information("print Mappings:{VirtKey:" + key + ", PhysicalKeyGroup: " + settings.Mappings[key] + "}");
            }

            Log.Information("print values");
            Type t = settings.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name != "KeyboardMappings" && prop.Name != "Mappings")
                {
                    MethodInfo getter = prop.GetGetMethod();
                    if (getter != null)
                    {
                        object value = getter.Invoke(settings, new object[] { });

                        Log.Information("print " + prop + ":" + value);
                    }
                }
            }
        }


    }
}
