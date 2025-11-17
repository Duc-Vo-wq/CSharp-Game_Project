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
        private const float JUMP_FORCE = 18f;  // Increased for higher jumps to reach platforms
        private const float JUMP_RELEASE_MULTIPLIER = .5f;  // For variable jump height
        private const float MAX_FALL_SPEED = 20f;
        private const float AIR_FRICTION = 0.95f;  // Less friction in air
        private const float GROUND_FRICTION = .3f;  // More friction on ground when not moving

        // State
        private bool isGrounded;
        private bool isMovingLeft;
        private bool isMovingRight;
        private bool isJumping;  // Track if jump key is held
        
        // Combat
        private float attackCooldown;
        private float attackTime;
        private const float ATTACK_COOLDOWN = 0.5f;
        private const float ATTACK_DURATION = 0.12f;
        private const float ATTACK_RANGE = 3.5f;  // Increased from 1.8f for better reach
        private const int ATTACK_DAMAGE = 2;  // Increased from 1 for faster defeats
        private int facing = 1; // 1 = right, -1 = left
        private int aimDirection = 0; // 0 = horizontal, 1 = up, -1 = down
        public bool IsAimingUp { get; private set; }
        
        // Stats
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Score { get; set; }
        public int ProjectileAmmo { get; set; }
        public System.Collections.Generic.List<Projectile> ActiveProjectiles { get; private set; }
        
        // Abilities (unlockable)
        public bool HasWallJump { get; set; }

        public Player(float x, float y)
        {
            X = x;
            Y = y;
            MaxHealth = 5;
            Health = MaxHealth;
            Score = 0;
            ProjectileAmmo = 0;
            ActiveProjectiles = new System.Collections.Generic.List<Projectile>();

            // Start with basic movement
            HasWallJump = false;
            facing = 1;
        }
        
        public void StartMoveLeft()
        {
            isMovingLeft = true;
            facing = -1;
        }

        public void StopMoveLeft()
        {
            isMovingLeft = false;
        }

        public void StartMoveRight()
        {
            isMovingRight = true;
            facing = 1;
        }

        public void StopMoveRight()
        {
            isMovingRight = false;
        }

        public int Facing => facing;
        public bool IsAttacking => attackTime > 0;
        public int AimDirection => aimDirection;

        public void StartAimUp()
        {
            aimDirection = 1;
            IsAimingUp = true;
        }

        public void StopAimUp()
        {
            aimDirection = 0;
            IsAimingUp = false;
        }

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
                float distance = (float)System.Math.Sqrt(dx * dx + dy * dy);

                // Check if enemy is within range
                if (distance > ATTACK_RANGE) continue;

                // Check if enemy is roughly in front (more forgiving directional check)
                // Allow hitting enemies at same X position or in front
                if (facing > 0 && dx < -0.5f) continue; // facing right, enemy is behind
                if (facing < 0 && dx > 0.5f) continue;  // facing left, enemy is behind

                // Check vertical alignment (more forgiving)
                if (System.Math.Abs(dy) < 2.0f)
                {
                    toHit.Add(enemy);
                }
            }

            foreach (var e in toHit)
            {
                e.TakeDamage(ATTACK_DAMAGE);
            }
        }

        public void ShootProjectile()
        {
            if (ProjectileAmmo > 0)
            {
                Projectile projectile;
                if (aimDirection == 1) // Shooting up
                {
                    projectile = new Projectile(X, Y, 0, -1);
                }
                else // Shooting horizontally
                {
                    float projectileX = X + facing;
                    projectile = new Projectile(projectileX, Y, facing, 0);
                }
                ActiveProjectiles.Add(projectile);
                ProjectileAmmo--;
            }
        }

        public void AddProjectileAmmo(int amount)
        {
            ProjectileAmmo += amount;
        }

        public void StartJump()
        {
            isJumping = true;
            if (isGrounded)
            {
                velocityY = -JUMP_FORCE;
                isGrounded = false;
            }
        }

        public void StopJump()
        {
            isJumping = false;
            // Variable jump height: cut jump short when button released
            if (velocityY < 0)
            {
                velocityY *= JUMP_RELEASE_MULTIPLIER;
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

            // Apply gravity
            velocityY += GRAVITY * deltaTime;
            if (velocityY > MAX_FALL_SPEED)
            {
                velocityY = MAX_FALL_SPEED;
            }

            // Handle horizontal movement based on input state
            bool isMoving = isMovingLeft || isMovingRight;

            if (isMovingLeft && !isMovingRight)
            {
                velocityX = -MOVE_SPEED;
            }
            else if (isMovingRight && !isMovingLeft)
            {
                velocityX = MOVE_SPEED;
            }
            else
            {
                // No input or both pressed - apply friction
                if (isGrounded)
                {
                    velocityX *= GROUND_FRICTION;
                }
                else
                {
                    velocityX *= AIR_FRICTION;
                }

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

            // Update all active projectiles
            foreach (var projectile in ActiveProjectiles)
            {
                projectile.Update(deltaTime, room);
            }

            // Remove inactive projectiles
            ActiveProjectiles.RemoveAll(p => !p.IsActive);
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
            if (IsAttacking)
            {
                return facing > 0 ? '⚔' : '⚔';
            }
            return '@';
        }
    }
}