## Goals
- Verify the app builds and runs.
- Validate that local videos can be played from anywhere via the centralized playback path (service or existing orchestration).
- Confirm multi-monitor playback, controls (pause/resume, volume, opacity), queue/loop behavior, and error handling.

## Build & Launch
- Build Debug: `msbuild d:\Projects\TrainMeX\TrainMe\TrainMe.sln /t:Build /p:Configuration=Debug`
- Launch: `d:\Projects\TrainMeX\TrainMe\TrainMe\bin\Debug\TrainMe.exe`
- Alternative Release build if preferred.

## Smoke Tests
- Place sample `mp4` (H.264/AAC) files under `TrainMe\Videos\`.
- Start app and confirm `LauncherWindow` opens (App.xaml:16).
- Use UI to select videos and start playback; ensure `HypnoWindow` appears on selected screens.

## Playback Controls
- Pause/Resume: verify `PauseVideo`/`ContinueVideo` trigger correctly from the launcher UI (LauncherWindow.xaml.cs:153–164).
- Volume/Opacity: adjust and confirm applied across players (HypnoWindow.xaml.cs:44–51).
- Queue/Loop: confirm `MediaEnded` loops to next item (HypnoWindow.xaml.cs:64–71).

## Multi-Monitor
- Confirm one `HypnoWindow` per monitor and correct placement (WindowServices.MoveWindowToScreen, WindowServices.cs:47–71).
- Test with 1+ external displays, verify topmost transparent overlay behavior.

## Error Handling
- Remove/rename a file in the queue and attempt playback; confirm graceful handling (no crash, visible message/log).
- Try unsupported format (e.g., mkv) to observe `MediaElement` failure; ensure error surfaced without crash.

## Preferences
- Validate `preferences.ini` load (UserPreferences.cs:31–50), adjust opacity/volume, restart app, confirm persistence.

## Performance
- Monitor CPU/GPU during playback of multiple windows; verify responsiveness and quick stop.

## Optional Automated Checks
- Add a small UI smoke test using `UIAutomation` to detect window creation and title presence.
- Add a non-interactive integration test harness launching `HypnoWindow` with a short mp4 and asserting `MediaElement` state transitions (if feasible).

## Deliverables
- Run the above checks, capture findings, and fix issues (codec errors, missing file handling, null queues) if found.
- Provide a short test report with any recommendations (e.g., codec guidance, error messages, picker UX).

## Next Step
- With approval, I will build and run the app, execute these tests, and share the results. If issues are discovered, I will propose targeted fixes and verify them.