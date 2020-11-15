using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Button = System.Windows.Controls.Button;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{

    public partial class AnalogStickBindings : UserControl
    {

        private MainWindowView mwv;

        public static readonly DependencyProperty StickContextProperty = DependencyProperty.Register(
                "StickContext", typeof(string), typeof(AnalogStickBindings), new PropertyMetadata(default(string)));

        public string StickContext
        {
            get => (string)GetValue(StickContextProperty);
            set => SetValue(StickContextProperty, value);
        }

        ////////////////////////////////

        public AnalogStickBindings()
        {
            InitializeComponent();
        }

        private void InitializeButtons()
        {
            InitializeButton("buttonDown");
            InitializeButton("buttonL3R3");
            InitializeButton("buttonLeft");
            InitializeButton("buttonRight");
            InitializeButton("buttonUp");
        }

        private void InitializeButton(string buttonName)
        {
            Button button = FindName(buttonName) as Button;
            if (button != null)
            {
                VirtualKey virtualKey = GetVirtualKey(button);

                button.Tag = virtualKey;
                Binding dataBinding = new Binding("Settings.Mappings[" + virtualKey + "]");
                button.SetBinding(ContentProperty, dataBinding);
            }
        }

        public VirtualKey GetVirtualKey(Button button)
        {

            if (button == null || button.Name == null)
            {
                return VirtualKey.NULL;
            }

            if (StickContext == "LEFT")
            {
                if (button.Name == "buttonDown")
                    return VirtualKey.LeftStickDown;

                if (button.Name == "buttonLeft")
                    return VirtualKey.LeftStickLeft;

                if (button.Name == "buttonRight")
                    return VirtualKey.LeftStickRight;

                if (button.Name == "buttonUp")
                    return VirtualKey.LeftStickUp;

                if (button.Name == "buttonL3R3")
                    return VirtualKey.L3;

            }
            else if (StickContext == "RIGHT")
            {

                if (button.Name == "buttonDown")
                    return VirtualKey.RightStickDown;

                if (button.Name == "buttonLeft")
                    return VirtualKey.RightStickLeft;

                if (button.Name == "buttonRight")
                    return VirtualKey.RightStickRight;

                if (button.Name == "buttonUp")
                    return VirtualKey.RightStickUp;

                if (button.Name == "buttonL3R3")
                    return VirtualKey.R3;
            }

            // you should NEVER get this, if you do you fudged the code
            return VirtualKey.NULL;
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            mwv = (MainWindowView)((Grid)((Grid)this.Parent).Parent).Parent;

            Button button = (Button)sender;
            mwv.lastClickedButton = button;

            mwv.WaitingForKeyPress_Show();
        }

        private void Handler_ButtonLoaded(object sender, RoutedEventArgs e)
        {
            InitializeButtons();
        }

    }
}
