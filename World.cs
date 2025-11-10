using System.Collections.Generic;

namespace MetroidvaniaGame
{
    public class World
    {
        private Dictionary<int, Room> rooms;
        private int currentRoomId;
        
        public World()
        {
            rooms = new Dictionary<int, Room>();
            CreateWorld();
            currentRoomId = 0; // Start in room 0
        }
        
        private void CreateWorld()
        {
            // Room 0 - Starting room
            Room startRoom = RoomGenerator.CreateStartRoom();
            startRoom.RightRoom = 1;
            startRoom.UpRoom = 2;
            rooms[0] = startRoom;
            
            // Room 1 - Challenge room
            Room challengeRoom = RoomGenerator.CreateChallengeRoom();
            challengeRoom.LeftRoom = 0;
            challengeRoom.RightRoom = 3;
            rooms[1] = challengeRoom;
            
            // Room 2 - Ability room (above start)
            Room abilityRoom = RoomGenerator.CreateAbilityRoom();
            abilityRoom.DownRoom = 0;
            rooms[2] = abilityRoom;
            
            // Room 3 - Boss room
            Room bossRoom = RoomGenerator.CreateBossRoom();
            bossRoom.LeftRoom = 1;
            rooms[3] = bossRoom;
        }
        
        public Room GetCurrentRoom()
        {
            return rooms[currentRoomId];
        }
        
        public void CheckRoomTransition(Player player)
        {
            Room currentRoom = GetCurrentRoom();
            
            // Check right exit
            if (player.X >= currentRoom.Width - 2 && currentRoom.RightRoom.HasValue)
            {
                currentRoomId = currentRoom.RightRoom.Value;
                player.X = 2;
            }
            // Check left exit
            else if (player.X <= 1 && currentRoom.LeftRoom.HasValue)
            {
                currentRoomId = currentRoom.LeftRoom.Value;
                player.X = currentRoom.Width - 3;
            }
            // Check up exit
            else if (player.Y <= 1 && currentRoom.UpRoom.HasValue)
            {
                currentRoomId = currentRoom.UpRoom.Value;
                player.Y = GetCurrentRoom().Height - 3;
            }
            // Check down exit
            else if (player.Y >= currentRoom.Height - 2 && currentRoom.DownRoom.HasValue)
            {
                currentRoomId = currentRoom.DownRoom.Value;
                player.Y = 2;
            }
        }
        
        public void CheckCollectibles(Player player)
        {
            Room currentRoom = GetCurrentRoom();
            List<Collectible> toRemove = new List<Collectible>();
            
            foreach (var collectible in currentRoom.Collectibles)
            {
                // Check if player is close enough to collect
                float dx = player.X - collectible.X;
                float dy = player.Y - collectible.Y;
                float distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                
                if (distance < 1.5f)
                {
                    // Collect the item
                    switch (collectible.Type)
                    {
                        case CollectibleType.Health:
                            player.Heal(1);
                            player.AddScore(10);
                            break;
                        case CollectibleType.DoubleJump:
                            player.HasDoubleJump = true;
                            player.AddScore(100);
                            break;
                        case CollectibleType.Dash:
                            player.HasDash = true;
                            player.AddScore(100);
                            break;
                        case CollectibleType.Coin:
                            player.AddScore(25);
                            break;
                    }
                    
                    toRemove.Add(collectible);
                }
            }
            
            // Remove collected items
            foreach (var item in toRemove)
            {
                currentRoom.Collectibles.Remove(item);
            }
        }
        
        public void UpdateEnemies(float deltaTime, Player player)
        {
            Room currentRoom = GetCurrentRoom();
            List<Enemy> toRemove = new List<Enemy>();
            
            foreach (var enemy in currentRoom.Enemies)
            {
                enemy.Update(deltaTime, currentRoom);
                
                // Check collision with player
                float dx = player.X - enemy.X;
                float dy = player.Y - enemy.Y;
                float distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                
                if (distance < 1.5f)
                {
                    player.TakeDamage(1);
                    // Knockback player
                    if (player.X < enemy.X)
                        player.X -= 2;
                    else
                        player.X += 2;
                }
                
                if (enemy.Health <= 0)
                {
                    player.AddScore(50);
                    toRemove.Add(enemy);
                }
            }
            
            foreach (var enemy in toRemove)
            {
                currentRoom.Enemies.Remove(enemy);
            }
        }
    }
}
