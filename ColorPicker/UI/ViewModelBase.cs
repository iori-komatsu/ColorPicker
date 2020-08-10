using System.ComponentModel;

namespace ColorPicker.UI {
    public abstract class ViewModelBase : INotifyPropertyChanged {
#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore
    }
}
