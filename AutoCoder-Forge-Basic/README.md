# AutoCoder-Forge-Basic

An enhanced version of AutoCoder, a lightweight tool for generating 2D/3D video games with presets and templates. Inspired by classic games like Roblox, Tetris, Angry Birds, Frogger, Super Mario, Contra, Zelda, and Pac-Man, but reimagined with urban environments and hip-hop themes.

## Description

AutoCoder-Forge-Basic uses YAML tasks and Jinja2 templates to generate complete Godot projects for various game types. Each game is themed around urban settings with hip-hop elements, such as jumping across rooftops, stacking blocks to build skyscrapers, or launching projectiles in cityscapes.

The tool includes a game selector for easy generation of different game presets.

## Features

- **Game Presets**:
  - **Urban Beats Platformer**: Jump across rooftops, collect beats, avoid obstacles (inspired by Super Mario).
  - **Urban Block Stack**: Stack blocks to build skyscrapers and create beats (inspired by Tetris).
  - **Urban Slingshot Fury**: Launch projectiles at city targets to cause chaos (inspired by Angry Birds).
  - More presets can be added by creating new templates and tasks.

- **Urban Hip-Hop Themes**: All games incorporate urban environments, beats, and hip-hop culture elements.

- **Godot Integration**: Generates ready-to-run Godot 4.1+ projects.

- **Template-Based Generation**: Uses Jinja2 templates for customizable code generation.

- **Easy Selection**: Run `game_selector.py` to choose and generate games interactively.

## Requirements

- Python 3.8+
- Godot 4.1 or later (for running the generated games)
- Dependencies listed in `requirements.txt`:
  - jinja2
  - pyyaml
  - watchdog
  - black
  - pytest
  - fastapi
  - uvicorn[standard]

## Installation

1. Clone or download the repository.
2. Install Python dependencies:
   ```
   pip install -r requirements.txt
   ```
3. Ensure Godot is installed on your system.

## Usage

### Generating Games

1. Run the game selector:
   ```
   python game_selector.py
   ```
2. Choose a game type from the menu (e.g., platformer, tetris, slingshot).
3. The tool will generate the game files in the `out/` directory.

### Manual Generation

- Edit or create YAML tasks in `tasks/`.
- Run the AutoCoder:
  ```
  python autocoder.py --once
  ```
- Generated projects will be in `out/`.

### Running Generated Games

1. Open the generated project folder (e.g., `out/urban_platformer/`) in Godot.
2. Press F5 to run the game.

## Project Structure

- `autocoder.py`: Main script for processing tasks and rendering templates.
- `game_selector.py`: Interactive script for selecting game presets.
- `templates/`: Jinja2 templates for generating game code and assets.
- `tasks/`: YAML files defining what to generate.
- `out/`: Output directory for generated projects.
- `reports/`: JSON reports of generation results.

## Adding New Game Types

1. Create new Jinja2 templates in `templates/` (e.g., scripts, scenes).
2. Create a new YAML task in `tasks/` referencing the templates.
3. Add the preset to `GAME_PRESETS` in `game_selector.py`.

## Examples

### Generate a Platformer

Run `game_selector.py` and select "platformer". This creates a Godot project with:
- Player character that jumps on platforms.
- Collectible beats.
- Enemies to avoid.
- Urban rooftop levels.

### Customize a Game

Modify the context in the YAML task or edit the templates to change themes, mechanics, or assets.

## Contributing

Contributions are welcome! Please submit issues or pull requests for new game presets, improvements, or bug fixes.

## License

This project is open-source. See LICENSE file for details.

## Credits

Generated games are inspired by classic titles but adapted for urban hip-hop themes. Built on top of the original AutoCoder framework.