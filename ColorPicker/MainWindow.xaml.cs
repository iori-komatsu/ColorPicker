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

        public ReadOnlyReactivePropertySlim<string> CurrentColorCode { get; }
        public ReadOnlyReactivePropertySlim<Brush> SelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<Brush> InvertedSelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedHue { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedSaturation { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedValue { get; }

        public MainWindow() {
            InitializeComponent();

            this.DataContext = this;

            CopyColorCodeCommand.Subscribe(_ => CopyColorCode());

            var propSelectedColor = hsvControl.ObserveProperty(ctl => ctl.SelectedColor);
            var propSelectedHsv = hsvControl.ObserveProperty(ctl => ctl.SelectedHsv);

            CurrentColorCode = propSelectedColor
                .Select(color => $"#{color.R:x2}{color.G:x2}{color.B:x2}")
                .ToReadOnlyReactivePropertySlim();
            SelectedColorBrush = propSelectedColor
                .Select(color => (Brush)new SolidColorBrush(color))
                .ToReadOnlyReactivePropertySlim();
            InvertedSelectedColorBrush = propSelectedColor
                .Select(color => (Brush)new SolidColorBrush(color.Invert()))
                .ToReadOnlyReactivePropertySlim();
            SelectedHue = propSelectedHsv
                .Select(hsv => (double)hsv.H)
                .ToReadOnlyReactivePropertySlim();
            SelectedSaturation = propSelectedHsv
                .Select(hsv => (double)hsv.S)
                .ToReadOnlyReactivePropertySlim();
            SelectedValue = propSelectedHsv
                .Select(hsv => (double)hsv.V)
                .ToReadOnlyReactivePropertySlim();

            hsvControl.SelectedHsv = new Hsv(0, 1, 1);
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
            Clipboard.SetText(CurrentColorCode.Value);
        }
    }
}
