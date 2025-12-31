## Approach
- Create a reusable `VideoPlayerService` that orchestrates `HypnoWindow` instances and exposes a simple API to play local videos from anywhere.
- Register the service globally at app startup so any window/viewmodel can access it.
- Refactor existing playback logic from `LauncherWindow.xaml.cs` into the service while keeping `HypnoWindow` as the player.

## Key Files
- App entry/resources: `TrainMe\App.xaml`, `TrainMe\App.xaml.cs`
- Player window: `TrainMe\Windows\HypnoWindow.xaml` and `HypnoWindow.xaml.cs`
- Orchestration to extract: `TrainMe\Windows\LauncherWindow.xaml.cs`
- Utilities: `TrainMe\Classes\WindowServices.cs`, `ScreenViewer.cs`

## Service API
- `Play(files: IEnumerable<string>, screens?: IEnumerable<ScreenViewer>)`
- `PlayOnAllScreens(files: IEnumerable<string>)`
- `PauseAll()` / `ContinueAll()` / `StopAll()`
- `SetVolumeAll(double volume)` / `SetOpacityAll(double opacity)`
- `IsPlaying { get; }` and events for status updates (optional)

## Implementation Steps
1. Add `VideoPlayerService` under `TrainMe\Classes` that:
   - Manages a map of `ScreenViewer` → `HypnoWindow`
   - Uses existing `HypnoWindow` APIs: `SetQueue`, `SetVolume`, `SetOpacity`, `PauseVideo`, `ContinueVideo`, `ChangeVideo`
   - Places windows via `WindowServices.MoveWindowToScreen`
2. Initialize and register the service in `App.xaml.cs` (e.g., `Application.Current.Resources["VideoService"] = new VideoPlayerService()`), or make it a singleton.
3. Refactor `LauncherWindow.xaml.cs` to call the service instead of directly creating/controlling `HypnoWindow` instances.
4. Add safe file discovery helper that returns video files from `Videos/` and validates existence/format.
5. Optional: Add a file picker (`OpenFileDialog`) for ad-hoc selection from any window.
6. Add basic error handling (missing file, unsupported format) with user notifications.
7. Add minimal tests or a manual validation flow to verify multi-monitor playback, pause/continue, volume/opacity.

## Integration Points
- Extract: window creation and queue setup from `LauncherWindow.xaml.cs` and move to the service.
- Keep: `HypnoWindow` unchanged as the playback host; expose needed methods through the service.

## Validation
- Manual test: play across all screens using sample `mp4`; toggle pause/resume; adjust volume/opacity; verify loop.
- Log or message failures (bad path/codec). Consider a codec note in UI if playback fails.

## Risks & Mitigations
- Codec support: prefer H.264/AAC `mp4`; surface errors when `MediaElement` fails.
- Multiple topmost transparent windows: measure performance; allow stopping quickly.
- Path traversal: constrain to `Videos/` unless explicitly picked via dialog.