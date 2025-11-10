# Easy Customizations Guide

Want to personalize your game? Here are some simple changes you can make!

## 1. Change Player Character

**File:** `Player.cs`
**Line:** Around 108 in `GetSprite()` method

**Change from:**
```csharp
return '@';
```

**Change to:**
```csharp
return 'P';  // or any character you want!
```

Try: `☺`, `X`, `O`, `&`, `$`

## 2. Adjust Jump Height

**File:** `Player.cs`
**Line:** Around 24 in the constants section

**Change:**
```csharp
private const float JUMP_FORCE = 12f;  // Higher = jump higher
```

- Try `15f` for super jumps
- Try `8f` for shorter jumps
- Default: `12f`

## 3. Make Player Faster/Slower

**File:** `Player.cs`
**Line:** Around 23

**Change:**
```csharp
private const float MOVE_SPEED = 12f;  // Higher = faster
```

- Try `20f` for speedy gameplay
- Try `8f` for slower, more precise movement
- Default: `12f`

## 4. Change Enemy Appearance

**File:** `GameObjects.cs`
**Line:** Around 95 in Enemy's `GetSprite()` method

**Change from:**
```csharp
case EnemyType.Walker:
    return 'M';
```

**Change to:**
```csharp
case EnemyType.Walker:
    return 'Z';  // Zombie walker!
```

Try: `K` (knight), `G` (goblin), `S` (skeleton), `T` (troll)

## 5. Give Player More Health

**File:** `Player.cs`
**Line:** Around 42 in the constructor

**Change:**
```csharp
MaxHealth = 5;  // Change this number
```

- Try `10` for easier gameplay
- Try `3` for a challenge
- Default: `5`

## 6. Change Wall/Platform Characters

**File:** `Room.cs`
**Line:** In `RoomGenerator.CreateStartRoom()` around line 65

**Change:**
```csharp
room.SetTile(x, room.Height - 1, '█');  // Walls
room.SetTile(x, 15, '=');               // Platforms
```

Try different characters for different looks:
- Walls: `#`, `▓`, `▒`, `░`, `|`
- Platforms: `-`, `_`, `~`, `≡`

## 7. Add More Rooms

**File:** `World.cs`
**Line:** In `CreateWorld()` method around line 17

**Add after existing rooms:**
```csharp
// Room 4 - Your new room!
Room myRoom = RoomGenerator.CreateStartRoom(); // Copy an existing room
myRoom.LeftRoom = 3;  // Connect to boss room
rooms[4] = myRoom;
```

Don't forget to also set `room.RightRoom = 4` in Room 3!

## 8. Make Dash Faster/Longer

**File:** `Player.cs`

**Speed (line 25):**
```csharp
private const float DASH_SPEED = 25f;  // Higher = faster dash
```

**Duration (line 87):**
```csharp
dashTime = 0.2f;  // Higher = longer dash (try 0.5f)
```

**Cooldown (line 88):**
```csharp
dashCooldown = 1.0f;  // Lower = dash more often (try 0.5f)
```

## 9. Change Score Values

**File:** `World.cs`
**Line:** In `CheckCollectibles()` method around line 65

**Change:**
```csharp
case CollectibleType.Health:
    player.Heal(1);
    player.AddScore(10);  // Change this!
    break;
case CollectibleType.DoubleJump:
    player.HasDoubleJump = true;
    player.AddScore(100);  // Change this!
    break;
```

**Also in `UpdateEnemies()` around line 102:**
```csharp
if (enemy.Health <= 0)
{
    player.AddScore(50);  // Points for killing enemies
    toRemove.Add(enemy);
}
```

## 10. Start With Abilities Unlocked

**File:** `Player.cs`
**Line:** Around 50 in the constructor

**Change from:**
```csharp
HasDoubleJump = false;
HasDash = false;
```

**Change to:**
```csharp
HasDoubleJump = true;  // Start with double jump!
HasDash = true;        // Start with dash!
```

Great for testing or making an easier game!

## 11. Add More Enemies to a Room

**File:** `Room.cs`
**Line:** In any `RoomGenerator.CreateXXXRoom()` method

**Add:**
```csharp
// Add this line in the room creation
room.AddEnemy(new Enemy(30, 18, EnemyType.Walker));
room.AddEnemy(new Enemy(45, 15, EnemyType.Walker));
```

## 12. Change Game Speed

**File:** `Game.cs`
**Line:** Around 11

**Change:**
```csharp
private const int TARGET_FPS = 30;  // Frames per second
```

- Try `60` for smoother but faster gameplay
- Try `20` for choppier but slower gameplay
- Default: `30`

## 13. Add Text Colors

**File:** `Renderer.cs`
**Line:** In `DrawMenu()` method before any Console.WriteLine

**Add:**
```csharp
Console.ForegroundColor = ConsoleColor.Cyan;  // Title color
Console.WriteLine("║      METROIDVANIA ADVENTURE        ║");
Console.ForegroundColor = ConsoleColor.White; // Reset
```

Colors available: `Red`, `Green`, `Blue`, `Yellow`, `Cyan`, `Magenta`, `White`, `Gray`

## 14. Make Boss Harder

**File:** `GameObjects.cs`
**Line:** Around 31 in Enemy constructor

**Change:**
```csharp
case EnemyType.Boss:
    Health = 10;  // Increase this for a tougher boss!
    break;
```

Try: `20` for a real challenge!

## 15. Create a Custom Room Layout

**File:** `Room.cs`
**Add a new method:**

```csharp
public static Room CreateMyCustomRoom()
{
    Room room = new Room(60, 20);
    
    // Add floor
    for (int x = 0; x < room.Width; x++)
        room.SetTile(x, room.Height - 1, '█');
    
    // Add walls
    for (int y = 0; y < room.Height; y++)
    {
        room.SetTile(0, y, '█');
        room.SetTile(room.Width - 1, y, '█');
    }
    
    // Add your own platforms here!
    for (int x = 10; x < 20; x++)
        room.SetTile(x, 15, '=');
    
    // Add enemies and items
    room.AddEnemy(new Enemy(25, 18, EnemyType.Walker));
    room.AddCollectible(new Collectible(15, 14, CollectibleType.Health));
    
    return room;
}
```

Then add it to the world in `World.cs`!

## Testing Your Changes

After making any change:

1. Save the file
2. Run `dotnet run` in the terminal
3. Test the change in the game
4. If something breaks, undo your change and try again

## Pro Tips

- **Change one thing at a time** - easier to find problems!
- **Keep backups** - copy the file before changing it
- **Use comments** - add `// My change` so you remember what you did
- **Experiment!** - the best way to learn is by trying things

Have fun customizing your game! 🎮✨
