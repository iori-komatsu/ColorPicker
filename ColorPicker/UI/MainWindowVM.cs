using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Linq;
using System.Windows.Media;

namespace ColorPicker.UI {
    public class MainWindowVM : ViewModelBase, IDisposable {
        private readonly ColorPickerModel model;

        public ReadOnlyReactivePropertySlim<Hsv> SelectedHsv { get; }

        public ReadOnlyReactivePropertySlim<Color> SelectedColor { get; }
        public ReadOnlyReactivePropertySlim<Brush> SelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<Brush> InvertedSelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<string> CurrentColorCode { get; }

        public ReadOnlyReactivePropertySlim<double> SelectedHue { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedSaturation { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedValue { get; }

        public ReactiveCommand CopyColorCodeCommand { get; } = new ReactiveCommand();

        public MainWindowVM(ColorPickerModel model) {
            this.model = model;

            SelectedHsv = model
                .ObserveProperty(m => m.CurrentHsv)
                .ToReadOnlyReactivePropertySlim();

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

        public void ChangeCurrentHsv(Hsv newHsv) {
            model.CurrentHsv = newHsv;
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
