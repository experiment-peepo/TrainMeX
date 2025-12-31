## Approach
- Keep the current project structure and theme completely intact.
- Edit existing files only: `TrainMe/Windows/LauncherWindow.xaml`, `LauncherWindow.xaml.cs`. No new folders, view models, or dependencies.
- Maintain `VideoPlayerService.cs` unchanged; reuse `PlayPerMonitor(assignments, ...)`.

## UI Changes (Minimal)
- Remove the visible Play Per‑Monitor button; keep only `TRAIN ME!` as the start action.
- Add compact monitor chips inside the Added Files list items (inline on the right). Clicking cycles monitors (M0 → M1 → M2 → …). No labels added.
- `TRAIN ME!` enabled only when:
  - At least one monitor is selected,
  - At least one file is added, and
  - Every added file has an assignment chip.
- Disabled state: reduced opacity + small lock icon; button text remains `TRAIN ME!`.

## Logic Wiring (Code‑behind)
- Maintain an in‑memory `Dictionary<string, ScreenViewer>` mapping `filePath → assignedMonitor`.
- Update mapping when the chip is clicked; cycle through `MonitorPlayList` selected monitors.
- On `TRAIN ME!` click, build `IDictionary<ScreenViewer, IEnumerable<string>>` from the mapping and call `App.VideoService.PlayPerMonitor(assignments, OpacitySlider.Value, VolumeSlider.Value)`.
- Update existing `DisableButtonIfNoSelection()` to also check that all files are assigned.

## Files to Edit
- `TrainMe/Windows/LauncherWindow.xaml`: add an `ItemTemplate` to `AddedFilesList` with an inline chip; remove/hide Play Per‑Monitor button; keep styles.
- `TrainMe/Windows/LauncherWindow.xaml.cs`: add mapping + chip click handler; update gating logic; `TRAIN ME!` click builds assignments.

## Validation
- 0 monitors/files or any unassigned file → `TRAIN ME!` disabled.
- All files assigned and monitors selected → `TRAIN ME!` enabled and starts playback per assignments.
- Build and run to verify no project structure changes.

## Notes
- No new files beyond the already‑created mockups in the project root.
- No refactors to MVVM; strictly code‑behind in current files.
