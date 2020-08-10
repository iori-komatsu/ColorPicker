using System;
using System.Windows;
using System.Reactive.Linq;

namespace ColorPicker.UI {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        private readonly MainWindowVM viewModel;

        public MainWindow(MainWindowVM viewModel) {
            this.viewModel = viewModel;
            DataContext = viewModel;

            InitializeComponent();

            viewModel.CopyColorCodeCommand.Subscribe(_ => CopyColorCode());
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            viewModel.Dispose();
        }

        private void sliderH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = viewModel.SelectedHsv.Value;
            viewModel.SelectedHsv.Value = new Hsv((float)sliderH.Value, current.S, current.V);
        }

        private void sliderS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = viewModel.SelectedHsv.Value;
            viewModel.SelectedHsv.Value = new Hsv(current.H, (float)sliderS.Value, current.V);
        }

        private void sliderV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            Hsv current = viewModel.SelectedHsv.Value;
            viewModel.SelectedHsv.Value = new Hsv(current.H, current.S, (float)sliderV.Value);
        }

        private void CopyColorCode() {
            Clipboard.SetText(viewModel.CurrentColorCode.Value);
        }
    }
}
