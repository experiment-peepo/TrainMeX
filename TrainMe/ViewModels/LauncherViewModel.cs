using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TrainMe.Classes;
using Microsoft.Win32;

namespace TrainMe.ViewModels {
    public class LauncherViewModel : ObservableObject {
        public ObservableCollection<VideoItem> AddedFiles { get; } = new ObservableCollection<VideoItem>();
        public ObservableCollection<ScreenViewer> AvailableScreens { get; } = new ObservableCollection<ScreenViewer>();
        
        private Random random = new Random();

        private double _volume;
        public double Volume {
            get => _volume;
            set {
                if (SetProperty(ref _volume, value)) {
                    App.Settings.Volume = value;
                    App.Settings.Save();
                    App.VideoService.SetVolumeAll(value);
                }
            }
        }

        private double _opacity;
        public double Opacity {
            get => _opacity;
            set {
                if (SetProperty(ref _opacity, value)) {
                    App.Settings.Opacity = value;
                    App.Settings.Save();
                    App.VideoService.SetOpacityAll(value);
                }
            }
        }

        private bool _shuffle;
        public bool Shuffle {
            get => _shuffle;
            set => SetProperty(ref _shuffle, value);
        }

        private string _hypnotizeButtonText = "TRAIN ME!";
        public string HypnotizeButtonText {
            get => _hypnotizeButtonText;
            set => SetProperty(ref _hypnotizeButtonText, value);
        }

        private bool _isHypnotizeEnabled;
        public bool IsHypnotizeEnabled {
            get => _isHypnotizeEnabled;
            set => SetProperty(ref _isHypnotizeEnabled, value);
        }

        private bool _isDehypnotizeEnabled;
        public bool IsDehypnotizeEnabled {
            get => _isDehypnotizeEnabled;
            set => SetProperty(ref _isDehypnotizeEnabled, value);
        }

        private bool _isPauseEnabled;
        public bool IsPauseEnabled {
            get => _isPauseEnabled;
            set => SetProperty(ref _isPauseEnabled, value);
        }

        private string _pauseButtonText = "Pause";
        public string PauseButtonText {
            get => _pauseButtonText;
            set => SetProperty(ref _pauseButtonText, value);
        }

        private bool _pauseClicked;

        public ICommand HypnotizeCommand { get; }
        public ICommand DehypnotizeCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand BrowseCommand { get; }
        public ICommand RemoveSelectedCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand KofiCommand { get; }
        public ICommand MinimizeCommand { get; }

        public LauncherViewModel() {
            Volume = App.Settings.Volume;
            Opacity = App.Settings.Opacity;

            RefreshScreens();

            HypnotizeCommand = new RelayCommand(Hypnotize, _ => IsHypnotizeEnabled);
            DehypnotizeCommand = new RelayCommand(Dehypnotize);
            PauseCommand = new RelayCommand(Pause);
            BrowseCommand = new RelayCommand(Browse);
            RemoveSelectedCommand = new RelayCommand(RemoveSelected);
            RemoveItemCommand = new RelayCommand(RemoveItem);
            ClearAllCommand = new RelayCommand(ClearAll);
            SavePlaylistCommand = new RelayCommand(SavePlaylist);
            LoadPlaylistCommand = new RelayCommand(LoadPlaylist);
            ExitCommand = new RelayCommand(Exit);
            KofiCommand = new RelayCommand(Kofi);
            MinimizeCommand = new RelayCommand(Minimize);

            UpdateButtons();
        }

        public ICommand RemoveItemCommand { get; }
        public ICommand SavePlaylistCommand { get; }
        public ICommand LoadPlaylistCommand { get; }

        private void RemoveItem(object parameter) {
            if (parameter is VideoItem item) {
                AddedFiles.Remove(item);
                UpdateButtons();
            }
        }

        private void RefreshScreens() {
            AvailableScreens.Clear();
            foreach (var s in WindowServices.GetAllScreenViewers()) {
                AvailableScreens.Add(s);
            }
        }

        private void UpdateButtons() {
            bool hasFiles = AddedFiles.Count > 0;
            bool allAssigned = AllFilesAssigned();
            IsHypnotizeEnabled = hasFiles && allAssigned;
        }

        private bool AllFilesAssigned() {
            foreach (var f in AddedFiles) {
                if (f.AssignedScreen == null) return false;
            }
            return true;
        }

        private void Hypnotize(object parameter) {
            var selectedItems = parameter as System.Collections.IList;
            var assignments = BuildAssignmentsFromSelection(selectedItems);
            if (assignments == null || assignments.Count == 0) return;
            
            App.VideoService.PlayPerMonitor(assignments, Opacity, Volume);
            IsDehypnotizeEnabled = true;
            IsPauseEnabled = true;
        }

