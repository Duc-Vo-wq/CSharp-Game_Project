using System;

namespace MetroidvaniaGame
{
    public class Enemy
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public EnemyType Type { get; private set; }
        public float LastDamagedTime { get; set; }
        public System.Collections.Generic.List<EnemyProjectile> Projectiles { get; private set; }
        public System.Collections.Generic.List<TurretProjectile> TurretProjectiles { get; private set; }

        private float velocityX;
        private float velocityY;
        private const float GRAVITY = 25f;
        private const float MOVE_SPEED = 5f;
        private int direction = 1;
        private float actionTimer;
        private float projectileDropTimer;

        public Enemy(float x, float y, EnemyType type)
        {
            X = x;
            Y = y;
            Type = type;
            Projectiles = new System.Collections.Generic.List<EnemyProjectile>();
            TurretProjectiles = new System.Collections.Generic.List<TurretProjectile>();
            projectileDropTimer = 0f;

            switch (type)
            {
                case EnemyType.Walker:
                    Health = 3;
                    MaxHealth = 3;
                    break;
                case EnemyType.Flyer:
                    Health = 2;
                    MaxHealth = 2;
                    break;
                case EnemyType.Boss:
                    Health = 10;
                    MaxHealth = 10;
                    break;
                case EnemyType.Turret:
                    Health = 2;
                    MaxHealth = 2;
                    break;
            }
        }

        public void Update(float deltaTime, Room room)
        {
            Update(deltaTime, room, null);
        }

        public void Update(float deltaTime, Room room, Player? player)
        {
            actionTimer += deltaTime;

            switch (Type)
            {
                case EnemyType.Walker:
                    UpdateWalker(deltaTime, room);
                    break;
                case EnemyType.Flyer:
                    UpdateFlyer(deltaTime, room);
                    break;
                case EnemyType.Boss:
                    UpdateBoss(deltaTime, room, player);
                    break;
                case EnemyType.Turret:
                    UpdateTurret(deltaTime, room, player);
                    break;
            }
        }
        
        private void UpdateWalker(float deltaTime, Room room)
        {
            // Simple AI: Walk back and forth
            velocityX = MOVE_SPEED * direction;
            
            // Apply gravity
            velocityY += GRAVITY * deltaTime;
            
            // Move horizontally
            float newX = X + velocityX * deltaTime;
            
            // Check for walls or edges
            if (room.IsWall((int)newX, (int)Y) || !room.IsWall((int)newX, (int)Y + 1))
            {
                direction *= -1; // Turn around
            }
            else
            {
                X = newX;
            }
            
            // Move vertically
            float newY = Y + velocityY * deltaTime;
            if (room.IsWall((int)X, (int)newY))
            {
                Y = (int)Y;
                velocityY = 0;
            }
            else
            {
                Y = newY;
            }
        }
        
        private void UpdateFlyer(float deltaTime, Room room)
        {
            // Flyer moves in a sine wave pattern
            X += MOVE_SPEED * direction * deltaTime;
            Y += (float)Math.Sin(actionTimer * 3) * 0.5f;

            // Turn around at walls
            if (room.IsWall((int)X, (int)Y))
            {
                direction *= -1;
            }

            // Drop projectiles periodically
            projectileDropTimer += deltaTime;
            if (projectileDropTimer >= 2.0f) // Drop every 2 seconds
            {
                Projectiles.Add(new EnemyProjectile(X, Y, 0, 1)); // Drop downward
                projectileDropTimer = 0f;
            }

            // Update projectiles
            foreach (var proj in Projectiles)
            {
                proj.Update(deltaTime, room);
            }

            // Remove inactive projectiles
            Projectiles.RemoveAll(p => !p.IsActive);
        }
        
        private void UpdateBoss(float deltaTime, Room room, Player? player)
        {
            // Boss chases the player aggressively
            if (player != null)
            {
                // Calculate direction to player
                float dx = player.X - X;

                // Move towards player
                if (Math.Abs(dx) > 1.0f) // Only chase if not too close
                {
                    direction = dx > 0 ? 1 : -1;
                    velocityX = MOVE_SPEED * 0.8f * direction; // Faster than normal boss

                    float newX = X + velocityX * deltaTime;

                    // Only move if not hitting a wall
                    if (!room.IsWall((int)newX, (int)Y))
                    {
                        X = newX;
                    }
                }
            }

            // Keep boss on ground
            velocityY += GRAVITY * deltaTime;
            float newY = Y + velocityY * deltaTime;
            if (room.IsWall((int)X, (int)newY))
            {
                Y = (int)Y;
                velocityY = 0;
            }
            else
            {
                Y = newY;
            }
        }

        private void UpdateTurret(float deltaTime, Room room, Player? player)
        {
            // Turret is stationary, only shoots projectiles at player
            if (player != null)
            {
                // Shoot projectiles at player periodically
                projectileDropTimer += deltaTime;
                if (projectileDropTimer >= 2.5f) // Shoot every 2.5 seconds
                {
                    // Calculate direction to player
                    float dx = player.X - X;
                    float dy = player.Y - Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);

                    // Only shoot if player is in range (within 30 tiles)
                    if (distance < 30f && distance > 0)
                    {
                        // Normalize direction
                        int dirX = dx > 0 ? 1 : -1;
                        int dirY = 0; // Shoot horizontally only

                        TurretProjectiles.Add(new TurretProjectile(X, Y, dirX, dirY));
                        projectileDropTimer = 0f;
                    }
                }
            }

