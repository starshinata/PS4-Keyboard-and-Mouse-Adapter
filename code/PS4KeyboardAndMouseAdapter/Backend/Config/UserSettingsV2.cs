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

        ////////////////////////////////////////////////////////////////////////////
        /// Static props
        ////////////////////////////////////////////////////////////////////////////
        private static readonly ILogger StaticLogger = Log.ForContext(typeof(UserSettingsV1));


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


        private static void AddMapping(UserSettingsV3 newSettings, VirtualKey vk, PhysicalKeyGroup pkg)
        {

            foreach (PhysicalKey pk in pkg.PhysicalKeys)
            {
                Mapping mapping = new Mapping();
                mapping.uid = UserSettingsContainer.getNextMappingUid();
                mapping.PhysicalKeys.Add(pk);
                mapping.VirtualKeys.Add(vk);

                newSettings.Mappings.Add(mapping);
            }
        }


        public static UserSettingsV3 ImportValues(string json)
        {
            StaticLogger.Information("UserSettingsV2.ImportValues()");

            UserSettingsV2 legacySettings = JsonConvert.DeserializeObject<UserSettingsV2>(json);
            UserSettingsV3 newSettings = new UserSettingsV3();

            newSettings.AimToggle = legacySettings.AimToggle;
            newSettings.AimToggleRetoggleDelay = legacySettings.AimToggleRetoggleDelay;

            newSettings.AnalogStickLowerRange = legacySettings.AnalogStickLowerRange;
            newSettings.AnalogStickUpperRange = legacySettings.AnalogStickUpperRange;

            newSettings.MouseAimSensitivityEnabled = legacySettings.MouseAimSensitivityEnabled;

            newSettings.MouseControlsL3 = legacySettings.MouseControlsL3;
            newSettings.MouseControlsR3 = legacySettings.MouseControlsR3;

            newSettings.MouseDistanceLowerRange = legacySettings.MouseDistanceLowerRange;
            newSettings.MouseDistanceUpperRange = legacySettings.MouseDistanceUpperRange;
            newSettings.MouseMaxDistance = legacySettings.MouseMaxDistance;

            newSettings.MousePollingRate = legacySettings.MousePollingRate;

            newSettings.MouseWheelScrollHoldDuration = legacySettings.MouseWheelScrollHoldDuration;

            newSettings.MouseXAxisSensitivityAimModifier = legacySettings.MouseXAxisSensitivityAimModifier;
            newSettings.MouseXAxisSensitivityLookModifier = legacySettings.MouseXAxisSensitivityLookModifier;
            newSettings.MouseXAxisSensitivityMax = legacySettings.MouseXAxisSensitivityMax;

            newSettings.MouseYAxisSensitivityAimModifier = legacySettings.MouseYAxisSensitivityAimModifier;
            newSettings.MouseYAxisSensitivityLookModifier = legacySettings.MouseYAxisSensitivityLookModifier;
            newSettings.MouseYAxisSensitivityMax = legacySettings.MouseYAxisSensitivityMax;

            newSettings.RemotePlayVolume = legacySettings.RemotePlayVolume;

            newSettings.XYRatio = legacySettings.XYRatio;


            foreach (VirtualKey vk in legacySettings.Mappings.Keys)
            {
                PhysicalKeyGroup pkg = legacySettings.Mappings[vk];
                AddMapping(newSettings, vk, pkg);
            }



            newSettings.RefreshOptimisations();
            return newSettings;
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
