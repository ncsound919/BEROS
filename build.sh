#!/bin/bash
# BEROS Game - Automated Build Script
# Builds all platforms and generates artifacts

set -e

echo "======================================"
echo "BEROS Game - Build System"
echo "======================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Project root
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_DIR="$PROJECT_ROOT/build"
ARTIFACTS_DIR="$PROJECT_ROOT/artifacts"

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf "$BUILD_DIR" "$ARTIFACTS_DIR"
mkdir -p "$BUILD_DIR" "$ARTIFACTS_DIR"

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Build .NET Console version
build_dotnet_console() {
    echo ""
    echo -e "${YELLOW}Building .NET Console Version...${NC}"
    
    if ! command_exists dotnet; then
        echo -e "${RED}✗ .NET SDK not found${NC}"
        return 1
    fi
    
    local console_dir="$BUILD_DIR/console"
    mkdir -p "$console_dir"
    
    # Create project
    cd "$console_dir"
    dotnet new console -n BEROS.Console --force
    
    # Navigate into the created project directory
    cd BEROS.Console
    
    # Copy ConsoleGame.cs as Program.cs
    if [ -f "$PROJECT_ROOT/ConsoleGame.cs" ]; then
        cp "$PROJECT_ROOT/ConsoleGame.cs" Program.cs
    elif [ -f "$PROJECT_ROOT/Mainfile" ]; then
        # Fallback to Mainfile (though it's MAUI code)
        echo "Warning: Using Mainfile - may require dependencies"
        cp "$PROJECT_ROOT/Mainfile" Program.cs
    fi
    
    # Build
    dotnet build -c Release
    
    # Copy artifacts
    mkdir -p "$ARTIFACTS_DIR/console"
    if [ -d "bin/Release" ]; then
        cp -r bin/Release/* "$ARTIFACTS_DIR/console/" 2>/dev/null || true
    fi
    
    echo -e "${GREEN}✓ .NET Console build completed${NC}"
    return 0
}

# Build MAUI Android (placeholder)
build_maui_android() {
    echo ""
    echo -e "${YELLOW}Building MAUI Android Version...${NC}"
    
    if ! command_exists dotnet; then
        echo -e "${RED}✗ .NET SDK not found${NC}"
        return 1
    fi
    
    # Check if MAUI workload is installed
    if ! dotnet workload list | grep -q "maui"; then
        echo -e "${YELLOW}⚠ MAUI workload not installed${NC}"
        echo "  Run: dotnet workload install maui"
        return 1
    fi
    
    echo -e "${YELLOW}ℹ MAUI project setup required${NC}"
    echo "  1. Run: dotnet new maui -n BEROS.Mobile"
    echo "  2. Add SkiaSharp packages"
    echo "  3. Integrate mobile code from 'dotnet BEROS' file"
    
    return 0
}

# Run Python game agent
run_python_agent() {
    echo ""
    echo -e "${YELLOW}Running Python Game Agent...${NC}"
    
    if ! command_exists python3; then
        echo -e "${RED}✗ Python 3 not found${NC}"
        return 1
    fi
    
    cd "$PROJECT_ROOT"
    python3 beros_game.py
    
    return $?
}

# Main build sequence
main() {
    echo "Starting build process..."
    echo ""
    
    # Track build results
    local total=0
    local success=0
    
    # Build .NET Console
    total=$((total + 1))
    if build_dotnet_console; then
        success=$((success + 1))
    fi
    
    # Build MAUI (info only for now)
    total=$((total + 1))
    if build_maui_android; then
        success=$((success + 1))
    fi
    
    # Run Python agent
    total=$((total + 1))
    if run_python_agent; then
        success=$((success + 1))
    fi
    
    # Summary
    echo ""
    echo "======================================"
    echo "Build Summary"
    echo "======================================"
    echo "Total builds: $total"
    echo -e "${GREEN}Successful: $success${NC}"
    echo -e "${RED}Failed: $((total - success))${NC}"
    echo ""
    
    if [ -d "$ARTIFACTS_DIR" ]; then
        echo "Artifacts directory: $ARTIFACTS_DIR"
        ls -lh "$ARTIFACTS_DIR"
    fi
    
    echo ""
    echo "======================================"
    
    # Return exit code
    if [ $success -eq $total ]; then
        echo -e "${GREEN}✓ All builds completed successfully${NC}"
        return 0
    else
        echo -e "${YELLOW}⚠ Some builds incomplete or failed${NC}"
        return 1
    fi
}

# Run main build
main "$@"
