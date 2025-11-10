# Code Architecture Guide

Understanding how all the pieces fit together!

## The Big Picture

```
┌─────────────────────────────────────────────────────────────┐
│                         PROGRAM.CS                          │
│                    (Starts everything)                      │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                          GAME.CS                            │
│                   (The Game Controller)                     │
│  • Shows menu                                               │
│  • Runs game loop                                           │
│  • Handles input                                            │
│  • Checks game over                                         │
└──────┬──────────────────┬──────────────────┬───────────────┘
       │                  │                  │
       ▼                  ▼                  ▼
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│  PLAYER.CS  │  │  WORLD.CS   │  │ RENDERER.CS │
└─────────────┘  └─────────────┘  └─────────────┘
       │                  │                  │
       ▼                  ▼                  ▼
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│ • Position  │  │ • Rooms     │  │ • Draws     │
│ • Movement  │  │ • Map       │  │   screen    │
│ • Abilities │  │ • Enemies   │  │ • HUD       │
│ • Health    │  │ • Items     │  │ • Menus     │
└─────────────┘  └──────┬──────┘  └─────────────┘
                        │
           ┌────────────┴────────────┐
           ▼                         ▼
    ┌─────────────┐          ┌──────────────┐
    │   ROOM.CS   │          │ GAMEOBJECTS  │
    └─────────────┘          │     .CS      │
           │                 └──────────────┘
           ▼                         │
    ┌─────────────┐          ┌──────┴──────┐
    │ • Tiles     │          │             │
    │ • Walls     │          ▼             ▼
    │ • Platforms │    ┌─────────┐  ┌──────────┐
    │ • Size      │    │ ENEMIES │  │  ITEMS   │
    └─────────────┘    └─────────┘  └──────────┘
```

## How Data Flows

### 1. Game Loop (60 times per second)
```
INPUT → UPDATE → RENDER
  ↓       ↓        ↓
Keyboard  Move    Draw to
          Player  Screen
          Enemies
          Physics
```

### 2. Update Cycle
```
1. Read keyboard input
   └─→ Move player left/right
   └─→ Make player jump
   └─→ Trigger dash

2. Update player physics
   └─→ Apply gravity
   └─→ Check collisions with walls
   └─→ Update position

3. Update world
   └─→ Move enemies
   └─→ Check if player touched items
   └─→ Check room transitions
   └─→ Check if player hit enemy

4. Check game state
   └─→ Is player dead? → Game Over
   └─→ Did player win? → Victory
```

## File Responsibilities

### Program.cs (10 lines)
**Job:** Just starts the game
**What it does:**
- Creates a new Game
- Calls Game.Run()
**That's it!**

### Game.cs (150 lines)
**Job:** Controls the entire game flow
**What it does:**
- Shows the menu
- Runs the main game loop
- Reads keyboard input
- Calls Update on everything
- Calls Render to draw
- Shows game over screen

**Key Methods:**
- `Run()` - Starts everything
- `GameLoop()` - The main loop
- `HandleInput()` - Reads keyboard
- `Update()` - Updates game state
- `Render()` - Draws the screen

### Player.cs (180 lines)
**Job:** Everything about the player character
**What it does:**
- Stores player position (X, Y)
- Handles movement (left, right)
- Handles jumping (including double jump)
- Handles dashing
- Applies physics (gravity, velocity)
- Tracks health and score
- Tracks which abilities are unlocked

**Key Properties:**
- `X, Y` - Where player is
- `Health` - How much health left
- `Score` - Player's score
- `HasDoubleJump, HasDash` - Abilities unlocked

**Key Methods:**
- `MoveLeft(), MoveRight()` - Movement
- `Jump()` - Jumping (and double jump)
- `Dash()` - Dash ability
- `Update()` - Physics and collision
- `TakeDamage()` - When hit by enemy

### World.cs (110 lines)
**Job:** Manages all the rooms and connections
**What it does:**
- Stores all rooms in a dictionary
- Tracks which room player is in
- Creates the game map
- Handles room transitions
- Manages all collectibles
- Manages all enemies

**Key Methods:**
- `CreateWorld()` - Builds the map
- `GetCurrentRoom()` - Returns current room
- `CheckRoomTransition()` - Player changing rooms?
- `CheckCollectibles()` - Player touch item?
- `UpdateEnemies()` - Move enemies, check hits

### Room.cs (180 lines)
**Job:** Represents one room/screen
**What it does:**
- Stores the tile layout (walls, platforms)
- Stores enemies in this room
- Stores collectibles in this room
- Checks if a position is a wall
- Provides room connections (left, right, up, down)

**Key Properties:**
- `Tiles[,]` - 2D array of characters
- `Enemies` - List of enemies
- `Collectibles` - List of items
- `LeftRoom, RightRoom` - Connections

