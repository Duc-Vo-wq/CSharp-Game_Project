using System;
using System.Text;

namespace MetroidvaniaGame
{
    public class Renderer
    {
        private char[,] buffer;
        private char[,] previousBuffer;
        private int bufferWidth = 80;
        private int bufferHeight = 25;
        private int consoleWidth;
        private int consoleHeight;
        
        public Renderer()
        {
            // Get actual console size
            try
            {
                consoleWidth = Console.WindowWidth;
                consoleHeight = Console.WindowHeight;
            }
            catch
            {
                consoleWidth = 80;
                consoleHeight = 25;
            }
            
            // Use smaller of desired size or actual console size
            bufferWidth = Math.Min(80, consoleWidth);
            bufferHeight = Math.Min(25, consoleHeight);
            
            buffer = new char[bufferHeight, bufferWidth];
            previousBuffer = new char[bufferHeight, bufferWidth];
            ClearBuffer();
        }
        
        private void ClearBuffer()
        {
            for (int y = 0; y < bufferHeight; y++)
            {
                for (int x = 0; x < bufferWidth; x++)
                {
                    buffer[y, x] = ' ';
                }
            }
        }
        
        public void Render(World world, Player player, GameState state)
        {
            ClearBuffer();
            
            if (state == GameState.Playing)
            {
                Room currentRoom = world.GetCurrentRoom();
                
                // Draw room
                DrawRoom(currentRoom);
                
                // Draw collectibles
                foreach (var collectible in currentRoom.Collectibles)
                {
                    int x = (int)collectible.X;
                    int y = (int)collectible.Y;
                    if (x >= 0 && x < bufferWidth && y >= 0 && y < bufferHeight - 4)
                    {
                        SetPixel(x, y, collectible.GetSprite());
                    }
                }
                
                // Draw enemies
                foreach (var enemy in currentRoom.Enemies)
                {
                    int x = (int)enemy.X;
                    int y = (int)enemy.Y;
                    if (x >= 0 && x < bufferWidth && y >= 0 && y < bufferHeight - 4)
                    {
                        SetPixel(x, y, enemy.GetSprite());
                    }
                }
                
                // Draw player
                int playerX = (int)player.X;
                int playerY = (int)player.Y;
                if (playerX >= 0 && playerX < bufferWidth && playerY >= 0 && playerY < bufferHeight - 4)
                {
                    SetPixel(playerX, playerY, player.GetSprite());
                }

                    // Draw melee attack indicator if attacking
                    if (player.IsAttacking)
                    {
                        int attackX = playerX + player.Facing;
                        int attackY = playerY;
                        if (attackX >= 0 && attackX < bufferWidth && attackY >= 0 && attackY < bufferHeight - 4)
                        {
                            SetPixel(attackX, attackY, '*');
                        }
                    }
                
                // Draw HUD
                DrawHUD(player, currentRoom);
            }
            
            // Only update changed characters for smooth rendering
            StringBuilder output = new StringBuilder();
            for (int y = 0; y < bufferHeight; y++)
            {
                for (int x = 0; x < bufferWidth; x++)
                {
                    if (buffer[y, x] != previousBuffer[y, x])
                    {
                        // Safe cursor positioning - check bounds first
                        try
                        {
                            if (x < Console.WindowWidth && y < Console.WindowHeight)
                            {
                                Console.SetCursorPosition(x, y);
                                Console.Write(buffer[y, x]);
                            }
                        }
                        catch
                        {
                            // Ignore if console was resized
                        }
                        previousBuffer[y, x] = buffer[y, x];
                    }
                }
            }
        }
        
        private void DrawRoom(Room room)
        {
            int displayHeight = bufferHeight - 4; // Reserve space for HUD
            
            for (int y = 0; y < displayHeight && y < room.Height; y++)
            {
                for (int x = 0; x < bufferWidth && x < room.Width; x++)
                {
                    buffer[y, x] = room.GetTile(x, y);
                }
            }
        }
        
        private void SetPixel(int x, int y, char c)
        {
            if (x >= 0 && x < bufferWidth && y >= 0 && y < bufferHeight)
            {
                buffer[y, x] = c;
            }
        }
        
        private void DrawHUD(Player player, Room room)
        {
            int hudY = bufferHeight - 4;
            
            // Draw separator
            for (int x = 0; x < bufferWidth; x++)
            {
                buffer[hudY, x] = '─';
            }
            
            // Health bar
            string healthText = "Health: ";
            int startX = 2;
            for (int i = 0; i < healthText.Length; i++)
            {
                buffer[hudY + 1, startX + i] = healthText[i];
            }
            
            startX += healthText.Length;
            for (int i = 0; i < player.MaxHealth; i++)
            {
                buffer[hudY + 1, startX + i] = i < player.Health ? '♥' : '♡';
            }
            
            // Score
            string scoreText = $"Score: {player.Score}";
            startX = 2;
            for (int i = 0; i < scoreText.Length; i++)
            {
                buffer[hudY + 2, startX + i] = scoreText[i];
            }
            
            // Abilities
            string abilitiesText = "Abilities: ";
            startX = 30;
            for (int i = 0; i < abilitiesText.Length; i++)
            {
                buffer[hudY + 1, startX + i] = abilitiesText[i];
            }
            
            startX += abilitiesText.Length;
            int abilityIndex = 0;
            
            if (player.HasDoubleJump)
            {
                string ability = "[DoubleJump] ";
                for (int i = 0; i < ability.Length; i++)
                {
                    buffer[hudY + 1, startX + abilityIndex + i] = ability[i];
                }
                abilityIndex += ability.Length;
            }
            
            if (player.HasDash)
            {
                string ability = "[Dash] ";
                for (int i = 0; i < ability.Length; i++)
                {
                    buffer[hudY + 1, startX + abilityIndex + i] = ability[i];
                }
                abilityIndex += ability.Length;
            }
            
            // Controls reminder
            string controls = "WASD/Arrows: Move | Space: Jump | Shift: Dash | F: Attack | Q: Quit";
            startX = 2;
            for (int i = 0; i < controls.Length && startX + i < bufferWidth; i++)
            {
                buffer[hudY + 3, startX + i] = controls[i];
            }
        }
        
