using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PS4KeyboardAndMouseAdapter.UI
{
    class UITools
    {
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        public static void ShowWindow<T>() where T : Window, new()
        {
            T window = Application.Current.Windows.OfType<T>().FirstOrDefault();

            if (window == null)
                new T {Owner = Application.Current.MainWindow}.Show();
            else 
                window.Focus();
        }
    }
}
