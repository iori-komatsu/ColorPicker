using System;
using Reactive.Bindings;
using System.Windows;
using System.Windows.Media;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace ColorPicker {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public ReactiveCommand CopyColorCodeCommand { get; } = new ReactiveCommand();
        public ReactivePropertySlim<double> Hue { get; private set; }

        public MainWindow() {
            InitializeComponent();

            this.DataContext = this;

            CopyColorCodeCommand.Subscribe(_ => CopyColorCode());
            hsvControl.ObserveProperty(ctl => ctl.SelectedColor).Subscribe(_ => UpdateDisplayColor());
            hsvControl.ObserveProperty(ctl => ctl.SelectedHsv).Subscribe(_ => UpdateHsvSliders());
        }

        private void UpdateDisplayColor() {
            colorDisplay.Background = new SolidColorBrush(hsvControl.SelectedColor);
            colorDisplayTextBox.Text = GetCurrentColorCode();
            colorDisplayTextBox.Foreground = new SolidColorBrush(hsvControl.SelectedColor.Invert());
        }

        private string GetCurrentColorCode() {
            Color c = hsvControl.SelectedColor;
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

        private void CopyColorCode() {
            string code = GetCurrentColorCode();
            Clipboard.SetText(code);
        }
    }
}
