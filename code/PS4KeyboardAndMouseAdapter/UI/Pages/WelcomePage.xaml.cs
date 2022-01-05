using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class WelcomePage : UserControl
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        public void CompleteStep1()
        {
            Expander_Step1.IsEnabled = false;
            Expander_Step1.IsExpanded = false;

            Expander_Step2.Visibility = UIConstants.VISIBILITY_VISIBLE;
            Expander_Step2.IsEnabled = true;
            Expander_Step2.IsExpanded = true;
        }

        public void CompleteStep2()
        {
            Expander_Step2.IsEnabled = false;
            Expander_Step2.IsExpanded = false;

            Expander_Step3.Visibility = UIConstants.VISIBILITY_VISIBLE;
            Expander_Step3.IsEnabled = true;
            Expander_Step3.IsExpanded = true;
        }

    }
}
