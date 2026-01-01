using System.Windows;
using System.Windows.Input;
using TrainMeX.ViewModels;

namespace TrainMeX.Windows {
    public partial class SettingsWindow : Window {
        private SettingsViewModel _viewModel;

        public SettingsWindow() {
            InitializeComponent();
            _viewModel = new SettingsViewModel();
            _viewModel.RequestClose += (s, e) => {
                // Apply settings changes in owner window if it's LauncherWindow
                if (Owner is LauncherWindow launcherWindow) {
                    launcherWindow.ReloadHotkeys();
                    launcherWindow.ApplyAlwaysOnTopSetting();
                }
                this.Close();
            };
            DataContext = _viewModel;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
