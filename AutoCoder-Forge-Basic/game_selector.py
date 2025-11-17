#!/usr/bin/env python3
"""
Game Selector for AutoCoder-Forge-Basic
Allows users to select and generate urban hip-hop themed games.
"""

import os
import subprocess
from pathlib import Path

import yaml

ROOT = Path(__file__).resolve().parent
TASK_DIR = ROOT / "tasks"
OUT_DIR = ROOT / "out"

GAME_PRESETS = {
    "platformer": {
        "name": "Urban Beats Platformer",
        "description": "Jump across rooftops, collect beats, avoid obstacles.",
        "task_file": "generate_urban_platformer.yaml",
    },
    "tetris": {
        "name": "Urban Block Stack",
        "description": "Stack blocks to build skyscrapers and create beats.",
        "task_file": "generate_urban_tetris.yaml",
    },
    "slingshot": {
        "name": "Urban Slingshot Fury",
        "description": "Launch projectiles at city targets to cause chaos.",
        "task_file": "generate_urban_slingshot.yaml",
    },
    # Add more presets here as templates are created
}


def display_menu():
    print("Welcome to AutoCoder Game Generator!")
    print("Choose a game type to generate:")
    for key, preset in GAME_PRESETS.items():
        print(f"{key}: {preset['name']} - {preset['description']}")
    print("Type 'quit' to exit.")


def get_user_choice():
    while True:
        choice = input("Enter your choice: ").strip().lower()
        if choice in GAME_PRESETS:
            return choice
        elif choice == "quit":
            return None
        else:
            print("Invalid choice. Please try again.")


def generate_game(choice):
    preset = GAME_PRESETS[choice]
    task_file = TASK_DIR / preset["task_file"]

    if not task_file.exists():
        print(
            f"Task file {preset['task_file']} not found. Please ensure templates are set up."
        )
        return

    print(f"Generating {preset['name']}...")

    # Run the autocoder
    result = subprocess.run(
        ["python", "autocoder.py", "--once"], cwd=ROOT, capture_output=True, text=True
    )

    if result.returncode == 0:
        print("Game generated successfully!")
        output_dir = OUT_DIR / f"urban_{choice}"
        print(f"Output directory: {output_dir}")
        print("Open the project in Godot to play.")
    else:
        print("Error generating game:")
        print(result.stderr)


def main():
    while True:
        display_menu()
        choice = get_user_choice()
        if choice is None:
            break
        generate_game(choice)
        input("Press Enter to continue...")


if __name__ == "__main__":
    main()
