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
            
            // Platform
            for (int x = 10; x < 20; x++)
            {
                room.SetTile(x, 15, '=');
            }
            
            for (int x = 30; x < 45; x++)
            {
                room.SetTile(x, 12, '=');
            }
            
            // Add a basic enemy
            room.AddEnemy(new Enemy(25, 18, EnemyType.Walker));
            
            // Add health pickup
            room.AddCollectible(new Collectible(15, 14, CollectibleType.Health));
            
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
            
            // Add double jump powerup
            room.AddCollectible(new Collectible(30, 7, CollectibleType.DoubleJump));
            
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
            
            // Floating platforms
            for (int x = 5; x < 12; x++)
                room.SetTile(x, 16, '=');
                
            for (int x = 15; x < 22; x++)
                room.SetTile(x, 13, '=');
                
            for (int x = 25; x < 32; x++)
                room.SetTile(x, 10, '=');
                
            for (int x = 35; x < 42; x++)
                room.SetTile(x, 13, '=');
                
            for (int x = 45; x < 52; x++)
                room.SetTile(x, 16, '=');
            
            // Add enemies
            room.AddEnemy(new Enemy(18, 12, EnemyType.Walker));
            room.AddEnemy(new Enemy(38, 12, EnemyType.Walker));
            
            // Dash ability at the end
            room.AddCollectible(new Collectible(48, 15, CollectibleType.Dash));
            
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
            
            // Small platforms for dodging
            for (int x = 10; x < 15; x++)
                room.SetTile(x, 15, '=');
                
            for (int x = 45; x < 50; x++)
                room.SetTile(x, 15, '=');
            
            // Boss enemy
            room.AddEnemy(new Enemy(30, 17, EnemyType.Boss));
            
            return room;
        }
    }
}
