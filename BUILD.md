# BEROS Game - Build Documentation

Complete guide for building BEROS (Gummy Bear Farming & Racing) game across all platforms.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start](#quick-start)
3. [Android Build](#android-build)
4. [Platform-Specific Builds](#platform-specific-builds)
5. [CI/CD Pipeline](#cicd-pipeline)
6. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Core Requirements

- **Git** - Version control
- **.NET 10 SDK** - For console and MAUI builds
  - Download: https://dotnet.microsoft.com/download
  - Verify: `dotnet --version`

### Android Build Requirements

- .NET 10 SDK
- MAUI Android Workload
  ```bash
  dotnet workload install maui-android
  ```
- Java Development Kit (JDK 17+)
  - On Ubuntu: `sudo apt install openjdk-17-jdk`

### .NET Console (All Platforms)
- .NET 10 SDK (required)
- No additional dependencies

### Python Game Agent
- Python 3.8+

---

## Quick Start

### Build Android APK (Recommended)

```bash
# Navigate to mobile project
cd BEROS.Mobile

# Restore dependencies
dotnet restore

# Build Debug APK
dotnet build -f net10.0-android -c Debug

# Build Release APK
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk
```

The APK will be in: `BEROS.Mobile/bin/Release/net10.0-android/publish/`

### Build Console Version

```bash
# Create project
dotnet new console -n BEROS.Console -o ./build/console

# Copy game code
cp ConsoleGame.cs ./build/console/Program.cs

# Build
cd ./build/console
dotnet build -c Release

# Run
dotnet run -c Release
```

---

## Android Build

### Project Structure

The Android app is in the `BEROS.Mobile/` directory:

```
BEROS.Mobile/
├── App.xaml                    # Application resources
├── App.xaml.cs                 # Application entry point
├── AppShell.xaml               # Navigation shell
├── BEROS.Mobile.csproj         # Project file (Android target)
├── MainPage.xaml               # Game UI layout
├── MainPage.xaml.cs            # Game logic with SkiaSharp
├── MauiProgram.cs              # MAUI configuration
├── Platforms/
│   └── Android/
│       ├── AndroidManifest.xml # Android permissions
│       ├── MainActivity.cs     # Android activity
│       └── MainApplication.cs  # Android application
└── Resources/                  # App resources (icons, fonts, etc.)
```

### Build Commands

```bash
# Debug build (for development/testing)
cd BEROS.Mobile
dotnet build -f net10.0-android -c Debug

# Release build (for distribution)
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk

# Build signed APK for Play Store
dotnet publish -f net10.0-android -c Release \
  -p:AndroidPackageFormat=aab \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=mykey.keystore \
  -p:AndroidSigningKeyAlias=mykey \
  -p:AndroidSigningKeyPass=password \
  -p:AndroidSigningStorePass=password
```

### Game Features

- **SkiaSharp** canvas-based 2D rendering at 60 FPS
- Touch controls with virtual joystick
- Farm plots with watering and harvesting mechanics
- Gummy bear character with animations
- Rainbow particle effects
- Zone-based gameplay (Orchard Garden, Rainbow Racetrack)

### Install on Device

```bash
# Enable USB debugging on your Android device
# Connect device via USB

# List connected devices
adb devices

# Install APK
adb install BEROS.Mobile/bin/Release/net10.0-android/publish/com.beros.mobile-Signed.apk
```

---

## Platform-Specific Builds

### 1. .NET Console Game

**Target Platforms:** Windows, macOS, Linux

**File:** `ConsoleGame.cs` (contains complete game code)

#### Build Steps:

```bash
# Create console project
mkdir -p build/console
cd build/console
dotnet new console -n BEROS.Console

# Copy game code
cp ../../ConsoleGame.cs Program.cs

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
- `bin/Release/net10.0/BEROS.Console.dll`
- `bin/Release/net10.0/BEROS.Console.exe` (Windows)

---

### 2. MAUI Mobile (Android)

**Target Platform:** Android (API 21+)

**Directory:** `BEROS.Mobile/` (complete MAUI project)

#### Build Steps:

```bash
# Navigate to project
cd BEROS.Mobile

# Restore packages
dotnet restore

# Debug build
dotnet build -f net10.0-android -c Debug

# Release build (APK)
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=apk

# Release build (AAB for Play Store)
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=aab
```

#### Install on Device:

```bash
# Install Debug APK
adb install bin/Debug/net10.0-android/com.beros.mobile-Signed.apk

# Install Release APK
adb install bin/Release/net10.0-android/publish/com.beros.mobile-Signed.apk
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
   - Builds .NET 10 console version
   - Runs on Ubuntu
   - Uploads build artifacts

2. **build-maui-android**
   - Builds Android APK using MAUI
   - Installs Java JDK and MAUI workload
   - Uploads signed APK as artifact
   - Runs on Ubuntu

3. **build-python-agent**
   - Runs Python code linting
   - Executes game agent audit
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

**Download Android APK:**
After a successful build, download the APK artifact from GitHub Actions.

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

#### MAUI Android Workload Missing
```bash
# Install MAUI Android workload
dotnet workload install maui-android

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
# Install MAUI Android workload
dotnet workload install maui-android

# Repair workload
dotnet workload repair

# Verify Java (JDK 17+ required for .NET 10)
java -version

# Restore packages
cd BEROS.Mobile
dotnet restore
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
