using System.Collections.Generic;

namespace MetroidvaniaGame
{
    public class World
    {
        private Dictionary<int, Room> rooms;
        private int currentRoomId;
        private float roomTransitionCooldown;

        public World()
        {
            rooms = new Dictionary<int, Room>();
            CreateWorld();
            currentRoomId = 0; // Start in room 0
            roomTransitionCooldown = 0f;
        }
        
        private void CreateWorld()
        {
            // Room 0 - Starting room
            Room startRoom = RoomGenerator.CreateStartRoom();
            startRoom.RightRoom = 1;
            startRoom.LeftRoom = 5; // Treasure room to the left
            rooms[0] = startRoom;

            // Room 1 - Challenge room
            Room challengeRoom = RoomGenerator.CreateChallengeRoom();
            challengeRoom.LeftRoom = 0;
            challengeRoom.RightRoom = 7; // Connect to Turret room
            challengeRoom.UpRoom = 4; // Connect to Flyer room above
            rooms[1] = challengeRoom;

            // Room 3 - Boss room
            Room bossRoom = RoomGenerator.CreateBossRoom();
            bossRoom.LeftRoom = 7; // Connect to Turret room
            rooms[3] = bossRoom;

            // Room 4 - Flyer room (above challenge)
            Room flyerRoom = RoomGenerator.CreateFlyerRoom();
            flyerRoom.DownRoom = 1;
            flyerRoom.RightRoom = 6;
            rooms[4] = flyerRoom;

            // Room 5 - Treasure room (left of start room)
            Room treasureRoom = RoomGenerator.CreateTreasureRoom();
            treasureRoom.RightRoom = 0;
            rooms[5] = treasureRoom;

            // Room 6 - Arena room (right of flyer room)
            Room arenaRoom = RoomGenerator.CreateArenaRoom();
            arenaRoom.LeftRoom = 4;
            rooms[6] = arenaRoom;

            // Room 7 - Turret room (between challenge and boss)
            Room turretRoom = RoomGenerator.CreateTurretRoom();
            turretRoom.LeftRoom = 1;
            turretRoom.RightRoom = 3;
            rooms[7] = turretRoom;
        }
        
        public Room GetCurrentRoom()
        {
            return rooms[currentRoomId];
        }
        
        public void CheckRoomTransition(Player player)
        {
            // Don't allow transitions if cooldown is active
            if (roomTransitionCooldown > 0f)
                return;

            Room currentRoom = GetCurrentRoom();

            // Check right exit
            if (player.X >= currentRoom.Width - 2 && currentRoom.RightRoom.HasValue)
            {
                currentRoomId = currentRoom.RightRoom.Value;
                player.X = 2;
                roomTransitionCooldown = 0.5f; // Half second cooldown
            }
            // Check left exit
            else if (player.X <= 1 && currentRoom.LeftRoom.HasValue)
            {
                currentRoomId = currentRoom.LeftRoom.Value;
                player.X = currentRoom.Width - 3;
                roomTransitionCooldown = 0.5f;
            }
            // Check up exit (only in ceiling gap area for Challenge -> Flyer transition)
            else if (player.Y <= 1 && currentRoom.UpRoom.HasValue)
            {
                // Check if we're in a gap area (x=50-58 for Challenge room ceiling)
                bool inGapArea = (currentRoomId == 1 && player.X >= 50 && player.X < 58) ||
                                 (currentRoomId != 1); // Other rooms can transition anywhere

                if (inGapArea)
                {
                    currentRoomId = currentRoom.UpRoom.Value;
                    player.Y = GetCurrentRoom().Height - 3;
                    roomTransitionCooldown = 0.5f;
                }
            }
            // Check down exit (only in floor gap area for Flyer -> Challenge transition)
            else if (player.Y >= currentRoom.Height - 2 && currentRoom.DownRoom.HasValue)
            {
                // Check if we're in a gap area (x=50-58 for Flyer room floor gap)
                bool inGapArea = (currentRoomId == 4 && player.X >= 50 && player.X < 58) ||
                                 (currentRoomId != 4); // Other rooms can transition anywhere

                if (inGapArea)
                {
                    currentRoomId = currentRoom.DownRoom.Value;
                    player.Y = 2;
                    roomTransitionCooldown = 0.5f;
                }
            }
        }

        public void Update(float deltaTime)
        {
            // Update room transition cooldown
            if (roomTransitionCooldown > 0f)
            {
                roomTransitionCooldown -= deltaTime;
                if (roomTransitionCooldown < 0f)
                    roomTransitionCooldown = 0f;
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
                        case CollectibleType.ProjectileAmmo:
                            player.AddProjectileAmmo(5);  // Give 5 projectiles per pickup
                            player.AddScore(30);
                            break;
                        case CollectibleType.MaxHealthUpgrade:
                            player.IncreaseMaxHealth(1);  // Increase max health by 1
                            player.Heal(1);  // Also heal by 1
                            player.AddScore(100);
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
        
        public bool UpdateEnemies(float deltaTime, Player player)
        {
            Room currentRoom = GetCurrentRoom();
            List<Enemy> toRemove = new List<Enemy>();
            bool bossDefeated = false;

            foreach (var enemy in currentRoom.Enemies)
            {
                enemy.Update(deltaTime, currentRoom, player);

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

                // Check collision with enemy projectiles
                foreach (var enemyProj in enemy.Projectiles)
                {
                    if (!enemyProj.IsActive) continue;

                    float pdx = player.X - enemyProj.X;
                    float pdy = player.Y - enemyProj.Y;
                    float pDistance = (float)System.Math.Sqrt(pdx * pdx + pdy * pdy);

                    if (pDistance < 1.0f)
                    {
                        player.TakeDamage(1);
                        enemyProj.IsActive = false;
                    }
                }

                // Check collision with turret projectiles
                foreach (var turretProj in enemy.TurretProjectiles)
                {
                    if (!turretProj.IsActive) continue;

                    float pdx = player.X - turretProj.X;
                    float pdy = player.Y - turretProj.Y;
                    float pDistance = (float)System.Math.Sqrt(pdx * pdx + pdy * pdy);

                    if (pDistance < 1.0f)
                    {
                        player.TakeDamage(1);
                        turretProj.IsActive = false;
                    }
                }

                // Check collision with projectiles
                foreach (var projectile in player.ActiveProjectiles)
                {
                    if (!projectile.IsActive) continue;

                    float pdx = projectile.X - enemy.X;
                    float pdy = projectile.Y - enemy.Y;
                    float pDistance = (float)System.Math.Sqrt(pdx * pdx + pdy * pdy);

                    if (pDistance < 1.5f)
                    {
                        enemy.TakeDamage(1);
                        projectile.IsActive = false; // Deactivate projectile on hit
                    }
                }

                if (enemy.Health <= 0)
                {
                    // Check if this is the boss
                    if (enemy.Type == EnemyType.Boss)
                    {
                        bossDefeated = true;
                        player.AddScore(500); // Big score bonus for boss
                    }
                    else
                    {
                        player.AddScore(50);
                    }
                    toRemove.Add(enemy);
                }
            }

            foreach (var enemy in toRemove)
            {
                currentRoom.Enemies.Remove(enemy);
            }

            return bossDefeated;
        }
    }
}
