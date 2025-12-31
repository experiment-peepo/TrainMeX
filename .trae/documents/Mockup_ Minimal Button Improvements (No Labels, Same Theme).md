## Goal

* Keep the existing UI layout and color scheme.

* Improve only the two buttons: TRAIN ME! and Play Per‑Monitor.

* No new labels or extra text blocks.

## Mockup (to add)

* Create `ui-mockup-min-buttons.svg` showing:

  * TRAIN ME! states: disabled (greyed, lock icon) vs enabled (DeepPink), same text.

  * Play Per‑Monitor with subtle monitor/film icons on the button, same text.

  * All other UI remains unchanged.

## Behavior Tweaks (no labels)

* TRAIN ME! gating:

  * Keep button text as "TRAIN ME!" at all times.

  * Disabled style only: reduced opacity + a small lock icon inside the button.

  * Enabled style: normal DeepPink.

* Play Per‑Monitor clarity:

  * Add tiny monitor + film icons inside the button (left-aligned), no extra helper text.

  * Button enabled only when both files and monitors are selected (same gating as TRAIN ME!).

## Implementation Steps (after mockup approval)

1. Add the SVG mockup file to project root.
2. Update XAML for the two buttons:

   * Inject icons into button content (stacked horizontally) using existing styles.

   * Bind `IsEnabled` as today; change only visual states (opacity/icon visibility) when disabled.

   * Add concise tooltips (optional, not visible in layout; keeps labels off the canvas).
3. Verify build and run; check button states with 0/1+ selections.

## No New Tools

* Uses existing WPF and styles; no libraries added.

Approve and I will add the mockup file first, then implement these minimal XAML tweaks keeping the current design.