            // Update turret projectiles
            foreach (var proj in TurretProjectiles)
            {
                proj.Update(deltaTime, room);
            }

            // Remove inactive turret projectiles
            TurretProjectiles.RemoveAll(p => !p.IsActive);

            // Keep turret on ground (apply gravity)
            velocityY += GRAVITY * deltaTime;
            float newY = Y + velocityY * deltaTime;
            if (room.IsWall((int)X, (int)newY))
            {
                Y = (int)Y;
                velocityY = 0;
            }
            else
            {
                Y = newY;
            }
        }

        public char GetSprite()
        {
            switch (Type)
            {
                case EnemyType.Walker:
                    return 'M';
                case EnemyType.Flyer:
                    return 'F';
                case EnemyType.Boss:
                    return 'B';
                case EnemyType.Turret:
                    return 'T';
                default:
                    return 'E';
            }
        }
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
            LastDamagedTime = actionTimer;
        }

        public bool ShouldShowHealth()
        {
            // Show health for 3 seconds after being damaged
            return (actionTimer - LastDamagedTime) < 3.0f;
        }
    }
    
    public enum EnemyType
    {
        Walker,
        Flyer,
        Boss,
        Turret
    }
    
    public class Collectible
    {
        public float X { get; set; }
        public float Y { get; set; }
        public CollectibleType Type { get; private set; }
        
        public Collectible(float x, float y, CollectibleType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        
        public char GetSprite()
        {
            switch (Type)
            {
                case CollectibleType.Health:
                    return '♥';
                case CollectibleType.ProjectileAmmo:
                    return '◊';
                case CollectibleType.MaxHealthUpgrade:
                    return '+';
                default:
                    return '?';
            }
        }
    }
    
    public enum CollectibleType
    {
        Health,
        ProjectileAmmo,
        MaxHealthUpgrade
    }

    public class Projectile
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
        public bool IsActive { get; set; }
        private const float PROJECTILE_SPEED = 20f;

        public Projectile(float x, float y, int directionX, int directionY)
        {
            X = x;
            Y = y;
            DirectionX = directionX;
            DirectionY = directionY;
            VelocityX = PROJECTILE_SPEED * directionX;
            VelocityY = PROJECTILE_SPEED * directionY;
            IsActive = true;
        }

        public void Update(float deltaTime, Room room)
        {
            // Move projectile (no gravity - shoots straight)
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;

            // Deactivate if hit wall or out of bounds
            if (room.IsWall((int)X, (int)Y) || X < 0 || X >= room.Width || Y < 0 || Y >= room.Height)
            {
                IsActive = false;
            }
        }

        public char GetSprite()
        {
            if (DirectionY < 0) return '^'; // Shooting up
            if (DirectionY > 0) return 'v'; // Shooting down
            return DirectionX > 0 ? '>' : '<'; // Shooting horizontally
        }
    }

    public class EnemyProjectile
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public bool IsActive { get; set; }
        private const float PROJECTILE_SPEED = 15f;
        private const float GRAVITY = 20f;

        public EnemyProjectile(float x, float y, float velX, float velY)
        {
            X = x;
            Y = y;
            VelocityX = velX * PROJECTILE_SPEED;
            VelocityY = velY * PROJECTILE_SPEED;
            IsActive = true;
        }

        public void Update(float deltaTime, Room room)
        {
            // Apply gravity
            VelocityY += GRAVITY * deltaTime;

            // Move projectile
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;

            // Deactivate if hit wall or out of bounds
            if (room.IsWall((int)X, (int)Y) || X < 0 || X >= room.Width || Y < 0 || Y >= room.Height)
            {
                IsActive = false;
            }
        }

        public char GetSprite()
        {
            return 'v'; // Downward projectile
        }
    }

    public class TurretProjectile
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }
        public bool IsActive { get; set; }
        private const float PROJECTILE_SPEED = 18f;

        public TurretProjectile(float x, float y, int directionX, int directionY)
        {
            X = x;
            Y = y;
            DirectionX = directionX;
            DirectionY = directionY;
            VelocityX = PROJECTILE_SPEED * directionX;
            VelocityY = PROJECTILE_SPEED * directionY;
            IsActive = true;
        }

        public void Update(float deltaTime, Room room)
        {
            // Move projectile (no gravity - shoots straight like a bullet)
            X += VelocityX * deltaTime;
            Y += VelocityY * deltaTime;

            // Deactivate if hit wall or out of bounds
            if (room.IsWall((int)X, (int)Y) || X < 0 || X >= room.Width || Y < 0 || Y >= room.Height)
            {
                IsActive = false;
            }
        }

        public char GetSprite()
        {
            // Arrow points in direction of travel
            if (DirectionY < 0) return '^'; // Shooting up
            if (DirectionY > 0) return 'v'; // Shooting down
            return DirectionX > 0 ? '>' : '<'; // Shooting horizontally
        }
    }
}
