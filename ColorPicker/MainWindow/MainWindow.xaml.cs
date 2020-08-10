using System;
using System.Windows;
using System.Reactive.Linq;

namespace ColorPicker.MainWindow {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            ViewModel.CopyColorCodeCommand.Subscribe(_ => CopyColorCode());
            ViewModel.SelectedHsv.Value = new Hsv(0, 1, 1);
        }

        private MainWindowVM ViewModel {
            get => (MainWindowVM)Resources["ViewModel"];
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            ViewModel.Dispose();
        }

        private void sliderH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = ViewModel.SelectedHsv.Value;
            ViewModel.SelectedHsv.Value = new Hsv((float)sliderH.Value, current.S, current.V);
        }

        private void sliderS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = ViewModel.SelectedHsv.Value;
            ViewModel.SelectedHsv.Value = new Hsv(current.H, (float)sliderS.Value, current.V);
        }

        private void sliderV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = ViewModel.SelectedHsv.Value;
            ViewModel.SelectedHsv.Value = new Hsv(current.H, current.S, (float)sliderV.Value);
        }

        private void CopyColorCode() {
            Clipboard.SetText(ViewModel.CurrentColorCode.Value);
        }
    }
}
