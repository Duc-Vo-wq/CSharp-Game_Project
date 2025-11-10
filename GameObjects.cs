using System;

namespace MetroidvaniaGame
{
    public class Enemy
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Health { get; set; }
        public EnemyType Type { get; private set; }
        
        private float velocityX;
        private float velocityY;
        private const float GRAVITY = 25f;
        private const float MOVE_SPEED = 5f;
        private int direction = 1;
        private float actionTimer;
        
        public Enemy(float x, float y, EnemyType type)
        {
            X = x;
            Y = y;
            Type = type;
            
            switch (type)
            {
                case EnemyType.Walker:
                    Health = 1;
                    break;
                case EnemyType.Flyer:
                    Health = 2;
                    break;
                case EnemyType.Boss:
                    Health = 10;
                    break;
            }
        }
        
        public void Update(float deltaTime, Room room)
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
                    UpdateBoss(deltaTime, room);
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
        }
        
        private void UpdateBoss(float deltaTime, Room room)
        {
            // Boss slowly moves back and forth
            if (actionTimer > 2.0f)
            {
                direction *= -1;
                actionTimer = 0;
            }
            
            velocityX = MOVE_SPEED * 0.5f * direction;
            X += velocityX * deltaTime;
            
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
                default:
                    return 'E';
            }
        }
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }
    
    public enum EnemyType
    {
        Walker,
        Flyer,
        Boss
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
                case CollectibleType.DoubleJump:
                    return '^';
                case CollectibleType.Dash:
                    return '»';
                case CollectibleType.Coin:
                    return 'o';
                default:
                    return '?';
            }
        }
    }
    
    public enum CollectibleType
    {
        Health,
        DoubleJump,
        Dash,
        Coin
    }
}
