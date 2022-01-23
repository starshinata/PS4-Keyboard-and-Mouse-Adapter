using Pizza.Common;
using System;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class WelcomePage : System.Windows.Controls.UserControl
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

            ScrollViewer1.ScrollToTop();
        }

        public void CompleteStep2()
        {
            Expander_Step2.IsEnabled = false;
            Expander_Step2.IsExpanded = false;

            Expander_Step3.Visibility = UIConstants.VISIBILITY_VISIBLE;
            Expander_Step3.IsEnabled = true;
            Expander_Step3.IsExpanded = true;
            
            ScrollViewer1.ScrollToTop();
        }

        private void Handler_ExpanderProgegateScroll(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    Expander expander = (Expander)sender;
                    if (expander.Parent != null)
                    {
                        StackPanel grid = (StackPanel)expander.Parent;

                        if (grid.Parent != null)
                        {
                            ScrollViewer scv = (ScrollViewer)grid.Parent;
                            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
                            e.Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("WelcomePage.Handler_ExpanderProgegateScroll failed", ex);
            }
        }
    }
}
