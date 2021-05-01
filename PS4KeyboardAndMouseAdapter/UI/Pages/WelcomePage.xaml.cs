using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
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

    }
}
