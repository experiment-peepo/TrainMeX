## Overview
- Convert the classic .NET Framework 4.7.2 WPF project to SDK-style targeting .NET 8 Windows Desktop.
- Preserve functionality (WPF windows, MediaElement, Win32 interop, multi-monitor via Windows Forms).
- Validate build/run and fix any API or packaging issues.

## Current State
- Classic WPF .csproj targets v4.7.2 with explicit references (TrainMe\TrainMe.csproj:8–17, 40–57, 59–83).
- Uses `System.Windows.Forms.Screen` (WindowsForms) and Win32 `user32.dll` interop (WindowServices.cs:27–43).
- Media playback via WPF `MediaElement` (HypnoWindow.xaml:26–33; HypnoWindow.xaml.cs:58–62).

## Migration Steps
1. Convert `TrainMe.csproj` to SDK-style
- Replace XML with:
```
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
    <OutputType>WinExe</OutputType>
    <RootNamespace>TrainMe</RootNamespace>
    <AssemblyName>TrainMe</AssemblyName>
    <ApplicationIcon>Images\icon.ico</ApplicationIcon>
  </PropertyGroup>
  <ItemGroup>
    <Resource Include="Images\**\*" />
  </ItemGroup>
</Project>
```
- Remove classic `ProjectTypeGuids`, explicit framework references, and `ApplicationDefinition/Page` entries (SDK discovers WPF/XAML with `UseWPF`).
- Keep `Properties/Resources.resx` and `Properties/Settings.settings` items; SDK handles them.

2. Ensure Windows Forms interop works
- With `<UseWindowsForms>true</UseWindowsForms>`, existing `System.Windows.Forms.Screen` usage continues without extra references.

3. Review file paths and content
- External `Videos/` directory stays outside compilation; runtime relative path continues to work.
- Optional hardening later: use absolute path from `AppDomain.CurrentDomain.BaseDirectory` for `MediaElement.Source` to avoid working-directory issues.

4. Remove unused legacy config
- If `App.config` contains no needed settings, it can be removed; binding redirects are not needed on .NET 8.

5. Build & run
- Install .NET 8 SDK + Windows Desktop runtime.
- Build: `dotnet build d:\Projects\TrainMeX\TrainMe\TrainMe.sln -c Debug`
- Run: `d:\Projects\TrainMeX\TrainMe\TrainMe\bin\Debug\net8.0-windows\TrainMe.exe`

6. Validate functionality
- Smoke test video playback (mp4 H.264/AAC) across monitors, controls (pause/resume, volume/opacity), and loop.
- Verify Win32 transparent overlay still works and window placement on all screens.

7. Fixes if needed
- If `MediaElement` has codec issues on .NET 8, provide user feedback and recommend codec packs; consider switching to `MediaPlayer` API if needed.
- If any API warnings occur (e.g., `SetWindowLong` on 64-bit), optionally modernize to `SetWindowLongPtr` signatures.

## Risks & Considerations
- `MediaElement` behavior and codec availability under .NET 8 may differ; stick to H.264/AAC mp4.
- Resource/XAML auto-discovery requires `UseWPF`; ensure all XAML files are under project folder with standard build actions (current structure is compatible).
- Windows Forms interop requires `UseWindowsForms` set.

## Deliverables
- Updated SDK-style `.csproj` targeting `net8.0-windows`.
- Successful build and execution on .NET 8.
- Test report covering playback, controls, multi-monitor, and overlay behavior, with any follow-up fixes applied.