using System.Linq;
using System.Windows;

namespace PS4KeyboardAndMouseAdapter
{
    class MouseSettingsViewModel
    {
        private readonly MainViewModel mainVm;

        public double MaxMouseDistance => mainVm.Settings.MouseMaxDistance;

        public double MouseDistanceMinRange
        {
            get => mainVm.Settings.MouseDistanceLowerRange;
            set => mainVm.Settings.MouseDistanceLowerRange = value;
        }

        public double MouseDistanceMaxRange
        {
            get => mainVm.Settings.MouseDistanceUpperRange;
            set => mainVm.Settings.MouseDistanceUpperRange = value;
        }

        public double HorizontalVerticalRatio
        {
            get => mainVm.Settings.XYRatio;
            set => mainVm.Settings.XYRatio = value;
        }

        public MouseSettingsViewModel()
        {
            mainVm = Application.Current.Windows.OfType<MainWindowView>().FirstOrDefault()?.vm;
        }
    }
}
