using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;

namespace ColorPicker.UI {
    public class MainWindowVM : ViewModelBase, IDisposable {
        private readonly ColorPickerModel model;
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        public ReadOnlyReactivePropertySlim<Hsv> SelectedHsv { get; }

        public ReadOnlyReactivePropertySlim<Color> SelectedColor { get; }
        public ReadOnlyReactivePropertySlim<Brush> SelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<Brush> InvertedSelectedColorBrush { get; }
        public ReadOnlyReactivePropertySlim<string> CurrentColorCode { get; }

        public ReadOnlyReactivePropertySlim<double> SelectedHue { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedSaturation { get; }
        public ReadOnlyReactivePropertySlim<double> SelectedValue { get; }

        public ReactiveCommand CopyColorCodeCommand { get; }

        public MainWindowVM(ColorPickerModel model) {
            this.model = model;

            SelectedHsv = model
                .ObserveProperty(m => m.CurrentHsv)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            SelectedColor = SelectedHsv
                .Select(hsv => ColorF.FromHsv(hsv.H, hsv.S, hsv.V).ToColor())
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            SelectedColorBrush = SelectedColor
                .Select(color => (Brush)new SolidColorBrush(color))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            InvertedSelectedColorBrush = SelectedColor
                .Select(color => (Brush)new SolidColorBrush(color.Invert()))
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            CurrentColorCode = SelectedColor
                .Select(color => $"#{color.R:x2}{color.G:x2}{color.B:x2}")
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            SelectedHue = SelectedHsv
                .Select(hsv => (double)hsv.H)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            SelectedSaturation = SelectedHsv
                .Select(hsv => (double)hsv.S)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            SelectedValue = SelectedHsv
                .Select(hsv => (double)hsv.V)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(disposable);

            CopyColorCodeCommand = new ReactiveCommand().AddTo(disposable);

            CopyColorCodeCommand
                .Subscribe(_ => CopyColorCode())
                .AddTo(disposable);
        }

        public void ChangeCurrentHsv(Hsv newHsv) {
            model.CurrentHsv = newHsv;
        }

        private void CopyColorCode() {
            Clipboard.SetText(CurrentColorCode.Value);
        }

        public void Dispose() {
            disposable.Dispose();
        }
    }
}
