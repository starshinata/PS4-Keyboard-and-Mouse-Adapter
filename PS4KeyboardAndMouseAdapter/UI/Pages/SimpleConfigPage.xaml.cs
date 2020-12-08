using PS4KeyboardAndMouseAdapter.UI.Controls;
using Serilog;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class SimpleConfigPage : UserControl
    {

        public GamepadMappingController gamepadMappingController;

        public SimpleConfigPage()
        {
            Log.Information("SimpleConfigPage constructor START");
            InitializeComponent();
            gamepadMappingController = gamepadMappingControllerInner;
            Log.Information("SimpleConfigPage constructor END");
        }
    }
}
