## Overview
- Add the ability to select and play local video files from any folder, not just `Videos/`.
- Support absolute file paths throughout playback, while keeping existing `Videos/` list working.
- Provide a simple UI action to browse and play files immediately, and update the player to accept absolute paths.

## Key Files
- Player: `TrainMe/Windows/HypnoWindow.xaml.cs:40–62` (queue and `ChangeVideo`)
- Launcher: `TrainMe/Windows/LauncherWindow.xaml` and `.xaml.cs:52–68, 98–118, 153–165`
- Service: `TrainMe/Classes/VideoPlayerService.cs` (normalization currently forces `Videos/`)

## Implementation
1. Accept absolute paths in the player
- Update `HypnoWindow.ChangeVideo(int)` to:
  - If `Path.IsPathRooted(files[filePos])`, set `FirstVideo.Source = new Uri(files[filePos], UriKind.Absolute)`
  - Else, keep relative behavior to `Videos/<name>`
- Keep queue semantics unchanged; files array may contain mixed absolute and names.
- Add `MediaFailed` handler in `HypnoWindow.xaml` to surface unsupported formats or missing files.

2. Stop forcing `Videos/` in the service
- Update `VideoPlayerService.NormalizeFiles(...)` to:
  - Return absolute paths as-is (if rooted and `File.Exists`) and pass through relative names unchanged
  - Remove `Path.Combine("Videos", name)` logic
- Continue existence checks for absolute paths; skip missing entries.

3. Add a UI action to browse files anywhere
- In `LauncherWindow.xaml`: add a `Browse...` button near the video controls.
- In `LauncherWindow.xaml.cs`:
  - Use `Microsoft.Win32.OpenFileDialog` with `Multiselect=true` and filter for common video types
  - On selection, immediately call `App.VideoService.PlayOnScreens(selectedAbsolutePaths, selectedMonitors, OpacitySlider.Value, VolumeSlider.Value)`
  - Keep existing `Videos/` list and selection logic intact (users can still choose from the app folder).

4. Optional enhancements
- Show chosen external files in a small read-only list for user feedback.
- Allow drag & drop of files onto the launcher to start playback.
- Persist last-used browse folder in preferences for convenience.

## Validation
- Place sample mp4s in various folders (e.g., `Downloads`, external drive) and browse to them
- Start playback across selected monitors; verify pause/resume, volume/opacity control, and looping still work
- Try a missing file and unsupported format to confirm `MediaFailed` surfaces a clear message without app crash

## Risks & Notes
- Codec dependence remains (Windows Media Foundation); recommend H.264/AAC mp4
- Absolute paths must be local; skip URIs that are not file paths
- Ensure file dialog runs with sufficient permissions; large files may take time to buffer

## Deliverables
- Updated player and service to accept absolute paths
- New `Browse...` action in the launcher enabling play-from-anywhere
- Manual test report confirming functionality and error handling