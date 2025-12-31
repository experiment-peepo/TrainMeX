## Overview
- Rename the menu button to "Browse".
- Add an "Added Files" panel to show and manage files chosen from anywhere (add/remove/clear).
- Enable playing different videos on different monitors via a per‑monitor assignment and a new service method.

## Key Files
- UI: `TrainMe/Windows/LauncherWindow.xaml`, `LauncherWindow.xaml.cs`
- Player: `TrainMe/Windows/HypnoWindow.xaml.cs` (already supports absolute paths)
- Service: `TrainMe/Classes/VideoPlayerService.cs`

## UI Changes
1. Rename button
- Change `Browse and Play...` to `Browse` in `LauncherWindow.xaml`.

2. Added Files list
- Add `ListView x:Name="AddedFilesList"` to display absolute file paths.
- Add two buttons: `Remove Selected` and `Clear All` to manage the list.
- Persist list only in session (no disk writes) for simplicity.

3. Per‑monitor playback controls
- Add a new button `Play Per‑Monitor` that uses selected monitors and selected files to start distinct queues per monitor.
- Selection sources:
  - Selected items from `VideoList` (relative names in `Videos/`)
  - Selected items from `AddedFilesList` (absolute paths)
- Mapping mode: order‑based one‑to‑one; if counts differ, round‑robin assignment.

## Launcher Logic
- Maintain `ObservableCollection<string> AddedFiles` bound to `AddedFilesList`.
- `Browse_Click`: open `OpenFileDialog` (`Multiselect=true`), add chosen absolute paths to `AddedFiles`.
- `Remove Selected`/`Clear All`: mutate `AddedFiles`.
- `Play Per‑Monitor`:
  - Build `files = SelectedFrom(VideoList) ∪ SelectedFrom(AddedFilesList)`
  - Build `monitors = SelectedFrom(MonitorPlayList)`
  - Create assignments by index (round‑robin if needed), then call service.

## Service Changes
- Add `PlayPerMonitor(IDictionary<ScreenViewer, IEnumerable<string>> assignments, double opacity, double volume)`
  - For each monitor, create a `HypnoWindow`, place on screen, set opacity/volume, set queue.
  - Track windows and expose global controls (`PauseAll`, `ContinueAll`, `StopAll`, etc.) as before.
- Keep `NormalizeFiles` behavior (rooted paths pass through; relative names kept for `Videos/`).

## Validation
- Browse files from different folders and confirm they appear in Added Files; remove some and clear list.
- Select two monitors and two files; run `Play Per‑Monitor` and verify distinct videos per monitor.
- Mismatch counts (e.g., 3 monitors, 1 file) should round‑robin without errors.
- Pause/Resume/Volume/Opacity continue to apply globally; dehypnotize stops all windows.

## Notes
- Codec support: recommend H.264/AAC `mp4`.
- Keep added files session‑only initially; can persist last used folder later if desired.

## Deliverables
- Updated launcher UI and logic.
- New service method for per‑monitor playback.
- Manual test pass confirming browsing, management, and distinct per‑monitor playback.