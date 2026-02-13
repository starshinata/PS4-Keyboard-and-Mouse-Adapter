using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.MiscellaneousSettings
{

    public partial class ColourSchemeToggleControl : UserControl
    {
        public ColourSchemeToggleControl()
        {
            InitializeComponent();
            IsLight = ApplicationSettings.GetInstance().ColourSchemeIsLight;
        }

        public bool IsLight
        {
            get => (bool)GetValue(IsLightProperty);
            set => SetValue(IsLightProperty, value);
        }

        public static readonly DependencyProperty IsLightProperty = DependencyProperty.Register("IsLight", typeof(bool), typeof(ColourSchemeToggleControl), new PropertyMetadata(false, OnIsLightChanged));

        private static void OnIsLightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            ApplicationSettings.GetInstance().ColourSchemeIsLight = newValue;
            ColourSchemeChanger.Change(newValue);
        }

    }
}
