using System.Collections.Generic;

namespace MetroidvaniaGame
{
    public class Room
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public char[,] Tiles { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Collectible> Collectibles { get; private set; }
        
        // Room connections (which room ID to go to)
        public int? LeftRoom { get; set; }
        public int? RightRoom { get; set; }
        public int? UpRoom { get; set; }
        public int? DownRoom { get; set; }
        
        public Room(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new char[height, width];
            Enemies = new List<Enemy>();
            Collectibles = new List<Collectible>();
            
            // Initialize with empty space
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Tiles[y, x] = ' ';
                }
            }
        }
        
        public void SetTile(int x, int y, char tile)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                Tiles[y, x] = tile;
            }
        }
        
        public char GetTile(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                return Tiles[y, x];
            }
            return ' ';
        }
        
        public bool IsWall(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return true;
                
            char tile = Tiles[y, x];
            return tile == '#' || tile == '█' || tile == '=';
        }
        
        public bool IsPlatform(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;
                
            return Tiles[y, x] == '=';
        }
        
        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }
        
        public void AddCollectible(Collectible collectible)
        {
            Collectibles.Add(collectible);
        }
    }
    
    // Helper class for room generation
    public static class RoomGenerator
    {
        public static Room CreateStartRoom()
        {
            Room room = new Room(60, 20);

            // Floor
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, room.Height - 1, '█');
            }

            // Walls (with gaps for room transitions)
            for (int y = 0; y < room.Height; y++)
            {
                // Left wall - gap for Treasure room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(0, y, '█');

                // Right wall - gap for Challenge room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(room.Width - 1, y, '█');
            }

            // Ceiling
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, 0, '█');
            }
            
            // Platforms - closer together for easier parkour
            for (int x = 10; x < 20; x++)
            {
                room.SetTile(x, 15, '=');
            }

            for (int x = 22; x < 37; x++)
            {
                room.SetTile(x, 10, '=');
            }
            
            // Add a basic enemy
            room.AddEnemy(new Enemy(25, 18, EnemyType.Walker));

            // Add health pickups
            room.AddCollectible(new Collectible(15, 14, CollectibleType.Health));
            room.AddCollectible(new Collectible(35, 18, CollectibleType.Health));

            // Add projectile ammo pickup (adjusted for new platform position)
            room.AddCollectible(new Collectible(29, 9, CollectibleType.ProjectileAmmo));

            return room;
        }

        public static Room CreateAbilityRoom()
        {
            Room room = new Room(60, 20);
            
            // Floor
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, room.Height - 1, '█');
            }
            
            // Walls
            for (int y = 0; y < room.Height; y++)
            {
                room.SetTile(0, y, '█');
                room.SetTile(room.Width - 1, y, '█');
            }
            
            // Ceiling
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, 0, '█');
            }
            
            // High platform requiring double jump
            for (int x = 25; x < 35; x++)
            {
                room.SetTile(x, 8, '=');
            }
            
            // Stairs
            for (int i = 0; i < 5; i++)
            {
                for (int x = 10 + i * 3; x < 13 + i * 3; x++)
                {
                    room.SetTile(x, 18 - i, '=');
                }
            }

            // Add projectile ammo pickups
            room.AddCollectible(new Collectible(15, 17, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(45, 18, CollectibleType.ProjectileAmmo));

            // Add health pickups
            room.AddCollectible(new Collectible(30, 7, CollectibleType.Health));
            room.AddCollectible(new Collectible(12, 17, CollectibleType.Health));

            return room;
        }
        
        public static Room CreateChallengeRoom()
        {
            Room room = new Room(60, 20);

            // Floor
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, room.Height - 1, '█');
            }

            // Walls (with gaps for room transitions)
            for (int y = 0; y < room.Height; y++)
            {
                // Left wall - gap for Start room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(0, y, '█');

                // Right wall - gap for Boss room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(room.Width - 1, y, '█');
            }

            // Ceiling (with gap for upward exit on right side)
            for (int x = 0; x < room.Width; x++)
            {
                // Create exit gap at the top right (x=50-58) for going up to Flyer room
                if (x < 50 || x >= 58)
                {
                    room.SetTile(x, 0, '█');
                }
            }
            
            // Floating platforms (horizontal parkour)
            for (int x = 5; x < 12; x++)
                room.SetTile(x, 16, '=');

            for (int x = 15; x < 22; x++)
                room.SetTile(x, 13, '=');

            for (int x = 25; x < 32; x++)
                room.SetTile(x, 10, '=');

            for (int x = 35; x < 42; x++)
                room.SetTile(x, 13, '=');

            // Ascending platforms to reach the top (right side)
            for (int x = 45; x < 52; x++)
                room.SetTile(x, 10, '=');

            for (int x = 50; x < 57; x++)
                room.SetTile(x, 7, '=');

            // Final platform near ceiling for exit
            for (int x = 45; x < 52; x++)
                room.SetTile(x, 4, '=');
            
            // Add enemies
            room.AddEnemy(new Enemy(18, 12, EnemyType.Walker));
            room.AddEnemy(new Enemy(38, 12, EnemyType.Walker));

            // Add health pickups
            room.AddCollectible(new Collectible(18, 12, CollectibleType.Health));
            room.AddCollectible(new Collectible(53, 6, CollectibleType.Health));

            // Add projectile ammo pickups
            room.AddCollectible(new Collectible(8, 15, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(28, 9, CollectibleType.ProjectileAmmo));

            return room;
        }
        
        public static Room CreateBossRoom()
        {
            Room room = new Room(60, 20);
            
            // Floor
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, room.Height - 1, '█');
            }
            
            // Walls (with gap for room transition)
            for (int y = 0; y < room.Height; y++)
            {
                // Left wall - gap for Challenge room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(0, y, '█');

                // Right wall - solid (no connection)
                room.SetTile(room.Width - 1, y, '█');
            }

            // Ceiling
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, 0, '█');
            }

            // Small platforms for dodging
            for (int x = 10; x < 15; x++)
                room.SetTile(x, 15, '=');
                
            for (int x = 45; x < 50; x++)
                room.SetTile(x, 15, '=');
            
            // Boss enemy
            room.AddEnemy(new Enemy(30, 17, EnemyType.Boss));

            // Add Flyer enemies to make boss fight harder
            room.AddEnemy(new Enemy(15, 8, EnemyType.Flyer));
            room.AddEnemy(new Enemy(45, 10, EnemyType.Flyer));

            // Add health pickups (helpful for boss fight)
            room.AddCollectible(new Collectible(5, 18, CollectibleType.Health));
            room.AddCollectible(new Collectible(55, 18, CollectibleType.Health));

            // Add projectile ammo pickups (helpful for boss fight)
            room.AddCollectible(new Collectible(12, 14, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(47, 14, CollectibleType.ProjectileAmmo));

            return room;
        }

        public static Room CreateFlyerRoom()
        {
            Room room = new Room(60, 20);

            // Floor (with gap for downward exit matching Challenge room's ceiling gap)
            for (int x = 0; x < room.Width; x++)
            {
                if (x < 50 || x >= 58)
                {
                    room.SetTile(x, room.Height - 1, '█');
                }
            }

            // Walls (with gap for room transition)
            for (int y = 0; y < room.Height; y++)
            {
                // Left wall - solid (no connection)
                room.SetTile(0, y, '█');

                // Right wall - gap for Arena room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(room.Width - 1, y, '█');
            }

            // Ceiling
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, 0, '█');
            }

            // Landing platform near entry point (right side, upper area)
            for (int x = 48; x < 60; x++)
                room.SetTile(x, 17, '=');

            // Left side platform
            for (int x = 0; x < 12; x++)
                room.SetTile(x, 17, '=');

            // Center platforms at different heights
            for (int x = 15; x < 25; x++)
                room.SetTile(x, 14, '=');

            for (int x = 28; x < 38; x++)
                room.SetTile(x, 11, '=');

            // Add Flyer enemies at different heights
            room.AddEnemy(new Enemy(20, 8, EnemyType.Flyer));
            room.AddEnemy(new Enemy(33, 6, EnemyType.Flyer));
            room.AddEnemy(new Enemy(8, 10, EnemyType.Flyer));

            // Add projectile ammo on platforms for fighting Flyers
            room.AddCollectible(new Collectible(5, 16, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(20, 13, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(33, 10, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(52, 16, CollectibleType.ProjectileAmmo));

            // Add health pickups
            room.AddCollectible(new Collectible(18, 13, CollectibleType.Health));
            room.AddCollectible(new Collectible(33, 10, CollectibleType.Health));

            return room;
        }

        public static Room CreateTreasureRoom()
        {
            Room room = new Room(60, 20);

            // Floor
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, room.Height - 1, '█');
            }

            // Walls (with gap for room transition)
            for (int y = 0; y < room.Height; y++)
            {
                // Left wall - solid (no connection)
                room.SetTile(0, y, '█');

                // Right wall - gap for Start room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(room.Width - 1, y, '█');
            }

            // Ceiling
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, 0, '█');
            }

            // Create a treasure pedestal structure in center
            for (int x = 27; x < 33; x++)
                room.SetTile(x, 15, '█');
            for (int x = 28; x < 32; x++)
                room.SetTile(x, 14, '█');
            for (int x = 29; x < 31; x++)
                room.SetTile(x, 13, '█');

            // Side platforms for parkour (lowered by 3 units)
            for (int x = 10; x < 18; x++)
                room.SetTile(x, 15, '=');

            for (int x = 42; x < 50; x++)
                room.SetTile(x, 15, '=');

            for (int x = 5; x < 12; x++)
                room.SetTile(x, 9, '=');

            for (int x = 48; x < 55; x++)
                room.SetTile(x, 9, '=');

            // Lots of collectibles (treasure room!)
            room.AddCollectible(new Collectible(30, 12, CollectibleType.Health));
            room.AddCollectible(new Collectible(14, 14, CollectibleType.Health));
            room.AddCollectible(new Collectible(46, 14, CollectibleType.Health));
            room.AddCollectible(new Collectible(8, 8, CollectibleType.Health));
            room.AddCollectible(new Collectible(51, 8, CollectibleType.Health));

            room.AddCollectible(new Collectible(25, 18, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(35, 18, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(10, 18, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(50, 18, CollectibleType.ProjectileAmmo));

            return room;
        }

        public static Room CreateArenaRoom()
        {
            Room room = new Room(60, 20);

            // Floor
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, room.Height - 1, '█');
            }

            // Walls (with gap for room transition)
            for (int y = 0; y < room.Height; y++)
            {
                // Left wall - gap for Flyer room (ground level y=16-18)
                if (y < 16 || y > 18)
                    room.SetTile(0, y, '█');

                // Right wall - solid (no connection)
                room.SetTile(room.Width - 1, y, '█');
            }

            // Ceiling
            for (int x = 0; x < room.Width; x++)
            {
                room.SetTile(x, 0, '█');
            }

            // Central raised platform (lowered by 3 units)
            for (int x = 25; x < 35; x++)
                room.SetTile(x, 16, '=');

            // Corner platforms (lowered by 3 units)
            for (int x = 5; x < 13; x++)
                room.SetTile(x, 13, '=');

            for (int x = 47; x < 55; x++)
                room.SetTile(x, 13, '=');

            // Add multiple enemies (arena combat!)
            room.AddEnemy(new Enemy(10, 18, EnemyType.Walker));
            room.AddEnemy(new Enemy(50, 18, EnemyType.Walker));
            room.AddEnemy(new Enemy(30, 15, EnemyType.Walker));
            room.AddEnemy(new Enemy(15, 5, EnemyType.Flyer));
            room.AddEnemy(new Enemy(45, 7, EnemyType.Flyer));

            // Health and ammo for the fight
            room.AddCollectible(new Collectible(8, 12, CollectibleType.Health));
            room.AddCollectible(new Collectible(51, 12, CollectibleType.Health));
            room.AddCollectible(new Collectible(30, 15, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(20, 18, CollectibleType.ProjectileAmmo));
            room.AddCollectible(new Collectible(40, 18, CollectibleType.ProjectileAmmo));

            return room;
        }
    }
}