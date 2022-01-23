using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using PS4RemotePlayInjection;
using Serilog;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.MiscellaneousSettings
{
    public partial class RemotePlayToolbarSettingsControl : UserControl
    {

        public RemotePlayToolbarSettingsControl()
        {
            InitializeComponent();
            CalculateIsControlEnabled();
        }

        public void CalculateIsControlEnabled()
        {
            int EmulationMode = ApplicationSettings.GetInstance().EmulationMode;
            if (EmulationConstants.IsValidValue(EmulationMode) && EmulationMode == EmulationConstants.ONLY_VIGEM)
            {
                Element_Toggle.Visibility = UIConstants.VISIBILITY_COLLAPSED;
                Element_TextBlock_Description.Visibility = UIConstants.VISIBILITY_COLLAPSED;
                Element_TextBlock_Warning.Visibility = UIConstants.VISIBILITY_VISIBLE; 
                
            }
            else
            {
                Element_Toggle.Visibility = UIConstants.VISIBILITY_VISIBLE;
                Element_TextBlock_Description.Visibility = UIConstants.VISIBILITY_VISIBLE;
                Element_TextBlock_Warning.Visibility = UIConstants.VISIBILITY_COLLAPSED;
            }
        }

        private void Handler_ToolBarVisibleToggle(object sender, System.Windows.RoutedEventArgs e)
        {
            Log.Debug("RemotePlayToolbarSettingsControl.Handler_ToolBarVisibleToggle");
            UtilityData.IsToolBarVisible = !UtilityData.IsToolBarVisible;
        }

        #region testonly
        public UIElement testonly_GetElement_TextBlock_Description() { return Element_TextBlock_Description; }
        public UIElement testonly_GetElement_TextBlock_Warning() { return Element_TextBlock_Warning; }
        public UIElement testonly_GetElement_Toggle() { return Element_Toggle; }
        #endregion
    }
}
