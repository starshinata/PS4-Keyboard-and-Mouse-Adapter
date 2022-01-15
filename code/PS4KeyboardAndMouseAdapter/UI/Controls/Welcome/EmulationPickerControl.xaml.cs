using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class EmulationPickerControl : System.Windows.Controls.UserControl
    {

        public static readonly int ONLY_PROCESS_INJECTION = EmulationConstants.ONLY_PROCESS_INJECTION;
        public static readonly int ONLY_VIGEM = EmulationConstants.ONLY_VIGEM;
        public static readonly int VIGEM_AND_PROCESS_INJECTION = EmulationConstants.VIGEM_AND_PROCESS_INJECTION;

        private bool IsVigemInstalled = false;

        public EmulationPickerControl()
        {
            InitializeComponent();

            VigemManager vigemManager = new VigemManager();
            ShouldShowVigemInstallWarning(vigemManager);
            SetInitialRadioChecked();
            DisableUnavailableRadioButtons();
        }

        public void DisableUnavailableRadioButtons()
        {
            // if vigem isnt installed then we can only do process injection
            // so disable the other options
            if (!IsVigemInstalled)
            {
                SetRadioDisabledForTag(VIGEM_AND_PROCESS_INJECTION);
                SetRadioDisabledForTag(ONLY_VIGEM);
            }
        }

        public int GetValue()
        {
            RadioButton checkedRadioButton = Panel_RadioButtonGroup.Children.OfType<RadioButton>().
                Where(n => n.IsChecked == true).First();

            if (checkedRadioButton != null && checkedRadioButton.Tag != null)
            {
                int tag = Int32.Parse(checkedRadioButton.Tag.ToString());
                if (EmulationConstants.IsValidValue(tag))
                {
                    Log.Information("EmulationPickerControl tag is " + tag);
                    return tag;
                }
            }
            Log.Information("EmulationPickerControl tag is null");
            return -1;
        }

        public void GetValueAndSaveValueInApplicationSettings()
        {
            SaveValueInApplicationSettings(GetValue());
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void SaveValueInApplicationSettings(int value)
        {
            ApplicationSettings.GetInstance().EmulationMode = value;
        }

        public void SetInitialRadioChecked()
        {
            // if vigem is installed there are no restrictions
            // use the last known setting (assuming the are valid)
            if (IsVigemInstalled)
            {
                int initialTag = ApplicationSettings.GetInstance().EmulationMode;
                if (EmulationConstants.IsValidValue(initialTag))
                {
                    SetRadioCheckedForTag(initialTag);
                }
                else
                {
                    SetRadioCheckedForTag(EmulationConstants.VIGEM_AND_PROCESS_INJECTION);
                }
            }
            // if vigem isnt installed then we can only do process injection
            else
            {
                SetRadioCheckedForTag(EmulationConstants.ONLY_PROCESS_INJECTION);
            }
        }

        private void SetRadioCheckedForTag(int tag)
        {
            Panel_RadioButtonGroup.Children.OfType<RadioButton>().
             Where(n => Int32.Parse(n.Tag.ToString()) == tag).ToList().
             ForEach(n => n.IsChecked = true);
        }

        private void SetRadioDisabledForTag(int tag)
        {
            Panel_RadioButtonGroup.Children.OfType<RadioButton>().
             Where(n => Int32.Parse(n.Tag.ToString()) == tag).ToList().
             ForEach(n => n.IsEnabled = false);
        }

        public void ShouldShowVigemInstallWarning(VigemManager vigemManager)
        {
            Panel_VigemNotInstalled.Visibility = UIConstants.VISIBILITY_COLLAPSED;

            IsVigemInstalled = vigemManager.IsVigemDriverInstalled();
            if (!IsVigemInstalled)
            {
                Panel_VigemNotInstalled.Visibility = UIConstants.VISIBILITY_VISIBLE;
            }
        }


        #region testonly

        public List<RadioButton> testonly_getRadioButtons()
        {
            return Panel_RadioButtonGroup.Children.OfType<RadioButton>().ToList();
        }

        public bool testonly_isVisible_Panel_VigemNotInstalled()
        {
            return Panel_VigemNotInstalled.Visibility == UIConstants.VISIBILITY_VISIBLE;
        }

        public void testonly_setIsVigemInstalled(bool value)
        {
            IsVigemInstalled = value;
        }

        #endregion
    }
}
