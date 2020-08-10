using System.ComponentModel;

namespace ColorPicker {
    public class ColorPickerModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private Hsv currentHsv;

        public Hsv CurrentHsv {
            get => currentHsv;
            set {
                if (currentHsv != value) {
                    currentHsv = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentHsv)));
                }
            }
        }
    }
}
