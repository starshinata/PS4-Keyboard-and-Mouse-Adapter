using Pizza.Common;
using PS4KeyboardAndMouseAdapter.backend;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System.Linq;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class EmulationPickerControl : System.Windows.Controls.UserControl
    {
        public EmulationPickerControl()
        {
            InitializeComponent();
            SetInitialChecked();
        }

        private string GetValue()
        {
            RadioButton checkedRadioButton = RadioButtonGroup.Children.OfType<RadioButton>().
                Where(n => n.IsChecked == true).First();

            if (checkedRadioButton != null && checkedRadioButton.Tag != null)
            {
                string tag = checkedRadioButton.Tag.ToString();
                if (EmulationConstants.IsValidValue(tag))
                {
                    Log.Information("EmulationPickerControl tag is " + tag);
                    return tag;
                }
            }
            Log.Information("EmulationPickerControl tag is null");
            return null;
        }

        public void GetValueAndSaveValueInApplicationSettings()
        {
            SaveValueInApplicationSettings(GetValue());
        }

        private void SaveValueInApplicationSettings(string value)
        {
            ApplicationSettings.GetInstance().EmulationMode = value;
        }

        private void SetInitialChecked()
        {
            string initialTag = ApplicationSettings.GetInstance().EmulationMode;
            if (initialTag != null && EmulationConstants.IsValidValue(initialTag))
            {
                SetCheckedForTag(initialTag);
            }
            else
            {
                SetCheckedForTag(EmulationConstants.VIGEM_AND_PROCESS_INJECTION);
            }
        }

        private void SetCheckedForTag(string tag)
        {
            RadioButtonGroup.Children.OfType<RadioButton>().
             Where(n => n.Tag.ToString().Equals(tag)).ToList().
             ForEach(n => n.IsChecked = true);
        }

    }
}
