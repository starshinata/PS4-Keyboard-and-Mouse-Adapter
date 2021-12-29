using Pizza.Common;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Linq;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class EmulationPickerControl : System.Windows.Controls.UserControl
    {


        public static readonly int ONLY_PROCESS_INJECTION = 1; //EmulationConstants.ONLY_PROCESS_INJECTION;
        public static readonly int ONLY_VIGEM = 2;//EmulationConstants.ONLY_VIGEM;
        public static readonly int VIGEM_AND_PROCESS_INJECTION = 3; // EmulationConstants.VIGEM_AND_PROCESS_INJECTION;

        public EmulationPickerControl()
        {
            InitializeComponent();
            SetInitialChecked();
        }

        private int GetValue()
        {
            RadioButton checkedRadioButton = RadioButtonGroup.Children.OfType<RadioButton>().
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

        private void SaveValueInApplicationSettings(int value)
        {
            ApplicationSettings.GetInstance().EmulationMode = value;
        }

        private void SetInitialChecked()
        {
            int initialTag = ApplicationSettings.GetInstance().EmulationMode;
            if (EmulationConstants.IsValidValue(initialTag))
            {
                SetCheckedForTag(initialTag);
            }
            else
            {
                SetCheckedForTag(EmulationConstants.VIGEM_AND_PROCESS_INJECTION);
            }
        }

        private void SetCheckedForTag(int tag)
        {
            RadioButtonGroup.Children.OfType<RadioButton>().
             Where(n => Int32.Parse(n.Tag.ToString()) == tag).ToList().
             ForEach(n => n.IsChecked = true);
        }

    }
}
