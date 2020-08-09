using System.Windows;
using System.Windows.Media;

namespace ColorPicker {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            UpdateColorDisplayColor();
        }

        private void hsvControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            UpdateColorDisplayColor();
        }

        private void UpdateColorDisplayColor() {
            colorDisplay.Background = new SolidColorBrush(hsvControl.SelectedColor);
            colorDisplayTextBox.Text = FormatColor(hsvControl.SelectedColor);
            colorDisplayTextBox.Foreground = new SolidColorBrush(hsvControl.SelectedColor.Invert());
        }

        private string FormatColor(Color c) {
            return $"#{c.R:x2}{c.G:x2}{c.B:x2}";
        }
    }
}
