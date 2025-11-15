using System;

namespace MetroidvaniaGame
{
    public class Player
    {
        // Position and physics
        public float X { get; set; }
        public float Y { get; set; }
        private float velocityX;
        private float velocityY;
        
        // Constants
        private const float GRAVITY = 25f;
        private const float MOVE_SPEED = 12f;
        private const float JUMP_FORCE = 12f;
        private const float DASH_SPEED = 25f;
        private const float MAX_FALL_SPEED = 20f;
        
        // State
        private bool isGrounded;
        private bool hasUsedDoubleJump;
        private float dashCooldown;
        private bool isDashing;
        private float dashTime;
        private int dashDirection;
        
        // Combat
        private float attackCooldown;
        private float attackTime;
        private const float ATTACK_COOLDOWN = 0.5f;
        private const float ATTACK_DURATION = 0.12f;
        private const float ATTACK_RANGE = 1.8f;
        private const int ATTACK_DAMAGE = 1;
        private int facing = 1; // 1 = right, -1 = left
        
        // Stats
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Score { get; set; }
        
        // Abilities (unlockable)
        public bool HasDoubleJump { get; set; }
        public bool HasDash { get; set; }
        public bool HasWallJump { get; set; }
        
        public Player(float x, float y)
        {
            X = x;
            Y = y;
            MaxHealth = 5;
            Health = MaxHealth;
            Score = 0;
            
            // Start with basic movement
            HasDoubleJump = false;
            HasDash = false;
            HasWallJump = false;
            facing = 1;
        }
        
        public void MoveLeft()
        {
            if (!isDashing)
            {
                velocityX = -MOVE_SPEED;
                facing = -1;
            }
        }
        
        public void MoveRight()
        {
            if (!isDashing)
            {
                velocityX = MOVE_SPEED;
                facing = 1;
            }
        }

        public int Facing => facing;
        public bool IsAttacking => attackTime > 0;

        public void Attack(Room room)
        {
            if (attackCooldown > 0) return;

            // Start attack animation/cooldown
            attackTime = ATTACK_DURATION;
            attackCooldown = ATTACK_COOLDOWN;

            // Deal damage to enemies in front of the player within range
            if (room == null) return;

            var toHit = new System.Collections.Generic.List<Enemy>();
            foreach (var enemy in room.Enemies)
            {
                float dx = enemy.X - X;
                float dy = enemy.Y - Y;
                // Only hit enemies roughly in front (same vertical band) and within range
                if (dx * facing <= 0) continue; // behind the player
                float distance = (float)System.Math.Sqrt(dx * dx + dy * dy);
                if (distance <= ATTACK_RANGE && System.Math.Abs(dy) < 1.5f)
                {
                    toHit.Add(enemy);
                }
            }

            foreach (var e in toHit)
            {
                e.TakeDamage(ATTACK_DAMAGE);
            }
        }
        
        public void Jump()
        {
            if (isGrounded)
            {
                velocityY = -JUMP_FORCE;
                isGrounded = false;
                hasUsedDoubleJump = false;
            }
            else if (HasDoubleJump && !hasUsedDoubleJump && !isGrounded)
            {
                velocityY = -JUMP_FORCE;
                hasUsedDoubleJump = true;
            }
        }
        
        public void Dash()
        {
            if (HasDash && dashCooldown <= 0 && !isDashing)
            {
                isDashing = true;
                dashTime = 0.2f; // Dash duration
                dashCooldown = 1.0f; // Cooldown time
                dashDirection = velocityX >= 0 ? 1 : -1;
                velocityY = 0; // Stop falling during dash
            }
        }
        
        public void Update(float deltaTime, Room room)
        {
            // Update attack timers
            if (attackCooldown > 0)
            {
                attackCooldown -= deltaTime;
                if (attackCooldown < 0) attackCooldown = 0;
            }
            if (attackTime > 0)
            {
                attackTime -= deltaTime;
                if (attackTime < 0) attackTime = 0;
            }

            // Handle dash
            if (isDashing)
            {
                dashTime -= deltaTime;
                if (dashTime <= 0)
                {
                    isDashing = false;
                    velocityX = 0;
                }
                else
                {
                    velocityX = DASH_SPEED * dashDirection;
                }
            }
            
            // Update dash cooldown
            if (dashCooldown > 0)
            {
                dashCooldown -= deltaTime;
            }
            
            // Apply gravity when not dashing
            if (!isDashing)
            {
                velocityY += GRAVITY * deltaTime;
                if (velocityY > MAX_FALL_SPEED)
                {
                    velocityY = MAX_FALL_SPEED;
                }
            }
            
            // Friction - slow down horizontal movement
            if (!isDashing && isGrounded)
            {
                velocityX *= 0.8f;
                if (Math.Abs(velocityX) < 0.5f)
                {
                    velocityX = 0;
                }
            }
            
            // Move horizontally
            float newX = X + velocityX * deltaTime;
            if (!room.IsWall((int)newX, (int)Y))
            {
                X = newX;
            }
            else
            {
                velocityX = 0;
            }
            
            // Move vertically
            float newY = Y + velocityY * deltaTime;
            isGrounded = room.IsWall((int)X, (int)(Y + 1));
            
            if (velocityY > 0) // Falling
            {
                if (room.IsWall((int)X, (int)newY))
                {
                    // Hit ground
                    Y = (int)Y;
                    velocityY = 0;
                    isGrounded = true;
                    hasUsedDoubleJump = false;
                }
                else
                {
                    Y = newY;
                }
            }
            else if (velocityY < 0) // Rising
            {
                if (room.IsWall((int)X, (int)newY))
                {
                    // Hit ceiling
                    velocityY = 0;
                    Y = (int)Y + 1;
                }
                else
                {
                    Y = newY;
                }
            }
            
            // Clamp position to room bounds
            if (X < 0) X = 0;
            if (X >= room.Width) X = room.Width - 1;
            if (Y < 0) Y = 0;
            if (Y >= room.Height) Y = room.Height - 1;
        }
        
        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }
        
        public void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
        }
        
        public void AddScore(int points)
        {
            Score += points;
        }
        
        public char GetSprite()
        {
            if (isDashing)
            {
                return dashDirection > 0 ? '→' : '←';
            }
            return '@';
        }
    }
}