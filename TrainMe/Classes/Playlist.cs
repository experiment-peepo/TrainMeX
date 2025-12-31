using System.Collections.Generic;

namespace TrainMe.Classes {
    public class Playlist {
        public List<PlaylistItem> Items { get; set; } = new List<PlaylistItem>();
    }

    public class PlaylistItem {
        public string FilePath { get; set; }
        public int ScreenId { get; set; }
    }
}
