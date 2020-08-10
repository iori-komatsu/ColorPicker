using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.Windows.Media;

namespace ColorPicker.UI {
    class MainWindowVM : ViewModelBase, IDisposable {

        public ReactivePropertySlim<Hsv> SelectedHsv { get; } = new ReactivePropertySlim<Hsv>();

        public ReadOnlyReactivePropertySlim<Color> SelectedColor { get; }
        public ReadOnlyReactivePropertySlim<Brush> SelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<Brush> InvertedSelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<string> CurrentColorCode { get; }

        public ReadOnlyReactivePropertySlim<double> SelectedHue { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedSaturation { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedValue { get; }

        public ReactiveCommand CopyColorCodeCommand { get; } = new ReactiveCommand();

        public MainWindowVM() {
            SelectedColor = SelectedHsv
                .Select(hsv => ColorF.FromHsv(hsv.H, hsv.S, hsv.V).ToColor())
                .ToReadOnlyReactivePropertySlim();
            SelectedColorBrush = SelectedColor
                .Select(color => (Brush)new SolidColorBrush(color))
                .ToReadOnlyReactivePropertySlim();
            InvertedSelectedColorBrush = SelectedColor
                .Select(color => (Brush)new SolidColorBrush(color.Invert()))
                .ToReadOnlyReactivePropertySlim();
            CurrentColorCode = SelectedColor
                .Select(color => $"#{color.R:x2}{color.G:x2}{color.B:x2}")
                .ToReadOnlyReactivePropertySlim();

            SelectedHue = SelectedHsv
                .Select(hsv => (double)hsv.H)
                .ToReadOnlyReactivePropertySlim();
            SelectedSaturation = SelectedHsv
                .Select(hsv => (double)hsv.S)
                .ToReadOnlyReactivePropertySlim();
            SelectedValue = SelectedHsv
                .Select(hsv => (double)hsv.V)
                .ToReadOnlyReactivePropertySlim();
        }

        public void Dispose() {
            CopyColorCodeCommand.Dispose();

            SelectedValue.Dispose();
            SelectedSaturation.Dispose();
            SelectedHue.Dispose();

            CurrentColorCode.Dispose();
            InvertedSelectedColorBrush.Dispose();
            SelectedColorBrush.Dispose();
            SelectedColor.Dispose();

            SelectedHsv.Dispose();
        }
    }
}
