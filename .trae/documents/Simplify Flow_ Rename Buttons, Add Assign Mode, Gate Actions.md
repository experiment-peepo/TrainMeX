## Goals
- Keep the existing color scheme and layout.
- Make the flow obvious with minimal UI changes and clearer button names.
- Enable per‑video monitor assignment without extra labels.

## Button Renames
- TRAIN ME! → Play On Selected Monitors
- Play Per‑Monitor → Play Assigned
- Dehypnotize → Stop All
- Pause → Pause / Resume (toggle text as today)

## Easier Flow
- Step order (implicit, no labels):
  1) Select monitors
  2) Add videos
  3) Assign (optional)
  4) Play
- Gate actions with disabled states:
  - When files or monitors are missing, buttons remain disabled with reduced opacity and a lock icon (content stays the same; no extra labels).
  - Tooltips provide short guidance (not visible in layout).

## Assign Mode (Minimal UI)
- Add an Assign Mode toggle button:
  - Assign Mode (Off/On). When On, each item in Added Files shows a small monitor selector chip (M0/M1/M2…) inline on the right.
  - Clicking the chip cycles through monitors.
  - Exit Assign Mode to return to the normal list.
- Play Assigned uses these selections to build the mapping.

## Implementation Steps
1) XAML updates (LauncherWindow.xaml):
- Rename button contents per above.
- Add Assign Mode toggle and inline chips inside Added Files items when Assign Mode is On.
- Keep styles (`MainButton`, `LowerTitle`) and spacing intact; no new label blocks.
- Add minimal icons inside buttons (lock for disabled; monitor/film icons for Play Assigned).

2) Code‑behind (LauncherWindow.xaml.cs):
- Maintain `Dictionary<string, ScreenViewer>` mapping file → assigned monitor (updated by chip clicks).
- Gate enabling of Play buttons based on `AddedFilesList.SelectedItems.Count` and `MonitorPlayList.SelectedItems.Count`.
- Build assignments from mapping for Play Assigned; fall back to round‑robin only if a file has no assignment.

3) Service (VideoPlayerService.cs):
- Reuse existing `PlayPerMonitor(assignments, ...)` for Play Assigned.
- No behavior changes needed.

## Verification
- 0 selections → both Play buttons disabled (lock icon).
- Monitors + files selected → Play On Selected Monitors enabled.
- Assign Mode On → chips visible; set assignments and confirm Play Assigned uses them.
- Pause/Resume and Stop All work as before.

## Files to Update
- `TrainMe/Windows/LauncherWindow.xaml`
- `TrainMe/Windows/LauncherWindow.xaml.cs`
- (Reuse) `TrainMe/Classes/VideoPlayerService.cs`

After approval, I will implement the button renames, gating, Assign Mode chips, and the Play Assigned logic, keeping the theme identical and the UI uncluttered.