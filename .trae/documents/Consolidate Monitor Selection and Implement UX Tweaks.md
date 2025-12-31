## Goal
Keep only one monitor selection method and apply the remaining UX improvements.

## Keep
- Per‑file monitor dropdown (`AssignCombo`) in `Added Files` as the single source of truth.

## Remove
- Monitors list (`MonitorPlayList`) and related selection logic.
- Bulk assign controls and handlers.

## Wire Up
- Gating: Enable `TRAIN ME!` when at least one file exists and each file has a monitor assignment.
- Assignments builder: Use selected files; if none, use all added files. Respect shuffle.

## Drag‑and‑Drop
- Enable `AllowDrop` on `AddedFilesList`.
- Implement `DragOver` and `Drop` to add supported video files and auto‑assign to primary monitor.

## Playback Polish
- Set media position to `TimeSpan.Zero` on start.
- Include failing file path in playback error message.

## Files to Edit
- `TrainMe/Windows/LauncherWindow.xaml` and `.xaml.cs` — remove monitors list and bulk assign; add drag‑and‑drop; adjust gating and methods.
- `TrainMe/Windows/HypnoWindow.xaml.cs` — position and error message tweaks.

## Validation
- Build and run; verify adding via drag‑and‑drop, per‑file dropdown works, `TRAIN ME!` gating, shuffle, and playback across assigned monitors.