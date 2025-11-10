# Metroidvania Console Game

A complete metroidvania-style game built in C# for the console! Explore interconnected rooms, unlock abilities, defeat enemies, and discover secrets.

## Features

### Core Gameplay
- **Smooth Movement**: Fluid player controls with acceleration and physics
- **Multiple Rooms**: 4 interconnected areas to explore
- **Ability Progression**: Unlock Double Jump and Dash abilities
- **Enemy AI**: Multiple enemy types with different behaviors
- **Collectibles**: Health pickups, ability upgrades, and score items
- **Boss Battle**: Face off against a challenging boss enemy

### Technical Features
- **Flicker-Free Rendering**: Optimized console rendering for smooth visuals
- **Real-Time Physics**: Gravity, jumping, collision detection
- **Non-Blocking Input**: Responsive controls that don't freeze the game
- **Room Transitions**: Seamless movement between connected areas
- **HUD Display**: Live health, score, and ability tracking

## How to Run

### Prerequisites
- .NET 6.0 SDK or later installed on your computer
- Windows, macOS, or Linux (cross-platform!)

### Running the Game

1. **Open a terminal/command prompt** in the `MetroidvaniaGame` folder

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
- `WASD` or `Arrow Keys` - Move left/right
- `Space` or `W` or `Up Arrow` - Jump
- `Shift` - Dash (once unlocked)

**Game:**
- `Enter` - Start game (from menu)
- `R` - Restart (from game over)
- `Q` or `ESC` - Quit

## Game Map

```
        [Room 2]
     (Ability Room)
           |
[Room 0]--[Room 1]--[Room 3]
 (Start)  (Challenge) (Boss)
```

### Room Descriptions

**Room 0 - Starting Room**
- Simple platforms to learn movement
- Basic enemy to practice combat
- Health pickup available
- Exit right to Room 1, up to Room 2

**Room 2 - Ability Room (Above Start)**
- High platforms requiring precise jumping
- **Double Jump ability** at the top
- Practice your new mobility!

**Room 1 - Challenge Room**
- Series of floating platforms
- Multiple enemies to avoid
- **Dash ability** at the end
- Leads to the boss room

**Room 3 - Boss Room**
- Final challenge!
- Boss enemy with 10 health
- Defeat it to win!

## Game Mechanics

### Abilities
- **Double Jump**: Jump again in mid-air (unlock in Room 2)
- **Dash**: Quick horizontal burst of speed (unlock in Room 1)

### Collectibles
- `♥` Health - Restores 1 health point (+10 score)
- `^` Double Jump - Unlocks double jump ability (+100 score)
- `»` Dash - Unlocks dash ability (+100 score)
- `o` Coin - Bonus points (+25 score)

### Enemies
- `M` Walker - Patrols back and forth (1 HP, +50 score)
- `F` Flyer - Flies in a wave pattern (2 HP, +50 score)
- `B` Boss - Slow but powerful (10 HP, +50 score)

### Symbols
- `@` - You (the player)
- `→/←` - You (while dashing)
- `█` - Solid wall
- `=` - Platform (you can jump through from below)
- `M/F/B` - Enemies

## How It Meets Requirements

✅ **Keyboard Input**: WASD, Arrows, Space, Shift, R, Q, ESC
✅ **Start Menu**: Press Enter to start
✅ **Game Over Screen**: Shows final score with restart/quit options
✅ **Dynamic Updates**: Real-time physics, enemy movement, scoring
✅ **Boundary Constraints**: Walls, platforms, room boundaries
✅ **Multiple Actions**: Move, jump, dash, collect items
✅ **Score Tracking**: Points for enemies, collectibles, abilities
✅ **Game State Management**: Menu, playing, game over states
✅ **Restart Functionality**: Press R after game over
✅ **Console-Only**: No external graphics libraries
✅ **Smooth Gameplay**: Non-blocking async input handling
✅ **Progress Tracking**: Score and health indicators
✅ **Visual Feedback**: Character changes, item collection

## Code Structure

```
MetroidvaniaGame/
├── Program.cs          - Entry point
├── Game.cs             - Main game loop and state management
├── Player.cs           - Player character with abilities
├── Room.cs             - Room data and generation
├── World.cs            - World management and room connections
├── GameObjects.cs      - Enemies and collectibles
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
    // Add walls, platforms, enemies, collectibles
    return room;
}
```

2. Add it to the world in `World.cs`:
```csharp
Room myRoom = RoomGenerator.CreateMyRoom();
myRoom.LeftRoom = 0; // Connect to other rooms
rooms[4] = myRoom;
```

### Adding New Abilities

1. Add boolean property to `Player.cs`:
```csharp
public bool HasWallJump { get; set; }
```

2. Add the ability logic in the Update method
3. Create a collectible for it in `CollectibleType` enum
4. Handle collection in `World.CheckCollectibles()`

### Adding New Enemy Types

1. Add to `EnemyType` enum in `GameObjects.cs`
2. Add health value in Enemy constructor
3. Create Update method (e.g., `UpdateMyEnemy()`)
4. Add sprite in `GetSprite()` method

### Improving Physics

Edit constants in `Player.cs`:
```csharp
private const float GRAVITY = 25f;        // Fall speed
private const float JUMP_FORCE = 12f;     // Jump height
private const float MOVE_SPEED = 12f;     // Walk speed
private const float DASH_SPEED = 25f;     // Dash speed
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

**Flickering:**
- The renderer updates only changed characters
- If flickering occurs, your terminal may not support `SetCursorPosition` well

## Next Steps & Ideas

Here are ways you and your friend can expand this game:

### Easy Additions
- More rooms and a larger map
- Additional enemy types
- More collectible types (coins, power-ups)
- Sound effects (using Console.Beep)
- Different colored text (using Console.ForegroundColor)

### Medium Additions
- Save/load system (file I/O)
- Projectile attacks
- Moving platforms
- Checkpoints that restore health
- Locked doors and keys
- Secret areas

### Advanced Additions
- Map system showing explored rooms
- Multiple weapon types
- Shop system with currency
- Boss patterns with phases
- Particle effects (using character animations)
- Procedurally generated rooms

## Credits

Built for a class project demonstrating:
- Object-oriented design
- Game loop architecture
- Console manipulation
- Real-time physics simulation
- State management

Have fun exploring and expanding this game! 🎮
