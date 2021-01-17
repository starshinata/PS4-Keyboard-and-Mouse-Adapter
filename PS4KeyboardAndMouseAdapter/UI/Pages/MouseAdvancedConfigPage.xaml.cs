using System.Windows;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class MouseAdvancedConfigPage : UserControl
    {
        public MouseAdvancedConfigPage()
        {
            InitializeComponent();
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }

    }
}
