# Metroidvania Console Game

A complete metroidvania-style game built in C# for the console! Explore interconnected rooms, defeat enemies, collect projectile ammo, and face the challenging boss.

## Features

### Core Gameplay
- **Smooth Movement**: Fluid player controls with physics
- **6 Interconnected Rooms**: Freely explore and backtrack between areas
- **Projectile Combat**: Collect ammo to shoot enemies from a distance
- **Upward Aiming**: Aim and shoot upward to defeat flying enemies
- **Enemy AI**: Multiple enemy types with unique behaviors
- **Boss Battle**: Face off against a boss that actively chases you!
- **Victory Screen**: Complete the game by defeating the boss

### Technical Features
- **Flicker-Free Rendering**: Optimized console rendering for smooth visuals
- **Real-Time Physics**: Gravity, jumping, collision detection
- **Non-Blocking Input**: Responsive controls that don't freeze the game
- **Bidirectional Travel**: Move freely between all rooms
- **HUD Display**: Live health, score, and ammo tracking
- **Room Transition Cooldown**: Prevents accidental room switching

## How to Run

### Prerequisites
- .NET 6.0 SDK or later installed on your computer
- Windows, macOS, or Linux (cross-platform!)

### Running the Game

1. **Open a terminal/command prompt** in the game folder

2. **Build and run** with a single command:
   ```bash
   dotnet run
   ```

3. **Alternative**: Build first, then run:
   ```bash
   dotnet build
   dotnet run
   ```

### Controls

**Movement:**
- `A/D` or `Arrow Keys` - Move left/right
- `Space` or `Up Arrow` - Jump
- `W` - Aim upward (hold to keep aiming)

**Combat:**
- `F` - Melee attack (2 tiles range)
- `G` - Shoot projectile (requires ammo)

**Game:**
- `Enter` - Start game (from menu)
- `R` - Restart (from game over)
- `Q` or `ESC` - Quit

## Game Map

```
[Treasure] ← [Start] → [Challenge] → [Boss]
                            ↑
                        [Flyer] → [Arena]
```

### Room Descriptions

**Room 0 - Starting Room**
- Simple platforms to learn movement
- Basic enemy to practice combat
- 2 health pickups available
- Exit left to Treasure, right to Challenge

**Room 5 - Treasure Room (Left of Start)**
- Lots of health pickups (5 total)
- Lots of projectile ammo (4 total)
- Multiple platforms at different heights
- Safe zone with no enemies

**Room 1 - Challenge Room**
- Series of floating platforms
- 2 Walker enemies
- Ascending platforms on the right side lead up to Flyer room
- Ceiling gap at x=50-58 for upward exit
- 2 health pickups and 2 ammo pickups

**Room 4 - Flyer Room (Above Challenge)**
- 3 Flyer enemies that drop projectiles
- Platforms to navigate while dodging attacks
- Floor gap at x=50-58 to return to Challenge room
- 2 health pickups and 4 ammo pickups
- Exit right to Arena

**Room 6 - Arena Room (Right of Flyer)**
- 5 enemies total: 3 Walkers and 2 Flyers
- Multiple platforms for combat tactics
- 2 health pickups and 3 ammo pickups
- Intense combat challenge

**Room 3 - Boss Room (Right of Challenge)**
- Boss enemy with 10 health that **chases you**!
- 2 Flyer enemies to increase difficulty
- 2 health pickups and 2 ammo pickups
- Final challenge - defeat the boss to win!

## Game Mechanics

### Combat
- **Melee Attack**: Press F to attack enemies within 2 tiles
- **Projectile Attack**: Press G to shoot (uses 1 ammo per shot)
- **Upward Aiming**: Hold W while shooting to hit flying enemies
- **Enemy Health**: Walkers have 3 HP, Flyers have 2 HP, Boss has 10 HP
- **Damage Display**: Enemy health shows in UI for 3 seconds after taking damage

### Collectibles
- `♥` Health - Restores 1 health point (+10 score)
- `↑` Projectile Ammo - Adds 5 projectiles (+30 score)

### Enemies
- `M` Walker - Patrols back and forth (3 HP, +50 score)
- `F` Flyer - Flies in a wave pattern and drops projectiles (2 HP, +50 score)
- `B` Boss - **Actively chases the player** (10 HP, +50 score)

