using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController
{
    public partial class AnalogStickMouseInputs : UserControl
    {

        public AnalogStickMouseInputs()
        {
            InitializeComponent();

            // labels for debugging
            stickLeft.SetLabel("Left");
            stickRight.SetLabel("Right");
        }

        public void WindowResized()
        {
            stickLeft.RepaintStickCenter();
            stickRight.RepaintStickCenter();
        }
    }
}
