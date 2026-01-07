using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Windows.Input;
using TrainMeX.Classes;
using System.IO;

namespace TrainMeX.ViewModels {
    /// <summary>
    /// ViewModel for the Settings window
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class SettingsViewModel : ObservableObject {
        private double _defaultOpacity;
        private double _defaultVolume;

        private bool _launcherAlwaysOnTop;
        private bool _panicHotkeyCtrl;
        private bool _panicHotkeyShift;
        private bool _panicHotkeyAlt;
        private string _panicHotkeyKey;
        private ScreenViewer _selectedDefaultMonitor;
        private bool _alwaysOpaque;

        private bool _rememberLastPlaylist;
        private bool _rememberFilePosition;
        private bool _enableHotScreenIntegration;
        private int _hotScreenOffsetX;
        private int _hotScreenOffsetY;
        private bool _hotScreenUseClientArea;
        private bool _isGeneralExpanded;
        private bool _isPlaybackExpanded;
        private bool _isApplicationExpanded;
        private bool _isHotkeysExpanded;
        private bool _isHistoryExpanded;

        public bool IsGeneralExpanded {
            get => _isGeneralExpanded;
            set {
                if (SetProperty(ref _isGeneralExpanded, value) && value) {
                    CollapseOthers(nameof(IsGeneralExpanded));
                    App.Settings.LastExpandedSection = nameof(IsGeneralExpanded);
                }
            }
        }

        public bool IsPlaybackExpanded {
            get => _isPlaybackExpanded;
            set {
                if (SetProperty(ref _isPlaybackExpanded, value) && value) {
                    CollapseOthers(nameof(IsPlaybackExpanded));
                    App.Settings.LastExpandedSection = nameof(IsPlaybackExpanded);
                }
            }
        }

        public bool IsApplicationExpanded {
            get => _isApplicationExpanded;
            set {
                if (SetProperty(ref _isApplicationExpanded, value) && value) {
                    CollapseOthers(nameof(IsApplicationExpanded));
                    App.Settings.LastExpandedSection = nameof(IsApplicationExpanded);
                }
            }
        }

        public bool IsHotkeysExpanded {
            get => _isHotkeysExpanded;
            set {
                if (SetProperty(ref _isHotkeysExpanded, value) && value) {
                    CollapseOthers(nameof(IsHotkeysExpanded));
                    App.Settings.LastExpandedSection = nameof(IsHotkeysExpanded);
                }
            }
        }

        public bool IsHistoryExpanded {
            get => _isHistoryExpanded;
            set {
                if (SetProperty(ref _isHistoryExpanded, value) && value) {
                    CollapseOthers(nameof(IsHistoryExpanded));
                    App.Settings.LastExpandedSection = nameof(IsHistoryExpanded);
                }
            }
        }

        private void CollapseOthers(string current) {
            if (current != nameof(IsGeneralExpanded)) IsGeneralExpanded = false;
            if (current != nameof(IsPlaybackExpanded)) IsPlaybackExpanded = false;
            if (current != nameof(IsApplicationExpanded)) IsApplicationExpanded = false;
            if (current != nameof(IsHotkeysExpanded)) IsHotkeysExpanded = false;
            if (current != nameof(IsHistoryExpanded)) IsHistoryExpanded = false;
        }

        // Taboo Settings


        // Modifier flags
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_ALT = 0x0001;

        [SupportedOSPlatform("windows")]
        public SettingsViewModel() {
            // Load current settings
            var settings = App.Settings;
            _defaultOpacity = settings.DefaultOpacity;
            _defaultVolume = settings.DefaultVolume;

            _launcherAlwaysOnTop = settings.LauncherAlwaysOnTop;
            
            // Load panic hotkey settings
            _panicHotkeyCtrl = (settings.PanicHotkeyModifiers & MOD_CONTROL) != 0;
            _panicHotkeyShift = (settings.PanicHotkeyModifiers & MOD_SHIFT) != 0;
            _panicHotkeyAlt = (settings.PanicHotkeyModifiers & MOD_ALT) != 0;
            _panicHotkeyKey = settings.PanicHotkeyKey ?? "End";
            _alwaysOpaque = settings.AlwaysOpaque;

            _rememberLastPlaylist = settings.RememberLastPlaylist;
            _rememberFilePosition = settings.RememberFilePosition;
            _enableHotScreenIntegration = settings.EnableHotScreenIntegration;
            _hotScreenOffsetX = settings.HotScreenOffsetX;
            _hotScreenOffsetY = settings.HotScreenOffsetY;
            _hotScreenUseClientArea = settings.HotScreenUseClientArea;

            
            // Load and set the last expanded section
            var lastSection = settings.LastExpandedSection ?? nameof(IsGeneralExpanded);
            _isGeneralExpanded = lastSection == nameof(IsGeneralExpanded);
            _isPlaybackExpanded = lastSection == nameof(IsPlaybackExpanded);
            _isApplicationExpanded = lastSection == nameof(IsApplicationExpanded);
            _isHotkeysExpanded = lastSection == nameof(IsHotkeysExpanded);
            _isHistoryExpanded = lastSection == nameof(IsHistoryExpanded);

            // Ensure at least one is expanded if the loaded value was invalid
            if (!_isGeneralExpanded && !_isPlaybackExpanded && !_isApplicationExpanded && !_isHotkeysExpanded && !_isHistoryExpanded) {
                _isGeneralExpanded = true;
            }



            // Load available monitors
            AvailableMonitors = new ObservableCollection<ScreenViewer>();
            RefreshAvailableMonitors();
            
            // Load default monitor from settings
            if (!string.IsNullOrEmpty(settings.DefaultMonitorDeviceName)) {
                _selectedDefaultMonitor = AvailableMonitors.FirstOrDefault(m => m.DeviceName == settings.DefaultMonitorDeviceName);
            }

            OkCommand = new RelayCommand(Ok);
            CancelCommand = new RelayCommand(Cancel);
            OpenKoFiCommand = new RelayCommand(OpenKoFi);
            ResetPositionsCommand = new RelayCommand(ResetPositions);
        }

        private void ResetPositions(object obj) {
            if (System.Windows.MessageBox.Show("Are you sure you want to clear all saved video positions? This cannot be undone.", "Reset Playback History", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.Yes) {
                PlaybackPositionTracker.Instance.ClearAllPositions();
            }
        }

        private void OpenKoFi(object obj) {
            try {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                    FileName = "https://ko-fi.com/vexfromdestiny",
                    UseShellExecute = true
                });
            } catch (System.Exception ex) {
                Logger.Error("Failed to open Ko-Fi link", ex);
            }
        }

