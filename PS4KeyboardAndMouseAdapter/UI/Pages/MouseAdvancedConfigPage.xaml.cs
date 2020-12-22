using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    /// <summary>
    /// Interaction logic for MouseAdvancesConfigPage.xaml
    /// </summary>
    public partial class MouseAdvancedConfigPage : UserControl
    {
        public MouseAdvancedConfigPage()
        {
            Log.Information("MouseAdvancedConfigPage constructor START");
            InitializeComponent();
            Log.Information("MouseAdvancedConfigPage constructor FINISHED");
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }

    }
}
