using System.Windows;
using System.Windows.Media;

namespace ColorPicker {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            UpdateDisplayColor();
            UpdateHsvSliders();
        }

        private void hsvControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            UpdateDisplayColor();
            UpdateHsvSliders();
        }

        private void UpdateDisplayColor() {
            colorDisplay.Background = new SolidColorBrush(hsvControl.SelectedColor);
            colorDisplayTextBox.Text = FormatColor(hsvControl.SelectedColor);
            colorDisplayTextBox.Foreground = new SolidColorBrush(hsvControl.SelectedColor.Invert());
        }

        private string FormatColor(Color c) {
            return $"#{c.R:x2}{c.G:x2}{c.B:x2}";
        }

        private void UpdateHsvSliders() {
            sliderH.Value = hsvControl.SelectedHsv.H;
            sliderS.Value = hsvControl.SelectedHsv.S;
            sliderV.Value = hsvControl.SelectedHsv.V;
        }

        private void sliderH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = hsvControl.SelectedHsv;
            hsvControl.SelectedHsv = new Hsv((float)sliderH.Value, current.S, current.V);
        }

        private void sliderS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = hsvControl.SelectedHsv;
            hsvControl.SelectedHsv = new Hsv(current.H, (float)sliderS.Value, current.V);
        }

        private void sliderV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = hsvControl.SelectedHsv;
            hsvControl.SelectedHsv = new Hsv(current.H, current.S, (float)sliderV.Value);
        }
    }
}
