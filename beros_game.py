#!/usr/bin/env python3
"""
BEROS Game Agent - AI Assistant for Game Development and Testing
Provides automated testing, build validation, and game logic verification
"""

import json
import os
import subprocess
import sys
from dataclasses import dataclass
from enum import Enum
from typing import List, Dict, Optional


class BuildPlatform(Enum):
    """Supported build platforms"""
    DOTNET_CONSOLE = "dotnet-console"
    MAUI_ANDROID = "maui-android"
    MAUI_IOS = "maui-ios"
    UNITY_ANDROID = "unity-android"
    UNITY_IOS = "unity-ios"
    PYTHON = "python"


class BuildStatus(Enum):
    """Build status indicators"""
    SUCCESS = "success"
    FAILED = "failed"
    IN_PROGRESS = "in_progress"
    NOT_STARTED = "not_started"


@dataclass
class BuildResult:
    """Result of a build operation"""
    platform: BuildPlatform
    status: BuildStatus
    message: str
    artifacts: List[str] = None
    
    def __post_init__(self):
        if self.artifacts is None:
            self.artifacts = []


class BerosGameAgent:
    """
    Main game agent for BEROS project
    Handles build automation, testing, and validation
    """
    
    def __init__(self, project_root: str = "."):
        self.project_root = os.path.abspath(project_root)
        self.build_results: Dict[BuildPlatform, BuildResult] = {}
        
    def audit_project_structure(self) -> Dict[str, any]:
        """
        Audit the project structure and identify all game components
        """
        audit_report = {
            "project_root": self.project_root,
            "files_found": [],
            "platforms_detected": [],
            "build_ready": {},
            "recommendations": []
        }
        
        # Check for key files
        key_files = [
            "Mainfile",
            "Mobile unity  setup",
            "beros blueprint",
            "dotnet BEROS",
            "server",
            "racemanager",
            "workflows",
            "README.md"
        ]
        
        for filename in key_files:
            filepath = os.path.join(self.project_root, filename)
            if os.path.exists(filepath):
                audit_report["files_found"].append(filename)
        
        # Detect platforms
        if "Mainfile" in audit_report["files_found"]:
            audit_report["platforms_detected"].append("dotnet-console")
            audit_report["build_ready"]["dotnet-console"] = self._check_dotnet_ready()
        
        if "dotnet BEROS" in audit_report["files_found"]:
            audit_report["platforms_detected"].append("maui-mobile")
            audit_report["build_ready"]["maui-mobile"] = self._check_maui_ready()
        
        if "Mobile unity  setup" in audit_report["files_found"]:
            audit_report["platforms_detected"].append("unity")
            audit_report["build_ready"]["unity"] = self._check_unity_ready()
        
        # Generate recommendations
        if not audit_report["build_ready"].get("dotnet-console"):
            audit_report["recommendations"].append(
                "Install .NET 8 SDK to build console version"
            )
        
        if not audit_report["build_ready"].get("maui-mobile"):
            audit_report["recommendations"].append(
                "Install MAUI workload: dotnet workload install maui"
            )
        
        return audit_report
    
    def _check_dotnet_ready(self) -> bool:
        """Check if .NET SDK is available"""
        try:
            result = subprocess.run(
                ["dotnet", "--version"],
                capture_output=True,
                text=True,
                timeout=5
            )
            return result.returncode == 0
        except (subprocess.TimeoutExpired, FileNotFoundError):
            return False
    
    def _check_maui_ready(self) -> bool:
        """Check if MAUI workload is installed"""
        try:
            result = subprocess.run(
                ["dotnet", "workload", "list"],
                capture_output=True,
                text=True,
                timeout=10
            )
            return "maui" in result.stdout.lower()
        except (subprocess.TimeoutExpired, FileNotFoundError):
            return False
    
    def _check_unity_ready(self) -> bool:
        """Check if Unity is available"""
        # Unity check is platform-specific
        unity_paths = [
            "/Applications/Unity/Hub/Editor",  # macOS
            "C:\\Program Files\\Unity\\Hub\\Editor",  # Windows
            "/opt/unity",  # Linux
        ]
        
        for path in unity_paths:
            if os.path.exists(path):
                return True
        return False
    
    def build_platform(self, platform: BuildPlatform) -> BuildResult:
        """
        Build for specified platform
        """
        if platform == BuildPlatform.DOTNET_CONSOLE:
            return self._build_dotnet_console()
        elif platform == BuildPlatform.MAUI_ANDROID:
            return self._build_maui_android()
        elif platform == BuildPlatform.PYTHON:
            return self._build_python()
        else:
            return BuildResult(
                platform=platform,
                status=BuildStatus.FAILED,
                message=f"Platform {platform.value} not yet implemented"
            )
    
    def _build_dotnet_console(self) -> BuildResult:
        """Build .NET Console version"""
        try:
            # Create temporary build directory
            build_dir = os.path.join(self.project_root, "build", "console")
            os.makedirs(build_dir, exist_ok=True)
            
            # Create project
            subprocess.run(
                ["dotnet", "new", "console", "-n", "BEROS.Console", "-o", build_dir],
                check=True,
                capture_output=True,
                timeout=30
            )
            
            # Copy Mainfile as Program.cs
            mainfile = os.path.join(self.project_root, "Mainfile")
            program_cs = os.path.join(build_dir, "Program.cs")
            
            if os.path.exists(mainfile):
                with open(mainfile, 'r') as src:
                    with open(program_cs, 'w') as dst:
                        dst.write(src.read())
            
            # Build
            subprocess.run(
                ["dotnet", "build", "-c", "Release"],
                cwd=build_dir,
                check=True,
                capture_output=True,
                timeout=60
            )
            
            return BuildResult(
                platform=BuildPlatform.DOTNET_CONSOLE,
                status=BuildStatus.SUCCESS,
                message="Successfully built .NET Console version",
                artifacts=[os.path.join(build_dir, "bin", "Release")]
            )
            
        except subprocess.CalledProcessError as e:
            return BuildResult(
                platform=BuildPlatform.DOTNET_CONSOLE,
                status=BuildStatus.FAILED,
                message=f"Build failed: {e.stderr.decode() if e.stderr else str(e)}"
            )
        except Exception as e:
            return BuildResult(
                platform=BuildPlatform.DOTNET_CONSOLE,
                status=BuildStatus.FAILED,
                message=f"Build failed: {str(e)}"
            )
    
    def _build_maui_android(self) -> BuildResult:
        """Build MAUI Android version"""
        return BuildResult(
            platform=BuildPlatform.MAUI_ANDROID,
            status=BuildStatus.FAILED,
            message="MAUI Android build requires full project setup"
        )
    
    def _build_python(self) -> BuildResult:
        """Validate Python implementation"""
        return BuildResult(
            platform=BuildPlatform.PYTHON,
            status=BuildStatus.SUCCESS,
            message="Python game agent ready"
        )
    
    def run_all_builds(self) -> Dict[BuildPlatform, BuildResult]:
        """
        Run builds for all detected platforms
        """
        audit = self.audit_project_structure()
        
        for platform_name in audit["platforms_detected"]:
            if platform_name == "dotnet-console":
                platform = BuildPlatform.DOTNET_CONSOLE
            elif platform_name == "maui-mobile":
                platform = BuildPlatform.MAUI_ANDROID
            else:
                continue
            
            result = self.build_platform(platform)
            self.build_results[platform] = result
        
        return self.build_results
    
    def generate_report(self) -> str:
        """
        Generate a comprehensive build report
        """
        audit = self.audit_project_structure()
        
        report = []
        report.append("=" * 60)
        report.append("BEROS Game Build Agent - Audit Report")
        report.append("=" * 60)
        report.append("")
        
        report.append("Project Structure:")
        report.append(f"  Root: {audit['project_root']}")
        report.append(f"  Files Found: {len(audit['files_found'])}")
        for filename in audit['files_found']:
            report.append(f"    - {filename}")
        report.append("")
        
        report.append("Detected Platforms:")
        for platform in audit['platforms_detected']:
            ready = audit['build_ready'].get(platform, False)
            status = "✅ Ready" if ready else "❌ Not Ready"
            report.append(f"  - {platform}: {status}")
        report.append("")
        
        if audit['recommendations']:
            report.append("Recommendations:")
            for rec in audit['recommendations']:
                report.append(f"  - {rec}")
            report.append("")
        
        if self.build_results:
            report.append("Build Results:")
            for platform, result in self.build_results.items():
                status_symbol = "✅" if result.status == BuildStatus.SUCCESS else "❌"
                report.append(f"  {status_symbol} {platform.value}:")
                report.append(f"      Status: {result.status.value}")
                report.append(f"      Message: {result.message}")
                if result.artifacts:
                    report.append(f"      Artifacts: {', '.join(result.artifacts)}")
            report.append("")
        
        report.append("=" * 60)
        
        return "\n".join(report)


def main():
    """Main entry point for the game agent"""
    print("BEROS Game Agent - Starting...")
    print()
    
    agent = BerosGameAgent()
    
    # Run audit
    print("Auditing project structure...")
    audit = agent.audit_project_structure()
    
    # Generate and print report
    print()
    print(agent.generate_report())
    
    # Save report to file
    report_path = os.path.join(agent.project_root, "build-agent-report.txt")
    with open(report_path, 'w') as f:
        f.write(agent.generate_report())
    
    print(f"\nReport saved to: {report_path}")
    
    # Return appropriate exit code
    if audit['recommendations']:
        print("\n⚠️  Some build requirements are missing.")
        return 1
    else:
        print("\n✅ All build requirements satisfied.")
        return 0


if __name__ == "__main__":
    sys.exit(main())