        [SupportedOSPlatform("windows")]
        private void RefreshAvailableMonitors() {
            AvailableMonitors.Clear();
            try {
                // Add "All Screens" option first
                AvailableMonitors.Add(ScreenViewer.CreateAllScreens());
                
                var screens = WindowServices.GetAllScreenViewers();
                foreach (var screen in screens) {
                    AvailableMonitors.Add(screen);
                }
            } catch (System.Exception ex) {
                Logger.Warning("Failed to load monitors for settings", ex);
            }
        }

        public ObservableCollection<ScreenViewer> AvailableMonitors { get; }

        public ScreenViewer SelectedDefaultMonitor {
            get => _selectedDefaultMonitor;
            set => SetProperty(ref _selectedDefaultMonitor, value);
        }

        public double DefaultOpacity {
            get => _defaultOpacity;
            set => SetProperty(ref _defaultOpacity, value);
        }

        public double DefaultVolume {
            get => _defaultVolume;
            set => SetProperty(ref _defaultVolume, value);
        }



        public bool LauncherAlwaysOnTop {
            get => _launcherAlwaysOnTop;
            set => SetProperty(ref _launcherAlwaysOnTop, value);
        }

        public bool PanicHotkeyCtrl {
            get => _panicHotkeyCtrl;
            set {
                SetProperty(ref _panicHotkeyCtrl, value);
                OnPropertyChanged(nameof(PanicHotkeyDisplay));
            }
        }

        public bool PanicHotkeyShift {
            get => _panicHotkeyShift;
            set {
                SetProperty(ref _panicHotkeyShift, value);
                OnPropertyChanged(nameof(PanicHotkeyDisplay));
            }
        }

