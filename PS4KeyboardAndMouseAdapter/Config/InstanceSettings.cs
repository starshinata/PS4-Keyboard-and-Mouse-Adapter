using System.ComponentModel;

namespace PS4KeyboardAndMouseAdapter.Config
{
    // a collection of settings that will die when application is closed
    public class InstanceSettings : INotifyPropertyChanged
    {
        private static readonly InstanceSettings thisInstance = new InstanceSettings();

        public static void BroadcastRefresh()
        {
            thisInstance.PropertyChanged(thisInstance, new PropertyChangedEventArgs(""));
        }

        public static InstanceSettings GetInstance()
        {
            return thisInstance;
        }

        //////////////////////////////////////////////////////////////////////
        
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public bool EnableMouseInput { get; set; } = false;

    }
}