**Key Methods:**
- `IsWall()` - Is this tile solid?
- `GetTile()` - What character is here?
- `AddEnemy()` - Add an enemy
- `AddCollectible()` - Add an item

**RoomGenerator:**
Static class with methods like:
- `CreateStartRoom()` - The first room
- `CreateAbilityRoom()` - Room with abilities
- `CreateChallengeRoom()` - Harder room
- `CreateBossRoom()` - Final boss

### GameObjects.cs (140 lines)
**Job:** Defines enemies and collectibles

**Enemy Class:**
- Position (X, Y)
- Health
- Type (Walker, Flyer, Boss)
- AI behavior for each type
- Update() method to move

**Collectible Class:**
- Position (X, Y)
- Type (Health, DoubleJump, Dash, Coin)
- GetSprite() to show on screen

### Renderer.cs (200 lines)
**Job:** Draws everything to the console
**What it does:**
- Creates a buffer (invisible screen)
- Draws room tiles to buffer
- Draws enemies to buffer
- Draws collectibles to buffer
- Draws player to buffer
- Draws HUD (health, score, abilities)
- Only updates changed characters (no flicker!)
- Shows menus and game over screen

**Key Methods:**
- `Render()` - Main rendering
- `DrawRoom()` - Draw the level
- `DrawHUD()` - Draw UI at bottom
- `DrawMenu()` - Title screen
- `DrawGameOver()` - End screen

## How Objects Interact

### When Player Moves:
```
1. Game.HandleInput() receives key press
2. Game calls Player.MoveLeft() or Player.MoveRight()
3. Player.Update() applies physics
4. Player.Update() checks Room.IsWall()
5. If no wall, update position
6. Renderer.Render() draws player at new position
```

### When Player Collects Item:
```
1. Player moves near collectible
2. Game.Update() calls World.CheckCollectibles()
3. World checks distance between player and items
4. If close enough, give player the item
5. Update player stats (health, abilities, score)
6. Remove item from room
7. Renderer shows updated health/abilities
```

### When Player Changes Room:
```
1. Player moves to edge of room
2. Game.Update() calls World.CheckRoomTransition()
3. World checks if edge has a connection
4. Change currentRoomId to new room
5. Update player position to opposite edge
6. Renderer draws new room
```

## Object Ownership

```
Game owns:
  ├─ Player (1 instance)
  ├─ World (1 instance)
  │   └─ Rooms (4 instances in dictionary)
  │       ├─ Enemies (list)
  │       └─ Collectibles (list)
  └─ Renderer (1 instance)
```

## Common Operations

### Adding a New Room:
1. Create room in `RoomGenerator` (Room.cs)
2. Add to world in `CreateWorld()` (World.cs)
3. Connect to existing rooms via properties

### Adding a New Enemy:
1. Add type to `EnemyType` enum (GameObjects.cs)
2. Add health in Enemy constructor
3. Add behavior in `Update()` method
4. Add sprite in `GetSprite()`
5. Place in room using `room.AddEnemy()`

### Adding a New Ability:
1. Add bool property to Player (Player.cs)
2. Add ability type to `CollectibleType` (GameObjects.cs)
3. Add ability logic in Player methods
4. Handle collection in `World.CheckCollectibles()`
5. Display in HUD (Renderer.cs)

## Performance Considerations

**Why the game is smooth:**

1. **Frame rate limiting** (30 FPS)
   - Prevents running too fast
   - Consistent timing

2. **Delta time** (elapsed time between frames)
   - Physics scale with frame rate
   - Smooth movement on any computer

3. **Optimized rendering**
   - Only update changed characters
   - No full screen clearing
   - Buffer system prevents flicker

4. **Efficient collision**
   - Check only nearby tiles
   - Simple distance calculations
   - No complex physics engine needed

## Memory Layout

```
Stack (small, fast):
  └─ Local variables in methods
  └─ Function parameters
  └─ Loop counters

Heap (large, slower):
  └─ Game object
  └─ Player object
  └─ World object
  └─ Room objects
  └─ Lists of enemies/items
  └─ 2D arrays for tiles
```

## Design Patterns Used

**Game Loop Pattern:**
- Continuous loop: Input → Update → Render
- Fixed time step (30 FPS)

**State Pattern:**
- GameState enum (Menu, Playing, GameOver)
- Different behavior in each state

**Component Pattern:**
- Separate classes for concerns
- Player, World, Renderer are independent

**Object Pool Pattern (implicit):**
- Rooms stored in dictionary
- Reused when transitioning

**Factory Pattern:**
- RoomGenerator creates rooms
- Encapsulates room creation logic

Now you understand the architecture! Happy coding! 🚀
