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
        private int offsetX;
        private int offsetY;

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

            // Calculate offsets to center the game view
            offsetX = Math.Max(0, (consoleWidth - bufferWidth) / 2);
            offsetY = Math.Max(0, (consoleHeight - bufferHeight) / 2);

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
                    if (x >= 0 && x < bufferWidth && y >= 0 && y < bufferHeight - 5)
                    {
                        SetPixel(x, y, collectible.GetSprite());
                    }
                }

                // Draw enemies
                foreach (var enemy in currentRoom.Enemies)
                {
                    int x = (int)enemy.X;
                    int y = (int)enemy.Y;
                    if (x >= 0 && x < bufferWidth && y >= 0 && y < bufferHeight - 5)
                    {
                        SetPixel(x, y, enemy.GetSprite());
                    }

                    // Draw enemy projectiles
                    foreach (var proj in enemy.Projectiles)
                    {
                        int px = (int)proj.X;
                        int py = (int)proj.Y;
                        if (px >= 0 && px < bufferWidth && py >= 0 && py < bufferHeight - 5)
                        {
                            SetPixel(px, py, proj.GetSprite());
                        }
                    }
                }

                // Draw projectiles
                foreach (var projectile in player.ActiveProjectiles)
                {
                    int projX = (int)projectile.X;
                    int projY = (int)projectile.Y;
                    if (projX >= 0 && projX < bufferWidth && projY >= 0 && projY < bufferHeight - 5)
                    {
                        SetPixel(projX, projY, projectile.GetSprite());
                    }
                }

                // Draw player
                int playerX = (int)player.X;
                int playerY = (int)player.Y;
                if (playerX >= 0 && playerX < bufferWidth && playerY >= 0 && playerY < bufferHeight - 5)
                {
                    SetPixel(playerX, playerY, player.GetSprite());
                }

                // Draw facing indicator
                if (player.IsAimingUp)
                {
                    // Show upward aim indicator
                    int aimY = playerY - 1;
                    if (aimY >= 0 && aimY < bufferHeight - 5)
                    {
                        SetPixel(playerX, aimY, '↑');
                    }
                }
                else
                {
                    // Show horizontal facing indicator
                    int facingX = playerX + player.Facing;
                    if (facingX >= 0 && facingX < bufferWidth && playerY >= 0 && playerY < bufferHeight - 5)
                    {
                        SetPixel(facingX, playerY, player.Facing > 0 ? '→' : '←');
                    }
                }

                    // Draw melee attack indicator if attacking
                    if (player.IsAttacking)
                    {
                        // Show visual indicator for full attack range (1.8 tiles)
                        int attackX1 = playerX + player.Facing;
                        int attackX2 = playerX + (player.Facing * 2);
                        int attackY = playerY;

                        // First tile (always shown)
                        if (attackX1 >= 0 && attackX1 < bufferWidth && attackY >= 0 && attackY < bufferHeight - 5)
                        {
                            SetPixel(attackX1, attackY, '*');
                        }

                        // Second tile (shows extended range)
                        if (attackX2 >= 0 && attackX2 < bufferWidth && attackY >= 0 && attackY < bufferHeight - 5)
                        {
                            SetPixel(attackX2, attackY, '·');
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
                            int screenX = x + offsetX;
                            int screenY = y + offsetY;
                            if (screenX < Console.WindowWidth && screenY < Console.WindowHeight)
                            {
                                Console.SetCursorPosition(screenX, screenY);
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
            int displayHeight = bufferHeight - 5; // Reserve space for HUD (5 lines now)

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
            int hudY = bufferHeight - 5;

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

            // Projectile Ammo
            string ammoText = $"Ammo: {player.ProjectileAmmo}";
            startX = 20;
            for (int i = 0; i < ammoText.Length; i++)
            {
                buffer[hudY + 2, startX + i] = ammoText[i];
            }

            // Abilities
            string abilitiesText = "Abilities: ";
            startX = 30;
            for (int i = 0; i < abilitiesText.Length; i++)
            {
                buffer[hudY + 1, startX + i] = abilitiesText[i];
            }

            startX += abilitiesText.Length;
            // No abilities to display currently

            // Controls reminder
            string controls = "A/D: Move | Space: Jump | W: Aim Up | F: Attack | G: Shoot | Q: Quit";
            startX = 2;
            for (int i = 0; i < controls.Length && startX + i < bufferWidth; i++)
            {
                buffer[hudY + 3, startX + i] = controls[i];
            }

            // Enemy health display (below controls)
            startX = 2;
            int enemyDisplayX = startX;
            foreach (var enemy in room.Enemies)
            {
                if (enemy.ShouldShowHealth() && enemyDisplayX < bufferWidth - 15)
                {
                    string enemyType = enemy.Type == EnemyType.Boss ? "Boss" :
                                      enemy.Type == EnemyType.Flyer ? "Flyer" : "Enemy";
                    string enemyHealth = $"{enemyType}:{enemy.Health}/{enemy.MaxHealth} ";
                    for (int i = 0; i < enemyHealth.Length && enemyDisplayX + i < bufferWidth; i++)
                    {
                        buffer[hudY + 4, enemyDisplayX + i] = enemyHealth[i];
                    }
                    enemyDisplayX += enemyHealth.Length;
                }
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
                Console.WriteLine("  A/D or Arrow Keys - Move");
                Console.SetCursorPosition(startX, startY + 11);
                Console.WriteLine("  Space - Jump");
                Console.SetCursorPosition(startX, startY + 12);
                Console.WriteLine("  W - Aim Up");
                Console.SetCursorPosition(startX, startY + 13);
                Console.WriteLine("  F - Attack | G - Shoot");
                Console.SetCursorPosition(startX, startY + 14);
                Console.WriteLine("  Q or ESC - Quit");
                Console.SetCursorPosition(startX, startY + 16);
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
        
        public void DrawVictory(Player player)
        {
            Console.Clear();

            // Get safe starting position
            int startY = Math.Max(0, (Console.WindowHeight / 2) - 5);
            int startX = Math.Max(0, (Console.WindowWidth / 2) - 20);

            // Check if console is big enough for fancy display
            if (startY + 10 > Console.WindowHeight || startX + 40 > Console.WindowWidth)
            {
                // Console too small, draw simple victory screen
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("VICTORY!");
                Console.WriteLine("\nYou defeated the boss!");
                Console.WriteLine($"\nFinal Score: {player.Score}");
                Console.WriteLine("\nPress R to Restart");
                Console.WriteLine("Press Q to Quit");
                return;
            }

            try
            {
                Console.SetCursorPosition(startX, startY);
                Console.WriteLine("╔══════════════════════════════════╗");
                Console.SetCursorPosition(startX, startY + 1);
                Console.WriteLine("║                                  ║");
                Console.SetCursorPosition(startX, startY + 2);
                Console.WriteLine("║          VICTORY!                ║");
                Console.SetCursorPosition(startX, startY + 3);
                Console.WriteLine("║                                  ║");
                Console.SetCursorPosition(startX, startY + 4);
                Console.WriteLine("╚══════════════════════════════════╝");
                Console.SetCursorPosition(startX, startY + 6);
                Console.WriteLine("  You defeated the boss!");
                Console.SetCursorPosition(startX, startY + 7);
                Console.WriteLine($"  Final Score: {player.Score}");
                Console.SetCursorPosition(startX, startY + 9);
                Console.WriteLine("  Press R to Restart");
                Console.SetCursorPosition(startX, startY + 10);
                Console.WriteLine("  Press Q to Quit");
            }
            catch
            {
                // Fallback to simple display
                Console.Clear();
                Console.WriteLine("VICTORY!");
                Console.WriteLine("You defeated the boss!");
                Console.WriteLine($"Final Score: {player.Score}");
                Console.WriteLine("\nPress R to Restart | Press Q to Quit");
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