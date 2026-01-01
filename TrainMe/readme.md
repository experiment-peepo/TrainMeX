# TrainMeX

A WPF-based video overlay player application designed for multi-screen fullscreen video playback with independent settings per screen.

## Overview

TrainMeX is a Windows application that enables synchronized video playback across multiple monitors. Each screen can have its own playlist, opacity settings, and volume controls, making it ideal for immersive video experiences.

## Features

- **Multi-Screen Support**: Play videos across multiple monitors simultaneously
- **Independent Screen Settings**: Each screen has separate:
  - Video playlist
  - Opacity control
  - Volume control
- **Playlist Management**:
  - Add multiple video files
  - Drag and drop reordering
  - Shuffle playback
  - Remove individual items
- **Playback Controls**:
  - Play/Pause functionality
  - Global hotkeys for pause/panic
  - Session auto-save/restore
- **Customizable Settings**:
  - Default opacity and volume
  - Auto-load session on startup
  - Prevent overlay minimization
- **Portable**: Self-contained executable, no installation required

## Requirements

- **Operating System**: Windows 10/11 (x64)
- **Runtime**: .NET 8.0 (included in self-contained build)
- **Hardware**: Multiple monitors recommended (single monitor supported)

## Building from Source

### Prerequisites

- .NET 8.0 SDK
- Windows operating system
- Visual Studio 2022 or Visual Studio Code (optional)

### Quick Build

#### Option 1: PowerShell Script (Recommended)
```powershell
cd TrainMeX
.\publish.ps1
```

#### Option 2: Batch Script
```cmd
cd TrainMeX
publish.bat
```

#### Option 3: Manual Build
```bash
dotnet publish TrainMeX\TrainMeX.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish
```

The standalone executable will be located at `publish\TrainMeX.exe`.

### Development Build

```bash
dotnet build TrainMeX\TrainMeX.sln
```

## Usage

1. **Launch the Application**: Run `TrainMeX.exe`
2. **Add Videos**: Click "Browse Videos..." to add video files to your playlist
3. **Assign Screens**: Select a screen from the available monitors list
4. **Configure Settings**: Adjust opacity and volume for each screen
5. **Start Playback**: Click "TRAIN ME!" to begin fullscreen playback
6. **Control Playback**: Use the pause button or global hotkeys to control playback

### Global Hotkeys

- **Pause/Resume**: Toggle playback (default: configured in settings)
- **Panic**: Stop all playback immediately (default: configured in settings)

### Settings

Access settings via the ⚙ button in the launcher window. Configure:
- Default opacity and volume
- Auto-load session on startup
- Prevent overlay minimization
- Global hotkey bindings

## Project Structure

```
TrainMeX/
├── TrainMeX/                    # Main application project
│   ├── Classes/               # Core classes and services
│   │   ├── Behaviors/         # WPF behaviors
│   │   ├── VideoPlayerService.cs
│   │   ├── UserSettings.cs
│   │   ├── Playlist.cs
│   │   └── ...
│   ├── ViewModels/            # MVVM view models
│   ├── Windows/               # WPF windows
│   │   ├── LauncherWindow.xaml
│   │   ├── HypnoWindow.xaml
│   │   └── SettingsWindow.xaml
│   └── Images/                # Application resources
├── TrainMeX.Tests/             # Unit tests
├── publish/                   # Published executable output
└── readme.md                  # This file
```

## Testing

Run the test suite:

```bash
dotnet test TrainMeX\TrainMeX.Tests\TrainMeX.Tests.csproj
```

## Distribution

The published executable (`TrainMeX.exe`) is:
- **Self-contained**: Includes .NET 8.0 runtime
- **Portable**: No installation required
- **Single file**: All dependencies bundled
- **Size**: Approximately 50-100 MB

Settings and session data are stored as JSON files alongside the executable:
- `settings.json` - User preferences
- `session.json` - Session state

## Development Status

⚠️ **Note**: This project is currently not actively maintained. The original developer has shifted focus to a multiplatform solution without WPF dependencies. However, the codebase is open source and available for community contributions.

## Contributing

While the project is not actively maintained, contributions are welcome:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This software is licensed under the **GNU General Public License version 3 (GPLv3)**.

### What GPLv3 Means

The GPLv3 license grants users four essential freedoms:
1. **Freedom to use** the software for any purpose
2. **Freedom to change** the software to suit your needs
3. **Freedom to share** the software with others
4. **Freedom to share changes** you make

This is free software - it remains free software regardless of who modifies or distributes it. The copyleft nature of GPLv3 ensures that all derivative works also remain free and open source.

For more information, see the [GNU GPL v3 License](https://www.gnu.org/licenses/gpl-3.0.html) or the `license.txt` file included in this repository.

## Technical Details

- **Framework**: .NET 8.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Media Player**: WPF MediaElement
- **Settings Storage**: JSON files
- **Logging**: Custom logger with file output

## Known Limitations

- Windows-only (WPF dependency)
- Limited to local video files (no streaming support)
- Media codec support depends on system codecs
- Single user session (no multi-user support)

## Support

As mentioned in the Development Status section, this project is not actively supported. For issues or questions:
- Check existing documentation
- Review the codebase
- Consider contributing fixes or improvements

---

**Copyright (C) 2021 Damsel**

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
