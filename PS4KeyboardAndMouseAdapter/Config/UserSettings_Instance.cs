using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public partial class UserSettings : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //Note this property will not be serialised (see the save method)
        public Dictionary<VirtualKey, PhysicalKey> KeyboardMappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        // false if we need to migrate
        // true means we can ignore
        // default if false until we find a value
        public bool Version_1_0_12_OrGreater { get; set; } = false;

        public Dictionary<VirtualKey, PhysicalKeyGroup> Mappings { get; set; } = new Dictionary<VirtualKey, PhysicalKeyGroup>();

        public int AdvancedMappingPage_MappingsToShow { get; set; } = 4;

        public bool AimToggle { get; set; } = false;

        // time in milliseconds, after toggling aim on/off, before we retrigger on/off
        // set it too low, and you will be constantly toggline between aiming and not aiming
        public int AimToggleRetoggleDelay { get; set; } = 500;

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

        public double MouseXAxisSensitivityAimModifier { get; set; } = 1;
        public double MouseXAxisSensitivityLookModifier { get; set; } = 1;
        public double MouseXAxisSensitivityMax { get; set; } = 100;

        public double MouseYAxisSensitivityAimModifier { get; set; } = 1;
        public double MouseYAxisSensitivityLookModifier { get; set; } = 1;
        public double MouseYAxisSensitivityMax { get; set; } = 100;
        
        public double RemotePlayVolume { get; set; } = 1;

        //TODO do we still need this ?
        public double XYRatio { get; set; } = 1;

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

      
        ////////////////////////////////////////////////////////////////////////////


        protected UserSettings()
        {
            MousePollingRate = 60;
        }

        public UserSettings Clone()
        {
            // cloning by (serilise to string then deserialise)
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return JsonConvert.DeserializeObject<UserSettings>(json);
        }

        public bool MappingsContainsKey(VirtualKey vk) {
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

    }
}

