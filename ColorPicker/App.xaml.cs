using System.Windows;

namespace ColorPicker {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            var model = new ColorPickerModel();
            var viewModel = new UI.MainWindowVM(model);
            var window = new UI.MainWindow(viewModel);
            model.CurrentHsv = new Hsv(0, 1, 1);
            window.Show();
        }
    }
}
