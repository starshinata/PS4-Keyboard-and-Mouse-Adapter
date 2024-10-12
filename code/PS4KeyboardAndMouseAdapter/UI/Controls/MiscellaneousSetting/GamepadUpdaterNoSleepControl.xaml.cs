using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.MiscellaneousSettings
{
    public partial class GamepadUpdaterNoSleepControl : UserControl
    {
        public GamepadUpdaterNoSleepControl()
        {
            InitializeComponent();
            CalculateIsControlEnabled();
        }

        public void CalculateIsControlEnabled()
        {
            int EmulationMode = ApplicationSettings.GetInstance().EmulationMode;
            if (EmulationConstants.IsValidValue(EmulationMode) &&
                (EmulationMode == EmulationConstants.ONLY_VIGEM ||
                EmulationMode == EmulationConstants.VIGEM_AND_PROCESS_INJECTION))
            {
                Element_Toggle.Visibility = UIConstants.VISIBILITY_VISIBLE;
                Element_TextBlock_Description.Visibility = UIConstants.VISIBILITY_VISIBLE;
                Element_TextBlock_Warning.Visibility = UIConstants.VISIBILITY_COLLAPSED;
            }
            else
            {
                Element_Toggle.Visibility = UIConstants.VISIBILITY_COLLAPSED;
                Element_TextBlock_Description.Visibility = UIConstants.VISIBILITY_COLLAPSED;
                Element_TextBlock_Warning.Visibility = UIConstants.VISIBILITY_VISIBLE;
            }
        }

        #region testonly
        public UIElement testonly_GetElement_TextBlock_Description() { return Element_TextBlock_Description; }
        public UIElement testonly_GetElement_TextBlock_Warning() { return Element_TextBlock_Warning; }
        public UIElement testonly_GetElement_Toggle() { return Element_Toggle; }
        #endregion
    }
}
