# Quick Start Guide - For Complete Beginners

## Step 1: Install .NET

### Windows
1. Go to https://dotnet.microsoft.com/download
2. Download ".NET 6.0 SDK" or later
3. Run the installer
4. Click "Next" through all the steps

### Mac
1. Go to https://dotnet.microsoft.com/download
2. Download ".NET 6.0 SDK" or later for macOS
3. Open the .pkg file and follow instructions

### Linux
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install dotnet-sdk-6.0
```

## Step 2: Verify Installation

Open a terminal/command prompt and type:
```bash
dotnet --version
```

You should see a version number like `6.0.100` or higher.

## Step 3: Navigate to the Game Folder

### Windows (Command Prompt)
```cmd
cd C:\Users\YourName\Downloads\MetroidvaniaGame
```

### Mac/Linux (Terminal)
```bash
cd ~/Downloads/MetroidvaniaGame
```

## Step 4: Run the Game

Simply type:
```bash
dotnet run
```

That's it! The game will compile and start automatically.

## Common Issues

**"dotnet: command not found"**
- .NET isn't installed or not in your PATH
- Try restarting your terminal after installing
- On Windows, try restarting your computer

**"Could not find project"**
- Make sure you're in the right folder
- The folder should contain `MetroidvaniaGame.csproj`
- Type `dir` (Windows) or `ls` (Mac/Linux) to see files

**Console is too small**
- Maximize your terminal window
- The game needs at least 80x25 characters
- On Windows: Right-click title bar → Properties → Layout

**Characters look weird**
- Some terminals display special characters differently
- This is normal and won't affect gameplay
- Try Windows Command Prompt for best results

## Making Changes

1. Open any `.cs` file in a text editor (Notepad, VS Code, etc.)
2. Make your changes
3. Save the file
4. Run `dotnet run` again

The game will automatically recompile with your changes!

## Need Help?

- Check the main README.md for detailed documentation
- Look at the code comments for explanations
- Try changing small values first (like jump height)
- Test often by running the game after each change

Good luck with your project! 🚀
