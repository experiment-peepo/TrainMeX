using System.IO;
using TrainMe.Classes;

namespace TrainMe.ViewModels {
    public class VideoItem : ObservableObject {
        public string FilePath { get; }
        public string FileName => Path.GetFileName(FilePath);

        private ScreenViewer _assignedScreen;
        public ScreenViewer AssignedScreen {
            get => _assignedScreen;
            set => SetProperty(ref _assignedScreen, value);
        }

        public VideoItem(string filePath, ScreenViewer defaultScreen = null) {
            FilePath = filePath;
            AssignedScreen = defaultScreen;
        }

        public override string ToString() {
            return FileName;
        }
    }
}
