﻿using Newtonsoft.Json;
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


        // Note this property will not be serialised (see the save method)
        // this property is just for populating sensible values for the SimpleConfigPage
        public Dictionary<VirtualKey, PhysicalKey> KeyboardMappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();

        // Note this property will not be serialised (see the save method)
        // this property is for simple processing of inputs in GamepadProcessor.cs
        // but wait for this for be needed
        public Dictionary<VirtualKey, PhysicalKey> GamepadProcessorMappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();
                
        public List<Mapping> Mappings { get; set; } = new List<Mapping>();

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

        public UserSettingsV3 Clone()
        {
            // cloning by (serilise to string then deserialise)
            string json = JsonConvert.SerializeObject(this, Formatting.None);
            return JsonConvert.DeserializeObject<UserSettingsV3>(json);
        }

        public bool MappingsContainsKey(VirtualKey vk)
        {
            //TODO rewrite
            //return Mappings.ContainsKey(vk) && Mappings[vk] != null;
            return false;
        }

        public void Print()
        {
            UserSettingsV3.Print(this);
        }

        public void RefreshOptimisations()
        {

            //TODO rewrite

            /*List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
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
            }*/
        }

        ///////////////////////////////////////////////////////////////////////
        // STATIC METHODS
        ///////////////////////////////////////////////////////////////////////
        public static UserSettingsV3 ImportValues(string json)
        {
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