        public void DrawMenu()
        {
            Console.Clear();
            
            // Get safe starting position based on console size
            int startY = Math.Max(0, (Console.WindowHeight / 2) - 10);
            int startX = Math.Max(0, (Console.WindowWidth / 2) - 20);
            
            // Make sure we don't draw outside bounds
            if (startY + 20 > Console.WindowHeight || startX + 40 > Console.WindowWidth)
            {
                // Console too small, draw simple menu
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("METROIDVANIA ADVENTURE");
                Console.WriteLine("\nExplore and unlock abilities!");
                Console.WriteLine("\nPress ENTER to start!");
                Console.WriteLine("Press ESC to quit");
                return;
            }
            
            try
            {
                Console.SetCursorPosition(startX, startY);
                Console.WriteLine("╔════════════════════════════════════╗");
                Console.SetCursorPosition(startX, startY + 1);
                Console.WriteLine("║                                    ║");
                Console.SetCursorPosition(startX, startY + 2);
                Console.WriteLine("║      METROIDVANIA ADVENTURE        ║");
                Console.SetCursorPosition(startX, startY + 3);
                Console.WriteLine("║                                    ║");
                Console.SetCursorPosition(startX, startY + 4);
                Console.WriteLine("╚════════════════════════════════════╝");
                Console.SetCursorPosition(startX, startY + 6);
                Console.WriteLine("Explore a mysterious facility and");
                Console.SetCursorPosition(startX, startY + 7);
                Console.WriteLine("unlock new abilities to reach new areas!");
                Console.SetCursorPosition(startX, startY + 9);
                Console.WriteLine("CONTROLS:");
                Console.SetCursorPosition(startX, startY + 10);
                Console.WriteLine("  WASD or Arrow Keys - Move");
                Console.SetCursorPosition(startX, startY + 11);
                Console.WriteLine("  Space or W - Jump");
                Console.SetCursorPosition(startX, startY + 12);
                Console.WriteLine("  Shift - Dash (when unlocked)");
                Console.SetCursorPosition(startX, startY + 13);
                Console.WriteLine("  Q or ESC - Quit");
                Console.SetCursorPosition(startX, startY + 15);
                Console.WriteLine("Press ENTER to start!");
            }
            catch
            {
                // If any drawing fails, show simple menu
                Console.Clear();
                Console.WriteLine("METROIDVANIA ADVENTURE\n");
                Console.WriteLine("Press ENTER to start!");
            }
        }
        
        public void DrawGameOver(Player player)
        {
            Console.Clear();
            
            // Get safe starting position
            int startY = Math.Max(0, (Console.WindowHeight / 2) - 5);
            int startX = Math.Max(0, (Console.WindowWidth / 2) - 15);
            
            // Check if console is big enough for fancy display
            if (startY + 10 > Console.WindowHeight || startX + 30 > Console.WindowWidth)
            {
                // Console too small, draw simple game over
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("GAME OVER!");
                Console.WriteLine($"\nFinal Score: {player.Score}");
                Console.WriteLine("\nPress R to Restart");
                Console.WriteLine("Press Q to Quit");
                return;
            }
            
            try
            {
                Console.SetCursorPosition(startX, startY);
                Console.WriteLine("╔════════════════════════╗");
                Console.SetCursorPosition(startX, startY + 1);
                Console.WriteLine("║                        ║");
                Console.SetCursorPosition(startX, startY + 2);
                Console.WriteLine("║      GAME OVER!        ║");
                Console.SetCursorPosition(startX, startY + 3);
                Console.WriteLine("║                        ║");
                Console.SetCursorPosition(startX, startY + 4);
                Console.WriteLine("╚════════════════════════╝");
                Console.SetCursorPosition(startX, startY + 6);
                Console.WriteLine($"Final Score: {player.Score}");
                Console.SetCursorPosition(startX, startY + 8);
                Console.WriteLine("Press R to Restart");
                Console.SetCursorPosition(startX, startY + 9);
                Console.WriteLine("Press Q to Quit");
            }
            catch
            {
                // Fallback to simple display
                Console.Clear();
                Console.WriteLine("GAME OVER!");
                Console.WriteLine($"Final Score: {player.Score}");
                Console.WriteLine("\nPress R to Restart | Press Q to Quit");
            }
        }
    }
}