        public bool PanicHotkeyAlt {
            get => _panicHotkeyAlt;
            set {
                SetProperty(ref _panicHotkeyAlt, value);
                OnPropertyChanged(nameof(PanicHotkeyDisplay));
            }
        }

        public string PanicHotkeyKey {
            get => _panicHotkeyKey;
            set {
                SetProperty(ref _panicHotkeyKey, value);
                OnPropertyChanged(nameof(PanicHotkeyDisplay));
            }
        }

        public string PanicHotkeyDisplay {
            get {
                var parts = new System.Collections.Generic.List<string>();
                if (PanicHotkeyCtrl) parts.Add("Ctrl");
                if (PanicHotkeyShift) parts.Add("Shift");
                if (PanicHotkeyAlt) parts.Add("Alt");
                
                parts.Add(PanicHotkeyKey ?? "End");
                return string.Join("+", parts);
            }
        }

        public bool AlwaysOpaque {
            get => _alwaysOpaque;
            set => SetProperty(ref _alwaysOpaque, value);
        }



        public bool RememberLastPlaylist {
            get => _rememberLastPlaylist;
            set => SetProperty(ref _rememberLastPlaylist, value);
        }

        public bool RememberFilePosition {
            get => _rememberFilePosition;
            set => SetProperty(ref _rememberFilePosition, value);
        }

        public bool EnableHotScreenIntegration {
            get => _enableHotScreenIntegration;
            set => SetProperty(ref _enableHotScreenIntegration, value);
        }

        public int HotScreenOffsetX {
            get => _hotScreenOffsetX;
            set => SetProperty(ref _hotScreenOffsetX, value);
        }

        public int HotScreenOffsetY {
            get => _hotScreenOffsetY;
            set => SetProperty(ref _hotScreenOffsetY, value);
        }

        public bool HotScreenUseClientArea {
            get => _hotScreenUseClientArea;
            set => SetProperty(ref _hotScreenUseClientArea, value);
        }

            
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand OpenKoFiCommand { get; }
        public ICommand ResetPositionsCommand { get; }

        public event System.EventHandler RequestClose;

        private void Ok(object obj) {
            // Save settings
            var settings = App.Settings;
            settings.DefaultOpacity = DefaultOpacity;
            settings.DefaultVolume = DefaultVolume;

            settings.LauncherAlwaysOnTop = LauncherAlwaysOnTop;
            
            // Save default monitor
            settings.DefaultMonitorDeviceName = SelectedDefaultMonitor?.DeviceName;
            
            // Save panic hotkey settings
            uint modifiers = 0;
            if (PanicHotkeyCtrl) modifiers |= MOD_CONTROL;
            if (PanicHotkeyShift) modifiers |= MOD_SHIFT;
            if (PanicHotkeyAlt) modifiers |= MOD_ALT;
            settings.PanicHotkeyModifiers = modifiers;
            settings.PanicHotkeyKey = PanicHotkeyKey ?? "End";
            settings.AlwaysOpaque = AlwaysOpaque;

            settings.RememberLastPlaylist = RememberLastPlaylist;
            settings.RememberFilePosition = RememberFilePosition;
            settings.EnableHotScreenIntegration = EnableHotScreenIntegration;
            settings.HotScreenOffsetX = HotScreenOffsetX;
            settings.HotScreenOffsetY = HotScreenOffsetY;
            settings.HotScreenUseClientArea = HotScreenUseClientArea;
            
            // Save currently expanded section
            if (IsGeneralExpanded) settings.LastExpandedSection = nameof(IsGeneralExpanded);
            else if (IsPlaybackExpanded) settings.LastExpandedSection = nameof(IsPlaybackExpanded);
            else if (IsApplicationExpanded) settings.LastExpandedSection = nameof(IsApplicationExpanded);
            else if (IsHotkeysExpanded) settings.LastExpandedSection = nameof(IsHotkeysExpanded);
            else if (IsHistoryExpanded) settings.LastExpandedSection = nameof(IsHistoryExpanded);
            
            settings.Save();

            RequestClose?.Invoke(this, System.EventArgs.Empty);
        }

        private void Cancel(object obj) {
            RequestClose?.Invoke(this, System.EventArgs.Empty);
        }

    }
}
