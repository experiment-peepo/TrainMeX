## Scope

* Keep current TrainMe WPF UI and color scheme (DeepPink/HotPink styles).

* Improve clarity and feedback for:

  * Play Per‑Monitor button

  * TRAIN ME! (select files and monitors first) flow

## UI Tweaks (XAML)

* Play Per‑Monitor button:

  * Keep text but add a concise tooltip: “Assign one file per selected monitor; requires files + monitors”.

  * Place a small `Label` (LowerTitle) directly beneath with dynamic helper text: “Selected: <files> file(s), <monitors> monitor(s)”.

* TRAIN ME! button:

  * Keep content as “TRAIN ME!” consistently.

  * Add a `Label` (LowerTitle) under the button showing gating hint when disabled: “Select at least one file and one monitor”. Hidden when enabled.

* Shuffle helper:

  * Add a short helper `Label` next to the checkbox: “Shuffle order of selected files”.

* Counts near lists:

  * Add a `Label` (LowerTitle) above Added Files and above Monitors: “Files selected: N”, “Monitors selected: M”.

## Logic Updates (code‑behind)

* Centralize selection counts: compute `selectedFilesCount` and `selectedMonitorsCount` in `DisableButtonIfNoSelection()` (LauncherWindow\.xaml.cs:135).

* Button enablement:

  * `TRAIN ME!` enabled when `selectedFilesCount > 0 && selectedMonitorsCount > 0`.

  * `Play Per‑Monitor` enabled under same condition.

* Helper labels:

  * Update text/visibility of helper labels each time selection changes (`AddedFilesList.SelectionChanged`, `MonitorPlayList.SelectionChanged`).

* Play Per‑Monitor validation (LauncherWindow\.xaml.cs:191–214):

  * Replace MessageBox for missing selections with setting status helper label text (non‑blocking).

  * If files < monitors, clarify round‑robin mapping in tooltip/helper label: “Round‑robin assignment will be used”.

## Visual Consistency

* Use existing styles: `MainButton` and `LowerTitle` only; no new theme colors.

* Maintain spacing using current StackPanel margins.

## Accessibility

* Tooltips provide extra guidance without changing button text.

* Keyboard: Enter triggers TRAIN ME! if enabled (keep current behavior).

## Verification

* Select 0 files / 0 monitors → both buttons disabled; hint labels show guidance.

* Select files and monitors → buttons enabled; helper labels show counts.

* Play Per‑Monitor with fewer files than monitors → works with round‑robin; helper text indicates behavior.

## Files to Update

* `TrainMe/Windows/LauncherWindow.xaml`: add helper labels and tooltips; minor layout.

* `TrainMe/Windows/LauncherWindow.xaml.cs`: update `DisableButtonIfNoSelection`, selection change handlers, and Play Per‑Monitor validation.

After approval, I will implement these minimal tweaks directly, keeping the existing design and colors intact.
