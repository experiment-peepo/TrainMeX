## Goals

1. Make monitor selection effortless and explicit.
2. Reduce clicks and hidden states in the add→assign→play pipeline.
3. Keep the current UI structure and styles; limit changes to existing files.

## Monitors UX

- Show friendly monitor info in dropdowns: device name, resolution, primary tag.
  - Update `ScreenViewer.ToString()` to include `Screen.DeviceName`, `Bounds`, `Primary` (TrainMe/Classes/ScreenViewer.cs:29).
  - Keep IDs for internal mapping but present readable text to users.
- Default monitor assignment for new files to primary monitor.
  - When adding files (TrainMe/Windows/LauncherWindow.xaml.cs:178), assign `fileAssignments[f] = WindowServices.GetAllScreenViewers().First(x => x.Screen.Primary)`.
- Add bulk assignment quick action.
  - UI: a small `ComboBox` + `Assign All` button next to `Added Files` (TrainMe/Windows/LauncherWindow.xaml:62). Assigns all selected (or all if none selected) to chosen monitor.

## Files Selection Simplification

- Play all added files by default; selection becomes optional filter.
  - Adjust gating to enable `TRAIN ME!` when at least one file exists and each file has a monitor (TrainMe/Windows/LauncherWindow.xaml.cs:136).
  - If selection is empty, treat “selected files” as all added when building assignments (TrainMe/Windows/LauncherWindow.xaml.cs:228).
- Enable drag-and-drop file adding.
  - Add `AllowDrop="True"` and handlers on `AddedFilesList` to accept video files and add them (TrainMe/Windows/LauncherWindow.xaml & .xaml.cs around 48 and 178).

## Pipeline Logic

- Use the per-file monitor `ComboBox` consistently; remove dependence on the monitors list.
  - Keep `AssignCombo_Loaded` and `AssignCombo_SelectionChanged` (TrainMe/Windows/LauncherWindow.xaml.cs:256, 266).
- Implement shuffle option.
  - When `ShuffleCheckBox` is checked (TrainMe/Windows/LauncherWindow.xaml:75), call `GetSelectedAbsoluteRandomized()` (TrainMe/Windows/LauncherWindow.xaml.cs:121) in `BuildAssignmentsFromSelection` to randomize per-monitor queues.

## Persistence

- Restore slider initialization from preferences.
  - Un-comment and use `UserPreferences` in constructor to set `OpacitySlider` and `VolumeSlider` (TrainMe/Windows/LauncherWindow.xaml.cs:44).
- Persist file→monitor assignments.
  - Save simple `path;monitorId` lines to `preferences.ini` alongside volume/opacity.
  - Load on startup; if a monitor ID no longer exists, fall back to primary.

## Reliability and Polishing

- Guard Windows-specific APIs to quiet CA1416 warnings.
  - Add `SupportedOSPlatform("windows")` attribute or `OperatingSystem.IsWindows()` guards in `WindowServices.GetAllScreens` and `MoveWindowToScreen` (TrainMe/Classes/WindowServices.cs:47, 72).
- Improve error messages when a file fails to play.
  - Include the file path and suggest re-adding (TrainMe/Windows/HypnoWindow.xaml.cs:80).
- Minor media tweaks.
  - Set `FirstVideo.Position = TimeSpan.Zero` unless there is a specific reason to skip 1ms (TrainMe/Windows/HypnoWindow.xaml.cs:66).

## Optional MVVM Alignment (Minimal)

- Populate `Classes/ViewModels/LauncherViewModel.cs` with properties for `AddedFiles`, `Assignments`, `Opacity`, `Volume`, and commands for `Browse`, `AssignAll`, `Train`, `Pause`, `Stop`.
- Bind existing controls to VM while keeping current window and styles intact; reduce code-behind in `LauncherWindow.xaml.cs` gradually.

## Files to Edit

- `TrainMe/Windows/LauncherWindow.xaml` and `.xaml.cs` — UI tweaks, drag-drop, bulk assign, shuffle wiring.
- `TrainMe/Classes/ScreenViewer.cs` — More informative `ToString()`.
- `TrainMe/Classes/WindowServices.cs` — OS guards.
- `TrainMe/Windows/HypnoWindow.xaml.cs` — minor playback polish.
- `TrainMe/Classes/UserPreferences.cs` — extend to store assignments (optional).
- `TrainMe/Classes/ViewModels/LauncherViewModel.cs` — implement minimal VM (optional).

## Validation

- Build and run; verify:
  - New files auto-assigned to primary monitor.
  - Changing monitor via dropdown updates playback target.
  - Bulk assign works for selected/all files.
  - `TRAIN ME!` enables with at least one file and assignments present.
  - Shuffle randomizes per-monitor queues when checked.
  - Preferences load and save for sliders (and assignments if enabled).
