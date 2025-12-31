## Deliverables
- New mockup (v3) showing step‑gated flow, monitor mapping, improved list, status bar
- Updated launcher UI (XAML) implementing step‑gating, assignments, clearer actions
- Non‑blocking status messaging and basic accessibility improvements

## Phase 1: Mockup v3
- Create `ui-mockup-v3.svg` depicting:
  - Step 1: Select Monitors (tiles with resolution)
  - Step 2: Add Videos (absolute paths only), search box
  - Step 3: Assign per monitor (Assign dropdown on tiles; optional drag‑and‑drop)
  - Step 4: Play (Play Same / Play Per‑Monitor) with helper text
  - Status bar showing monitors, queue, and last message

## Phase 2: UI Skeleton (XAML)
- Add step headers and section containers
- Gate sections and actions via `IsEnabled` bindings:
  - Monitors selected > 0 enables Step 2
  - Files selected > 0 enables Step 3/4
- Add a status bar (Label/TextBlock) bound to a simple status property in code‑behind
- Update `AddedFilesList` with an `ItemTemplate` showing name and placeholders for duration/resolution (thumbnail optional later)
- Add a Monitor Map panel (`ItemsControl` bound to `WindowServices.GetAllScreenViewers()`), each tile shows monitor ID/resolution and a `ComboBox` to select an assigned file

## Phase 3: Wiring Logic (Code‑behind)
- Maintain an in‑memory assignments map: `Dictionary<ScreenViewer, string>` updated by each monitor tile’s dropdown
- “Play Same Video on Monitors”: use the first selected file across all selected monitors
- “Play Per‑Monitor”: build assignments from the map; inline warn (status bar) for monitors without an assignment
- Update `DisableButtonIfNoSelection` to reflect absolute‑paths only (files from Added Files)

## Phase 4: Usability & Accessibility
- Drag‑and‑drop to Added Files (AllowDrop + Drop handler for file paths)
- Keyboard navigation (tab order), visible focus indicators, `AutomationProperties.Name` on interactive elements
- Spacing/typography polish for clarity and contrast

## Optional Later: FFmpeg Thumbnails/Metadata
- If desired, add `ffmpeg/ffprobe` to generate thumbnails and read duration/resolution
- Minimal service wrapper to run commands and parse JSON; cache thumbnails in `Thumbnails/`

## Testing
- Single/Multiple monitor selection, file selection, assignments, Play Same vs Play Per‑Monitor
- Empty states and gating validation; status messages appear instead of modal dialogs
- Drag‑and‑drop adds files; accessibility checks (keyboard focus and labels)