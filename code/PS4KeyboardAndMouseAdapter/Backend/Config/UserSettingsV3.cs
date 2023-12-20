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
    public class UserSettingsV3
    {
        // Only keep this if this is the MOST RECENT VERSION
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        ////////////////////////////////////////////////////////////////////////

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

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

        // Version of UserSettings
        public int Version { get; set; } = 3;

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //


        ////////////////////////////////////////////////////////////////////////


        // Note this property will not be serialised (see the save method)
        // this property is for simple processing for AdvancedMappingsPag
        public Dictionary<string, List<Mapping>> Mappings_ForAdvancedMappingsPage = new Dictionary<string, List<Mapping>>();

        // Note this property will not be serialised (see the save method)
        // this property is for simple processing of inputs in GamepadProcessor.cs
        public Dictionary<VirtualKey, List<PhysicalKeyGroup>> Mappings_ForGamepadProcessor { get; set; } = new Dictionary<VirtualKey, List<PhysicalKeyGroup>>();

        // Note this property will not be serialised (see the save method)
        // this property is just for populating simple values for the SimpleConfigPage
        public Dictionary<VirtualKey, PhysicalKey> Mappings_ForSimpleConfigPage { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();

        public List<Mapping> Mappings { get; set; } = new List<Mapping>();


        ///////////////////////////////////////////////////////////////////////
        // INSTANCE METHODS
        ///////////////////////////////////////////////////////////////////////

        public UserSettingsV3()
        {
            MousePollingRate = 60;
        }

        public void BroadcastRefresh()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(""));
        }

        public UserSettingsV3 CloneForSave()
        {
            // cloning by (serilise to string then deserialise)
            string json = JsonConvert.SerializeObject(this, Formatting.None);
            UserSettingsV3 tempSettings = JsonConvert.DeserializeObject<UserSettingsV3>(json);

            // removing generated stuff, that dont need to be serialised
            tempSettings.Mappings_ForAdvancedMappingsPage = null;
            tempSettings.Mappings_ForGamepadProcessor = null;
            tempSettings.Mappings_ForSimpleConfigPage = null;

            foreach (Mapping mapping in tempSettings.Mappings)
            {
                mapping.uid = -1;
            }

            return tempSettings;
        }

        public void Print()
        {
            UserSettingsV3.Print(this);
        }

        public void RefreshOptimisations()
        {
            RefreshOptimisations_Mappings_ForAdvancedMappingsPage();
            RefreshOptimisations_Mappings_ForGamepadProcessor();
            RefreshOptimisations_Mappings_ForSimpleConfigPage();
        }

        private void RefreshOptimisations_Mappings_ForAdvancedMappingsPage()
        {
            Mappings_ForAdvancedMappingsPage = new Dictionary<string, List<Mapping>>();

            // populate with VirtualKeys
            // so that if we unbind the only mapping for a virtual key it doesnt disappear
            foreach (VirtualKey vk in KeyUtility.GetVirtualKeyValues())
            {
                string vkString = vk.ToString();
                if (!Mappings_ForAdvancedMappingsPage.ContainsKey(vkString))
                {
                    Mappings_ForAdvancedMappingsPage[vkString] = new List<Mapping>();
                }
            }

            // build mappings properly
            foreach (Mapping mapping in Mappings)
            {
                string vk = mapping.GetCompositeKeyVirtual();

                if (!Mappings_ForAdvancedMappingsPage.ContainsKey(vk))
                {
                    Mappings_ForAdvancedMappingsPage[vk] = new List<Mapping>();
                }

                Mappings_ForAdvancedMappingsPage[vk].Add(mapping);
            }
        }

        private void RefreshOptimisations_Mappings_ForGamepadProcessor()
        {
            Mappings_ForGamepadProcessor = new Dictionary<VirtualKey, List<PhysicalKeyGroup>>();

            foreach (Mapping mapping in Mappings)
            {
                foreach (VirtualKey vk in mapping.VirtualKeys)
                {
                    if (!Mappings_ForGamepadProcessor.ContainsKey(vk))
                    {
                        Mappings_ForGamepadProcessor[vk] = new List<PhysicalKeyGroup>();
                    }

                    PhysicalKeyGroup pkg = new PhysicalKeyGroup(mapping.PhysicalKeys);
                    Mappings_ForGamepadProcessor[vk].Add(pkg);
                }
            }
        }

        private void RefreshOptimisations_Mappings_ForSimpleConfigPage()
        {
            Mappings_ForSimpleConfigPage = new Dictionary<VirtualKey, PhysicalKey>();

            foreach (Mapping mapping in Mappings)
            {
                if (mapping.IsSimpleMapping())
                {
                    VirtualKey vk = CollectionsUtil.First(mapping.VirtualKeys);
                    if (vk != VirtualKey.NULL && !Mappings_ForSimpleConfigPage.ContainsKey(vk))
                    {
                        Mappings_ForSimpleConfigPage[vk] = CollectionsUtil.First(mapping.PhysicalKeys);
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        // STATIC METHODS
        ///////////////////////////////////////////////////////////////////////

        public static UserSettingsV3 ImportValues(string json)
        {
            // if you are wondering about fixing mapping.uid
            // then we do that in UserSettingsContainer.FixMappingUids()
            return JsonConvert.DeserializeObject<UserSettingsV3>(json);
        }

        public static void Print(UserSettingsV3 settings)
        {
            Log.Information("UserSettings.Print()");


            Log.Information("print mappings");
            //TODO rewrite
            /*List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                Log.Information("print Mappings:{VirtKey:" + key + ", PhysicalKeyGroup: " + settings.Mappings[key] + "}");
            }*/

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
