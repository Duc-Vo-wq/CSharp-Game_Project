# Troubleshooting Guide

Common issues and how to fix them!

## Installation Issues

### "dotnet: command not found"

**Problem:** .NET is not installed or not in PATH

**Solutions:**
1. Install .NET SDK from https://dotnet.microsoft.com/download
2. Restart your terminal/command prompt
3. On Windows, restart your computer
4. Verify with: `dotnet --version`

### "The SDK 'Microsoft.NET.Sdk' specified could not be found"

**Problem:** Wrong .NET version or corrupted installation

**Solutions:**
1. Uninstall .NET
2. Reinstall .NET SDK 6.0 or later
3. Make sure you get the SDK, not just the Runtime

## Compilation Issues

### "CS0246: The type or namespace name could not be found"

**Problem:** Missing using statement or wrong namespace

**Solution:**
- Check if all files are in the same folder
- Make sure `namespace MetroidvaniaGame` is at the top of each file
- Run: `dotnet clean` then `dotnet build`

### "CS0103: The name 'X' does not exist in the current context"

**Problem:** Variable or method name typo

**Solution:**
- Check spelling carefully (C# is case-sensitive!)
- Make sure the variable is declared
- Check if you're in the right scope

### "CS1061: 'X' does not contain a definition for 'Y'"

**Problem:** Trying to call a method or property that doesn't exist

**Solution:**
- Check the class definition
- Make sure the method is public
- Check spelling (case-sensitive!)

## Runtime Issues

### Game exits immediately

**Problem:** Unhandled exception or error

**Solution:**
1. Look at the error message
2. Check if all files are present
3. Try running: `dotnet run --verbosity detailed`

### Console window is too small

**Problem:** Terminal window too small for the game

**Solutions:**
- Maximize your terminal window
- On Windows: Right-click title bar → Properties → Layout
  - Set Width: 80 or more
  - Set Height: 25 or more
- On Mac: Terminal → Preferences → Profiles → Window
- Try a different terminal emulator

### Characters look weird or wrong

**Problem:** Terminal doesn't support special characters

**Solutions:**
1. This is normal on some terminals
2. Try Windows Command Prompt (best support)
3. Edit the sprite characters to use basic ASCII:
   ```csharp
   // In Player.cs
   return 'P';  // Instead of @
   
   // In GameObjects.cs
   return 'H';  // Instead of ♥
   return '*';  // Instead of special characters
   ```

### Game is too fast or too slow

**Problem:** Frame rate issues

**Solutions:**
- Edit `TARGET_FPS` in Game.cs
- For slower: `private const int TARGET_FPS = 20;`
- For faster: `private const int TARGET_FPS = 60;`
- Default is 30 FPS

### Controls don't work or are laggy

**Problem:** Input handling issues

**Solutions:**
1. Make sure the game window is focused (clicked on)
2. Try a different terminal
3. Close other programs using lots of CPU
4. Check if your keyboard layout is US (WASD might be different)

### Player falls through floor

**Problem:** Physics or collision bug

**Solutions:**
1. Check if you modified physics values too much
2. Reset constants in Player.cs to defaults:
   ```csharp
   private const float GRAVITY = 25f;
   private const float JUMP_FORCE = 12f;
   ```
3. Make sure room has a floor:
   ```csharp
   for (int x = 0; x < room.Width; x++)
       room.SetTile(x, room.Height - 1, '█');
   ```

### Player gets stuck in walls

**Problem:** Collision detection issue

**Solutions:**
1. Check player starting position
2. Make sure X and Y are valid:
   ```csharp
   player = new Player(5, 15); // Not 0, 0
   ```
3. Check room boundaries are set correctly

### Enemies don't move

**Problem:** Enemy update not being called

**Solutions:**
1. Check World.UpdateEnemies() is called in Game.Update()
2. Make sure deltaTime is being passed correctly
3. Verify enemies were added to the room:
   ```csharp
   room.AddEnemy(new Enemy(25, 18, EnemyType.Walker));
   ```

### Items don't collect

**Problem:** Collision detection range issue

**Solutions:**
1. Check World.CheckCollectibles() is called
2. Try increasing the collection distance in World.cs:
   ```csharp
   if (distance < 2.0f) // Increased from 1.5f
   ```

### Screen flickers

**Problem:** Rendering optimization not working

**Solutions:**
1. This is rare with the buffer system
2. Try a different terminal
3. Check if Console.SetCursorPosition is supported
4. Reduce TARGET_FPS to 20

### Can't change rooms

**Problem:** Room connections not set up

**Solutions:**
1. Check room connections in World.cs:
   ```csharp
   startRoom.RightRoom = 1;
   challengeRoom.LeftRoom = 0;
   ```
2. Make sure both sides are connected
3. Player must be at edge (X < 2 or X > Width - 2)

## Code Issues After Modifications

### Changed code but nothing happens

**Problem:** Not recompiling

**Solution:**
- Always run `dotnet run` after changing code
- The game recompiles automatically

### Game crashes after my change

**Problem:** Syntax error or logic bug

**Solutions:**
1. Read the error message carefully
2. Undo your last change
3. Make smaller changes one at a time
4. Check for:
   - Missing semicolons ;
   - Mismatched { braces }
   - Missing closing quotes "

### "Cannot implicitly convert type 'int' to 'float'"

**Problem:** Type mismatch

**Solution:**
- Add `f` after numbers: `25f` not `25`
- Or cast: `(float)25`

### "Index was outside the bounds of the array"

**Problem:** Array access out of range

**Solutions:**
1. Check array size before accessing
2. Common with Tiles[y, x]
3. Make sure x < Width and y < Height
4. Check loop conditions

### Player has infinite health

**Problem:** Health not decreasing or resetting

**Solutions:**
1. Check TakeDamage() is being called
2. Check collision with enemies is working
3. Make sure Health isn't being reset every frame

## Performance Issues

### High CPU usage

**Problem:** Game loop running too fast

**Solutions:**
1. The game should limit to 30 FPS automatically
2. Check the Task.Delay(1) is present in GameLoop()
3. Don't set TARGET_FPS too high

### Memory leak (RAM increasing)

**Problem:** Objects not being cleaned up

**Solutions:**
1. Make sure enemies/items are removed when collected
2. Check toRemove lists are working
3. Don't create new objects every frame

## Terminal-Specific Issues

### Windows Command Prompt
- **Best compatibility**
- If colors don't work, try: `chcp 65001` (UTF-8)

### Windows PowerShell
- Works well
- May need to run as administrator

### Mac Terminal
- Works great
- Some special characters may look different

### Linux Terminal
- Full support
- If issues, try: `export TERM=xterm-256color`

### VS Code Integrated Terminal
- Generally works
- May have character encoding issues
- Try external terminal if problems

## Getting Help

If you're still stuck:

1. **Read the error message carefully**
   - It usually tells you exactly what's wrong
   - Note the file name and line number

2. **Check your recent changes**
   - Undo the last thing you changed
   - Does it work now?

3. **Compare with original code**
   - Look at the backup
   - What's different?

4. **Search the error online**
   - Copy the error message
   - Add "C#" to your search
   - StackOverflow usually has answers

5. **Ask your friend/teacher**
   - Explain what you changed
   - Show the error message
   - Describe what you expected vs. what happened

## Prevention Tips

**To avoid issues:**

1. **Test frequently**
   - Run the game after each change
   - Catch bugs early

2. **Make backups**
   - Copy files before big changes
   - Use version control (git)

3. **Change one thing at a time**
   - Easier to find what broke
   - Know exactly what caused issues

4. **Read comments**
   - Understand what code does
   - Don't delete important parts

5. **Keep it simple**
   - Start with small changes
   - Test before making it complex

## Still Not Working?

Create a new issue with:
- **What you're trying to do**
- **What happens instead**
- **Error messages (full text)**
- **What you changed**
- **Your operating system**

Good luck! You've got this! 🚀
