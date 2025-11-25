# BEROS Game Manual

## 🐻 Welcome to BEROS: Gummy Bear Farming & Racing Adventure!

```
   ____  _____ ____   ___  ____  
  | __ )| ____|  _ \ / _ \/ ___| 
  |  _ \|  _| | |_) | | | \___ \ 
  | |_) | |___|  _ <| |_| |___) |
  |____/|_____|_| \_\\___/|____/ 
  
  Gummy Bear Farming & Racing Adventure
```

Welcome to the sweetest adventure in gaming! In BEROS, you play as a gummy bear farmer who tends magical candy crops and races on rainbow roads. This manual will teach you everything you need to know to become the ultimate gummy bear champion!

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Controls](#controls)
3. [Characters](#characters)
4. [Game Zones](#game-zones)
5. [Farming System](#farming-system)
6. [Racing System](#racing-system)
7. [Economy & Progression](#economy--progression)
8. [Multiplayer](#multiplayer)
9. [Tips & Tricks](#tips--tricks)
10. [Troubleshooting](#troubleshooting)

---

## Getting Started

### System Requirements

**Console Version (.NET 8)**
- Any OS with .NET 8 Runtime
- Terminal/Console with Unicode support
- Keyboard input

**Mobile Version (MAUI)**
- Android 5.0+ (API 21+)
- iOS 14.0+
- Touch screen

### Installation

#### Console Version
```bash
# Build and run
dotnet new console -n BEROS.Console
cp ConsoleGame.cs BEROS.Console/Program.cs
cd BEROS.Console
dotnet run -c Release
```

#### Mobile Version
See `BUILD.md` for detailed MAUI and Unity setup instructions.

### Starting the Game

When you launch BEROS, you'll see the title screen and be spawned in the **Orchard Garden** as a Brown Gummy Bear. You start with:
- 💎 100 Sparkles
- 🔑 0 Magic Keys
- 🌱 Farm Level 0
- 🌈 Rainbow Watering Can

---

## Controls

### Console/PC Controls

| Key | Action | Description |
|-----|--------|-------------|
| `W` | Move Up | Walk north on the map |
| `A` | Move Left | Walk west on the map |
| `S` | Move Down | Walk south on the map |
| `D` | Move Right | Walk east on the map |
| `E` | Interact | Water crops or harvest ready plants |
| `F` | Special Ability | Use your bear's unique power |
| `R` | Enter Race | Join or start a race (when at racetrack) |
| `Space` | Jump | Jump over obstacles |
| `Q` | Quit | Exit the game |

### Mobile Controls

| Control | Action |
|---------|--------|
| Virtual Joystick (Left) | Movement in all directions |
| Interact Button (Right) | Water/harvest nearby crops |
| Tap Screen | Jump or interact with objects |

---

## Characters

### Playable Gummy Bears

Each gummy bear has a unique color, tractor, and personality!

#### 🟤 Brown Gummy Bear
```
      .-"""-.
     /        \
    |  O    O  |
    |    __    |
     \  '--'  /
      '-.  .-'
```
- **Home Zone:** Orchard Garden
- **Tractor Color:** Brown with Apple Icon 🍎
- **Trait:** Durable - Takes less damage
- **Starting Zone:** Orchard Garden

#### 🩷 Pink Gummy Bear
```
      .-"""-.
     /  ♥  ♥  \
    |  @    @  |
    |    ~~    |
     \  '--'  /
      '-.  .-'
```
- **Home Zone:** Cotton Candy Fields
- **Tractor Color:** Pink with Cotton Icon 🍬
- **Trait:** Boost - Faster acceleration
- **Specialty:** Cotton candy crops

#### 🔵 Blue Gummy Bear
```
      .-"""-.
     /  ~  ~  \
    |  *    *  |
    |    ^^    |
     \  '--'  /
      '-.  .-'
```
- **Home Zone:** Lollipop Forest
- **Tractor Color:** Blue with Lollipop Icon 🍭
- **Trait:** Handling - Better control
- **Specialty:** Lollipop trees

### NPCs

- **Brown Gummy Bear NPC:** "Hello! Water my apples!" (Orchard Garden)
- More NPCs to discover in each zone!

---

## Game Zones

BEROS features four unique zones, each with its own atmosphere and activities:

### 🍎 Orchard Garden
```
Zone Layout:
............................
.  a  .  A  .  a  .  A  .  .
............................
.  A  .  a  .  A  .  a  .  .
............................
      [P] <- You
```
- **Owner:** Brown Gummy Bear
- **Crop:** Apple Trees
- **Plots:** 6 farm plots
- **Spawn Point:** (2, 2)
- **Activities:** Farming apples, talking to NPCs
- **Visual:** Rolling green hills with fruit trees

### 🍬 Cotton Candy Fields
- **Owner:** Pink Gummy Bear
- **Crop:** Cotton Candy bushes
- **Atmosphere:** Fluffy pink fields
- **Special:** Clouds of spun sugar float by

### 🍭 Lollipop Forest
- **Owner:** Blue Gummy Bear
- **Crop:** Lollipop trees
- **Atmosphere:** Swirling candy canes
- **Special:** Rainbow paths between trees

### 🌈 Rainbow Racetrack
```
🏁 START ══════════ Rainbow Road ══════════ FINISH 🏆
```
- **Capacity:** Up to 8 players
- **Activities:** Tractor racing, powerup collection
- **Spawn Point:** (0, 5)
- **Rewards:** Race wins earn Sparkles and Magic Keys

---

## Farming System

### The Basics

Farming is the core gameplay of BEROS. Grow magical candy crops to earn Sparkles!

#### Crop Growth Stages

1. **Seed** - Just planted, needs water
2. **Sapling** - Growing, keep watering
3. **Small Plant** - Almost there!
4. **Full Plant** - Nearly ready
5. **Ready to Harvest** - Glowing with ripe fruit! (Shown as capital letter)

#### Map Symbols
- `.` - Empty ground
- `a` - Growing apple tree (not ready)
- `A` - Harvestable apple tree (ready!)
- `P` - Your player position

### How to Farm

1. **Walk to a plot** using WASD controls
2. **Water the plot** by pressing `E` near a crop
3. **Wait for growth** - Watered crops grow 2x faster!
4. **Harvest** when ready - Press `E` near a ready crop (capital letter)

### Farming Stats

| Stat | Description |
|------|-------------|
| Growth Rate | 0.01 per tick (0.02 when watered) |
| Harvest Radius | 3 tiles |
| Sparkles per Harvest | 10-20 depending on crop |
| XP per Harvest | 10 XP |

### Tools

#### 🌈 Rainbow Watering Can
- Your starting tool
- Waters crops to speed up growth
- Creates rainbow sparkle particles when used
- Unlimited uses

### Crops by Zone

| Zone | Crop | Growth Time | Sparkles Reward |
|------|------|-------------|-----------------|
| Orchard Garden | Apple Tree 🍎 | 60 ticks | 15 |
| Cotton Candy Fields | Cotton Candy 🍬 | Variable | Variable |
| Lollipop Forest | Lollipop 🍭 | Variable | Variable |

---

## Racing System

### Entering a Race

1. Navigate to the **Rainbow Racetrack** zone
2. Press `R` to enter/start a race
3. Race begins when all players are ready!

### Race Controls

- **WASD/Joystick:** Steer your tractor
- **Boost:** Collect powerups for speed boosts
- **Drift:** Maintain speed through turns

### Tractor Traits

| Bear | Tractor Trait | Effect |
|------|---------------|--------|
| Brown | Durable | Withstands collisions better |
| Pink | Boost | Faster acceleration & speed |
| Blue | Handling | Tighter turns & control |

### Powerups

Collect these on the track:

| Powerup | Effect |
|---------|--------|
| ⚡ Speed Boost | Temporary speed increase |
| 🌈 Rainbow Trail | Leave a slippery trail |
| 🐰 Bunny Summon | Hop ahead of opponents |

### Race Rewards

| Placement | Sparkles | Magic Keys |
|-----------|----------|------------|
| 🥇 1st Place | 50 | 1 |
| 🥈 2nd Place | 30 | 0 |
| 🥉 3rd Place | 20 | 0 |

---

## Economy & Progression

### Currency

#### 💎 Sparkles
- Primary currency
- Earned from: Harvesting (10-20), Racing (50/win)
- Used for: Buying tools, unlocking zones

#### 🔑 Magic Keys
- Premium currency
- Earned from: Race wins (1), Level ups (5)
- Used for: Unlocking secret areas (requires 3+)

### Leveling System

Your **Farm Level** increases as you earn XP:

```
Level = √(XP / 100)
```

| Level | XP Required | Rewards |
|-------|-------------|---------|
| 1 | 100 | Basic Tools |
| 5 | 2,500 | New Zone Access |
| 10 | 10,000 | Advanced Tool |
| 25 | 62,500 | Special Tractor Upgrade |
| 50 | 250,000 | Master Farmer Title |

### Racing Ranks

Progress through racing leagues:

1. 🟢 **Beginner** - Learning the tracks
2. 🔵 **Novice** - Getting competitive
3. 🟣 **Expert** - Skilled racer
4. 🟡 **Champion** - Rainbow Road Master!

---

## Multiplayer

### Local Multiplayer

BEROS currently ships as a single-player experience; multiplayer support via SignalR is planned for a future update.

### Setting Up a Game

1. **Host runs the server:**
   ```bash
   dotnet run --urls=http://0.0.0.0:5000
   ```

2. **Players connect:**
   - Join room code: `GUMMY123`
   - Or create custom room codes

### Private Rooms

- Enter room password when prompted
- Only friends with the code can join
- Supports up to 8 players

### Synced Features

| Feature | Synced? |
|---------|---------|
| Player Positions | ✅ Yes |
| Crop Growth | ✅ Yes |
| Race Results | ✅ Yes |
| Chat | ✅ Yes |

---

## Tips & Tricks

### Farming Tips

1. **Always water your crops!** Watered crops grow 2x faster
2. **Harvest in batches** for efficiency
3. **Visit all zones** to maximize variety
4. **Keep crops planted** - Empty plots earn nothing

### Racing Tips

1. **Choose your bear wisely:**
   - New players: Blue (Handling)
   - Speed demons: Pink (Boost)
   - Defensive: Brown (Durable)

2. **Learn the track layouts**
3. **Save boosts for straightaways**
4. **Cut corners carefully**

### Economy Tips

1. **Farm before racing** - Build up Sparkles first
2. **Race for Magic Keys** - They're harder to get
3. **Invest in tools early** - They pay off quickly
4. **Save 3 keys** for secret area access

### General Tips

1. **Explore all zones** - Each has unique content
2. **Talk to NPCs** - They give hints and quests
3. **Play with friends** - Multiplayer is more fun!
4. **Check your status bar** - Know your resources

---

## Troubleshooting

### Common Issues

#### Game Won't Start
```bash
# Check .NET version
dotnet --version  # Should be 8.0+

# Rebuild
dotnet build -c Release
```

#### Controls Not Responding
- Make sure the console window is focused
- Check keyboard layout (QWERTY expected)
- Try pressing keys slowly

#### Can't See All Characters
- Enable Unicode in your terminal
- Use a font that supports emoji
- Try a different terminal emulator

#### Multiplayer Connection Failed
1. Check server is running
2. Verify IP address is correct
3. Ensure port 5000 is not blocked
4. Check firewall settings

### Performance Issues

- Close other applications
- Reduce terminal size if laggy
- Update .NET runtime

### Getting Help

1. Check `BUILD.md` for setup issues
2. Review `beros blueprint` for game specs
3. Check GitHub Issues for known problems

---

## Credits

### Development Team
- **ncsound919** - Creator & Developer

### Technologies Used
- .NET 8 SDK
- C# Programming Language
- SkiaSharp (Mobile Graphics)
- SignalR (Multiplayer)
- MAUI (Mobile Framework)
- Unity (Alternative Mobile)

### Asset Sources (CC0/Free)
- OpenGameArt.org
- Kenney.nl
- CraftPix.net freebies

### Special Thanks
- The .NET MAUI team
- The SkiaSharp community
- All playtesters and contributors!

---

## Version History

### v1.0.0 (2025)
- Initial release
- 3 playable gummy bears
- 4 unique zones
- Farming and racing systems
- Local multiplayer support

---

## Quick Reference Card

```
╔═══════════════════════════════════════════════════════════╗
║                    BEROS QUICK REFERENCE                  ║
╠═══════════════════════════════════════════════════════════╣
║  CONTROLS                                                 ║
║  W/A/S/D = Move    E = Interact    F = Special           ║
║  R = Race          Space = Jump    Q = Quit              ║
╠═══════════════════════════════════════════════════════════╣
║  ZONES                                                    ║
║  🍎 Orchard Garden    🍬 Cotton Candy Fields              ║
║  🍭 Lollipop Forest   🌈 Rainbow Racetrack                ║
╠═══════════════════════════════════════════════════════════╣
║  RESOURCES                                                ║
║  💎 Sparkles - Earn from farming (15+) & racing (50)     ║
║  🔑 Magic Keys - Earn from wins (1) & level ups (5)      ║
╠═══════════════════════════════════════════════════════════╣
║  FARMING                                                  ║
║  1. Walk to crop  2. Press E to water  3. Harvest when A ║
╠═══════════════════════════════════════════════════════════╣
║  RACING                                                   ║
║  Go to Racetrack → Press R → Win → Get rewards!          ║
╚═══════════════════════════════════════════════════════════╝
```

---

*BEROS v1.0.0 | © 2025 ncsound919 | MIT License*

**Have fun farming and racing!** 🐻🍎🏎️🌈
