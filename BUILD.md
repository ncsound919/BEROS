# BEROS Game - Build Documentation

Complete guide for building BEROS (Gummy Bear Farming & Racing) game across all platforms.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start](#quick-start)
3. [Platform-Specific Builds](#platform-specific-builds)
4. [Build Automation](#build-automation)
5. [CI/CD Pipeline](#cicd-pipeline)
6. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Core Requirements

- **Git** - Version control
- **.NET 8 SDK** - For console and MAUI builds
  - Download: https://dotnet.microsoft.com/download
  - Verify: `dotnet --version`

### Platform-Specific Requirements

#### .NET Console (All Platforms)
- .NET 8 SDK (required)
- No additional dependencies

#### MAUI Mobile (Android/iOS)
- .NET 8 SDK (required)
- MAUI Workload
  ```bash
  dotnet workload install maui
  ```
- **Android:**
  - Android SDK (API 21+)
  - Java Development Kit (JDK 11+)
  
- **iOS:**
  - macOS with Xcode 14+
  - Apple Developer Account (for device deployment)

#### Unity (Optional)
- Unity 2021.3 LTS or newer
- Unity Hub recommended
- Android/iOS build support modules

#### Python Game Agent
- Python 3.8+
- Pip package manager

---

## Quick Start

### Automated Build (Recommended)

Run the automated build script:

```bash
./build.sh
```

This will:
1. Build .NET Console version
2. Check MAUI prerequisites
3. Run Python game agent audit
4. Generate build report

### Manual Build - .NET Console

```bash
# Create project
dotnet new console -n BEROS.Console

# Copy game code
cp Mainfile BEROS.Console/Program.cs

# Build
cd BEROS.Console
dotnet build -c Release

# Run
dotnet run -c Release
```

### Python Game Agent

```bash
# Run audit
python3 beros_game.py

# The agent will:
# - Audit project structure
# - Check build prerequisites
# - Generate recommendations
# - Create build-agent-report.txt
```

---

## Platform-Specific Builds

### 1. .NET 8 Console Game

**Target Platforms:** Windows, macOS, Linux

**File:** `Mainfile` (contains complete game code)

#### Build Steps:

```bash
# Create console project
mkdir -p build/console
cd build/console
dotnet new console -n BEROS.Console

# Copy game code
cp ../../Mainfile Program.cs

# Restore dependencies
dotnet restore

# Build Release
dotnet build -c Release

# Run the game
dotnet run -c Release
```

#### Game Controls:
- `W/A/S/D` - Movement
- `E` - Interact (water crops, harvest)
- `F` - Special ability
- `R` - Enter race
- `Q` - Quit

#### Build Output:
- `bin/Release/net8.0/BEROS.Console.dll`
- `bin/Release/net8.0/BEROS.Console.exe` (Windows)

---

### 2. MAUI Mobile (Android/iOS)

**Target Platforms:** Android, iOS

**File:** `dotnet BEROS` (contains MAUI + SkiaSharp implementation)

#### Setup Steps:

```bash
# Install MAUI workload
dotnet workload install maui

# Create MAUI project
dotnet new maui -n BEROS.Mobile
cd BEROS.Mobile

# Add required packages
dotnet add package SkiaSharp.Views.Maui.Controls
dotnet add package SkiaSharp.Views.Maui
dotnet add package Microsoft.AspNetCore.SignalR.Client
dotnet add package CommunityToolkit.Mvvm
dotnet add package Plugin.Maui.Audio
```

#### Integrate Game Code:

1. Replace `MainPage.xaml.cs` content with code from `dotnet BEROS` file
2. Add game assets to `Resources/Raw/assets/` folder
3. Configure XAML UI from provided template

#### Build Android:

```bash
# Debug build
dotnet build -f net8.0-android

# Release build (requires signing)
dotnet publish -f net8.0-android -c Release

# Install to connected device
adb install bin/Release/net8.0-android/publish/com.beros.mobile-Signed.apk
```

#### Build iOS:

```bash
# Simulator
dotnet build -f net8.0-ios -c Release /p:Platform=iPhoneSimulator

# Device (requires provisioning profile)
dotnet build -f net8.0-ios -c Release /p:Platform=iPhone

# Archive for App Store
# Recommended: Use Fastlane for automated signing and upload
```

---

### 3. Unity Mobile (Alternative)

**Target Platforms:** Android, iOS, Windows, macOS

**File:** `Mobile unity  setup` (contains Unity-specific code)

#### Setup Steps:

1. **Create Unity Project:**
   - Open Unity Hub
   - Create new 3D project
   - Name: BEROS
   - Template: 3D Core

2. **Install Packages:**
   - Photon Fusion (Networking)
   - New Input System
   - Easy Touch Controls (Asset Store)

3. **Import Code:**
   - Create `Scripts/` folder
   - Add `PlayerController.cs` from `Mobile unity  setup`
   - Add `RaceManager.cs` from `racemanager`

4. **Build Settings:**
   - **Android:**
     - File → Build Settings → Android
     - Set minimum API level: 21+
     - Configure keystore for release
   
   - **iOS:**
     - File → Build Settings → iOS
     - Set target SDK
     - Configure signing in Xcode

#### Unity Build Commands (CLI):

```bash
# Android
Unity -quit -batchmode -projectPath . -buildTarget Android -executeMethod BuildScript.BuildAndroid

# iOS
Unity -quit -batchmode -projectPath . -buildTarget iOS -executeMethod BuildScript.BuildIOS
```

---

### 4. Multiplayer Server

**File:** `server` (contains SignalR server code)

#### Setup:

```bash
# Create server project
dotnet new web -n BEROS.Server
cd BEROS.Server

# Add SignalR
dotnet add package Microsoft.AspNetCore.SignalR

# Copy server code from 'server' file
# Replace Program.cs content
```

#### Run Server:

```bash
# Development
dotnet run --urls=http://0.0.0.0:5000

# Production
dotnet publish -c Release
dotnet BEROS.Server.dll --urls=http://0.0.0.0:5000
```

#### Expose to LAN (for mobile testing):

```bash
# Option 1: Direct IP (same network)
# Find your local IP: ifconfig (macOS/Linux) or ipconfig (Windows)
# Update mobile app to connect to: http://YOUR_IP:5000/beros

# Option 2: ngrok (public tunnel)
ngrok http 5000
# Use the provided ngrok URL in mobile app
```

---

## Build Automation

### Using build.sh Script

The `build.sh` script automates the build process:

```bash
# Make executable (first time)
chmod +x build.sh

# Run full build
./build.sh

# Output will be in:
# - build/      (temporary build files)
# - artifacts/  (final build outputs)
```

### Using Python Game Agent

The Python agent provides automated auditing:

```bash
# Run audit
python3 beros_game.py

# Check report
cat build-agent-report.txt
```

**Agent Features:**
- Project structure validation
- Build prerequisite checking
- Platform detection
- Automated .NET console builds
- Comprehensive reporting

---

## CI/CD Pipeline

### GitHub Actions Workflow

Location: `.github/workflows/build.yml`

**Triggered by:**
- Push to `main` or `copilot/**` branches
- Pull requests to `main`
- Manual workflow dispatch

**Jobs:**

1. **build-dotnet-console**
   - Builds .NET 8 console version
   - Runs on Ubuntu
   - Uploads artifacts

2. **build-maui-android**
   - Checks MAUI prerequisites
   - Generates setup documentation
   - Runs on Ubuntu

3. **build-python-agent**
   - Tests Python game agent
   - Runs linting
   - Packages Python code

4. **audit-and-document**
   - Generates build audit report
   - Lists all game components
   - Documents build status

**View Build Status:**
```bash
# From repository
gh workflow view build.yml
gh run list
```

---

## Troubleshooting

### Common Issues

#### .NET SDK Not Found
```bash
# Verify installation
dotnet --version

# If not found, download from:
# https://dotnet.microsoft.com/download
```

#### MAUI Workload Missing
```bash
# Install MAUI
dotnet workload install maui

# Update workloads
dotnet workload update

# List installed workloads
dotnet workload list
```

#### Build Fails with "Program.cs not found"
```bash
# Ensure Mainfile is copied correctly
cp Mainfile build/console/Program.cs

# Check file exists
ls -l build/console/Program.cs
```

#### Python Agent Fails
```bash
# Check Python version (3.8+ required)
python3 --version

# Install dependencies
pip install dataclasses  # Only needed for Python < 3.7
```

#### Android Build Fails
```bash
# Check Android SDK
dotnet workload repair

# Verify Java
java -version  # Should be JDK 11+

# Check Android SDK location
echo $ANDROID_HOME
```

#### iOS Build Fails
```bash
# Must be on macOS
# Check Xcode
xcodebuild -version

# Check provisioning profiles
# Open Xcode → Preferences → Accounts
```

### Getting Help

1. **Check build logs:**
   ```bash
   # Local builds
   cat build/console/build.log
   
   # CI/CD builds
   gh run view --log
   ```

2. **Run Python agent for diagnostics:**
   ```bash
   python3 beros_game.py
   ```

3. **Read generated reports:**
   - `build-agent-report.txt` - Python agent audit
   - `build-audit.md` - CI/CD build report

---

## Asset Requirements

Reference: `beros blueprint` file

### Required Assets:

**Sprites:**
- Gummy bears (brown, pink, blue) - 32x32px
- Tractors (3 colors) - 48x48px
- Crops (apple tree, cotton candy, lollipop) - 32x32px
- Racetrack tiles - 32x32px

**Audio:**
- Water splash SFX
- Harvest chime SFX
- Race start SFX
- Boost SFX

**Sources** (all free/CC0):
- OpenGameArt.org
- Kenney.nl
- CraftPix.net freebies

---

## Build Matrix

| Platform | File | Status | Prerequisites |
|----------|------|--------|---------------|
| .NET Console | `Mainfile` | ✅ Ready | .NET 8 SDK |
| MAUI Android | `dotnet BEROS` | ⚠️ Setup Required | .NET 8, MAUI, Android SDK |
| MAUI iOS | `dotnet BEROS` | ⚠️ Setup Required | macOS, Xcode, MAUI |
| Unity Mobile | `Mobile unity  setup` | ⚠️ Setup Required | Unity 2021.3+ |
| Server | `server` | ✅ Ready | .NET 8 SDK |
| Python Agent | `beros_game.py` | ✅ Ready | Python 3.8+ |

---

## Next Steps

1. ✅ Run automated build: `./build.sh`
2. ✅ Review Python agent report: `cat build-agent-report.txt`
3. ⚠️ Set up MAUI project (if targeting mobile)
4. ⚠️ Download game assets (see `beros blueprint`)
5. ⚠️ Configure multiplayer server (if needed)
6. ✅ Test .NET console version locally

---

## Additional Resources

- **Blueprint:** `beros blueprint` - Complete game specification
- **Workflow:** `workflows` - Original CI/CD configuration
- **README:** `README.md` - Project overview
- **License:** `LICENSE` - Project license

---

*Last Updated: 2025-11-20*
*Build System Version: 1.0.0*
