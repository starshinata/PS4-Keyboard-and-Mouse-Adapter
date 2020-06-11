using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    /// <summary>
    /// Interaction logic for SliderV.xaml
    /// </summary>
    public partial class SliderV : UserControl
    {
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(SliderV), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(float), typeof(SliderV), new PropertyMetadata(default(float)));

        public float Minimum
        {
            get => (float) GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(int), typeof(SliderV), new PropertyMetadata(default(int)));

        public int Maximum
        {
            get => (int) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(SliderV), new PropertyMetadata(default(double)));

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
            "TickFrequency", typeof(double), typeof(SliderV), new PropertyMetadata(default(double)));

        public double TickFrequency
        {
            get => (double) GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }

        public SliderV()
        {
            InitializeComponent();
        }
    }
}