### Symbols
- `@` - You (the player)
- `→/←/↑` - Facing/aiming indicator
- `⚔` - You (while attacking)
- `█` - Solid wall
- `=` - Platform (you can jump through from below)
- `>/</^/v` - Player projectiles
- `M/F/B` - Enemies
- `*` and `·` - Melee attack visual (2-tile range)

## How It Meets Requirements

✅ **Keyboard Input**: A/D, Arrows, Space, W, F, G, R, Q, ESC
✅ **Start Menu**: Press Enter to start
✅ **Game Over Screen**: Shows final score with restart/quit options
✅ **Victory Screen**: Shows when boss is defeated
✅ **Dynamic Updates**: Real-time physics, enemy movement, scoring
✅ **Boundary Constraints**: Walls, platforms, room boundaries
✅ **Multiple Actions**: Move, jump, aim, melee attack, shoot projectiles
✅ **Score Tracking**: Points for enemies, collectibles
✅ **Game State Management**: Menu, playing, game over, victory states
✅ **Restart Functionality**: Press R after game over
✅ **Console-Only**: No external graphics libraries
✅ **Smooth Gameplay**: Non-blocking async input handling
✅ **Progress Tracking**: Score and health indicators
✅ **Visual Feedback**: Character changes, item collection, attack visuals

## Code Structure

```
MetroidvaniaGame/
├── Program.cs          - Entry point
├── Game.cs             - Main game loop and state management
├── Player.cs           - Player character with combat abilities
├── Room.cs             - Room data and generation
├── World.cs            - World management and room connections
├── GameObjects.cs      - Enemies, projectiles, and collectibles
├── Renderer.cs         - Flicker-free console rendering
└── MetroidvaniaGame.csproj - Project configuration
```

## Development Tips

### Adding New Rooms

1. Create a new room in `RoomGenerator` class:
```csharp
public static Room CreateMyRoom()
{
    Room room = new Room(60, 20);

    // Add floor, walls with gaps for doors (y=16-18 for ground level)
    // Add platforms, enemies, collectibles

    return room;
}
```

2. Add it to the world in `World.cs`:
```csharp
Room myRoom = RoomGenerator.CreateMyRoom();
myRoom.LeftRoom = 0; // Connect to other rooms
rooms[7] = myRoom;
```

### Adding New Enemy Types

1. Add to `EnemyType` enum in `GameObjects.cs`
2. Add health value in Enemy constructor
3. Create Update method (e.g., `UpdateMyEnemy()`)
4. Add sprite in `GetSprite()` method
5. For enemies that need player position (like Boss), use the player parameter

### Improving Physics

Edit constants in `Player.cs`:
```csharp
private const float GRAVITY = 25f;        // Fall speed
private const float JUMP_FORCE = 18f;     // Jump height (increased from 15f)
private const float MOVE_SPEED = 12f;     // Walk speed
```

### Changing Room Size

Default is 60x20, but you can change in `RoomGenerator`:
```csharp
Room room = new Room(80, 25); // Wider and taller
```

## Troubleshooting

**Game runs too fast/slow:**
- Adjust `TARGET_FPS` in `Game.cs`
- Default is 30 FPS

**Characters look weird:**
- Some terminals don't support special characters
- Edit `GetSprite()` methods to use basic ASCII

**Input feels laggy:**
- Try running in a different terminal
- Windows Command Prompt works best
- PowerShell and Terminal.app also work well

**Doors not working:**
- Walk directly into the wall gaps (at floor level, y=16-18)
- Gaps should be visible as openings in the walls

**Flickering:**
- The renderer updates only changed characters
- If flickering occurs, your terminal may not support `SetCursorPosition` well

## Next Steps & Ideas

Here are ways you can expand this game:

### Easy Additions
- More rooms and a larger map
- Additional enemy types
- More collectible types
- Sound effects (using Console.Beep)
- Different colored text (using Console.ForegroundColor)

### Medium Additions
- Save/load system (file I/O)
- Moving platforms
- Checkpoints that restore health
- Locked doors and keys
- Secret areas
- New weapon types

### Advanced Additions
- Map system showing explored rooms
- Shop system with currency
- Boss patterns with multiple phases
- Particle effects (using character animations)
- Procedurally generated rooms
- Achievements system

## Credits

Built for a class project demonstrating:
- Object-oriented design
- Game loop architecture
- Console manipulation
- Real-time physics simulation
- State management
- Enemy AI behaviors

Have fun exploring and expanding this game! 🎮