        private Dictionary<ScreenViewer, IEnumerable<string>> BuildAssignmentsFromSelection(System.Collections.IList selectedItems) {
            var selectedFiles = new List<VideoItem>();
            if (selectedItems != null && selectedItems.Count > 0) {
                foreach (VideoItem f in selectedItems) selectedFiles.Add(f);
            } else {
                foreach (var f in AddedFiles) selectedFiles.Add(f);
            }

            if (selectedFiles.Count < 1) return null;

            // Simple validation again just in case
            if (selectedFiles.Any(x => x.AssignedScreen == null)) return null;

            if (Shuffle) selectedFiles = selectedFiles.OrderBy(a => random.Next()).ToList();

            var assignments = new Dictionary<ScreenViewer, IEnumerable<string>>();
            foreach (var f in selectedFiles) {
                var assigned = f.AssignedScreen;
                if (assigned == null) continue;
                if (!assignments.ContainsKey(assigned)) assignments[assigned] = new List<string>();
                ((List<string>)assignments[assigned]).Add(f.FilePath);
            }
            return assignments;
        }

        private void Dehypnotize(object obj) {
            IsDehypnotizeEnabled = false;
            IsPauseEnabled = false;
            App.VideoService.StopAll();
        }

        private void Pause(object obj) {
            if (_pauseClicked) {
                _pauseClicked = false;
                PauseButtonText = "Pause";
                App.VideoService.ContinueAll();
            } else {
                _pauseClicked = true;
                PauseButtonText = "Continue";
                App.VideoService.PauseAll();
            }
        }

        private void Browse(object obj) {
            var dlg = new OpenFileDialog {
                Multiselect = true,
                Filter = "Video Files|*.mp4;*.mkv;*.avi;*.mov;*.wmv|All Files|*.*"
            };
            if (dlg.ShowDialog() == true) {
                // Ensure screens are up to date
                if (AvailableScreens.Count == 0) RefreshScreens();
                var primary = AvailableScreens.FirstOrDefault(v => v.Screen.Primary) ?? AvailableScreens.FirstOrDefault();
                
                foreach (var f in dlg.FileNames) {
                    if (!AddedFiles.Any(x => x.FilePath == f)) {
                        AddedFiles.Add(new VideoItem(f, primary));
                    }
                }
                UpdateButtons();
            }
        }

        private void RemoveSelected(object parameter) {
            var selectedItems = parameter as System.Collections.IList;
            if (selectedItems == null) return;
            
            var toRemove = new List<VideoItem>();
            foreach (VideoItem f in selectedItems) toRemove.Add(f);
            foreach (var f in toRemove) {
                AddedFiles.Remove(f);
            }
            UpdateButtons();
        }

        private void ClearAll(object obj) {
            AddedFiles.Clear();
            UpdateButtons();
        }

        private void Exit(object obj) {
            if (MessageBox.Show("Exit program? All hypnosis will be terminated :(", "Exit program", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        }

        private void Kofi(object obj) {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                FileName = "https://ko-fi.com/damsel",
                UseShellExecute = true
            });
        }

        private void Minimize(object obj) {
            if (obj is Window w) w.WindowState = WindowState.Minimized;
        }
        
        // Method to handle Drag & Drop from View
        public void AddDroppedFiles(string[] files) {
             if (AvailableScreens.Count == 0) RefreshScreens();
             var primary = AvailableScreens.FirstOrDefault(v => v.Screen.Primary) ?? AvailableScreens.FirstOrDefault();

             foreach (var f in files) {
                 var ext = System.IO.Path.GetExtension(f)?.ToLowerInvariant();
                 if (ext == ".mp4" || ext == ".mkv" || ext == ".avi" || ext == ".mov" || ext == ".wmv") {
                     if (!AddedFiles.Any(x => x.FilePath == f)) {
                         AddedFiles.Add(new VideoItem(f, primary));
                     }
                 }
             }
             UpdateButtons();
        }

        public void MoveVideoItem(VideoItem item, int newIndex) {
            if (item == null) return;
            var oldIndex = AddedFiles.IndexOf(item);
            if (oldIndex < 0 || newIndex < 0 || newIndex >= AddedFiles.Count) return;
            
            AddedFiles.Move(oldIndex, newIndex);
        }

        private void SavePlaylist(object obj) {
            var dlg = new SaveFileDialog {
                Filter = "TrainMe Playlist|*.json",
                FileName = "playlist.json"
            };
            if (dlg.ShowDialog() == true) {
                var playlist = new Playlist();
                foreach (var item in AddedFiles) {
                    playlist.Items.Add(new PlaylistItem {
                        FilePath = item.FilePath,
                        ScreenId = item.AssignedScreen?.ID ?? 0
                    });
                }
                
                var json = System.Text.Json.JsonSerializer.Serialize(playlist);
                System.IO.File.WriteAllText(dlg.FileName, json);
            }
        }

        private void LoadPlaylist(object obj) {
            var dlg = new OpenFileDialog {
                Filter = "TrainMe Playlist|*.json"
            };
            if (dlg.ShowDialog() == true) {
                try {
                    var json = System.IO.File.ReadAllText(dlg.FileName);
                    var playlist = System.Text.Json.JsonSerializer.Deserialize<Playlist>(json);
                    
                    if (playlist != null) {
                        AddedFiles.Clear();
                        if (AvailableScreens.Count == 0) RefreshScreens();
                        
                        foreach (var item in playlist.Items) {
                            var screen = AvailableScreens.FirstOrDefault(s => s.ID == item.ScreenId) ?? AvailableScreens.FirstOrDefault();
                            AddedFiles.Add(new VideoItem(item.FilePath, screen));
                        }
                        UpdateButtons();
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Failed to load playlist: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
