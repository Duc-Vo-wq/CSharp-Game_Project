using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetroidvaniaGame
{
    public class Game
    {
        private const int TARGET_FPS = 30;
        private const int FRAME_TIME_MS = 1000 / TARGET_FPS;

        private Player? player;
        private World? world;
        private Renderer renderer;
        private bool isRunning;
        private GameState state;

        // Track key states for responsive movement
        private Dictionary<ConsoleKey, long> keyPressTime = new Dictionary<ConsoleKey, long>();
        private const long MOVEMENT_KEY_THRESHOLD_MS = 150; // Shorter for movement keys to reduce tap distance
        private const long ACTION_KEY_THRESHOLD_MS = 500; // Longer for jump/aim to handle keyboard repeat delay
        
        public Game()
        {
            state = GameState.Menu;
            renderer = new Renderer();
        }

        private long GetKeyThreshold(ConsoleKey key)
        {
            // Movement keys use shorter threshold to prevent excessive tap movement
            if (key == ConsoleKey.A || key == ConsoleKey.D ||
                key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
            {
                return MOVEMENT_KEY_THRESHOLD_MS;
            }
            // Jump and aim keys use longer threshold for better hold detection
            return ACTION_KEY_THRESHOLD_MS;
        }
        
        public async Task Run()
        {
            ShowMenu();
            
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (state == GameState.Menu && key == ConsoleKey.Enter)
                    {
                        StartGame();
                        break;
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        return;
                    }
                }
                await Task.Delay(50);
            }
            
            await GameLoop();
        }
        
        private void ShowMenu()
        {
            Console.Clear();
            renderer.DrawMenu();
        }
        
        private void StartGame()
        {
            state = GameState.Playing;
            world = new World();
            player = new Player(5, 15); // Starting position
            isRunning = true;
            Console.Clear();
        }
        
        private async Task GameLoop()
        {
            Stopwatch sw = Stopwatch.StartNew();
            long lastFrameTime = 0;
            
            while (isRunning)
            {
                long currentTime = sw.ElapsedMilliseconds;
                long deltaTime = currentTime - lastFrameTime;

                if (deltaTime >= FRAME_TIME_MS)
                {
                    lastFrameTime = currentTime;

                    // Handle input
                    HandleInput();

                    // Update game state
                    Update(deltaTime / 1000.0f);

                    // Render
                    Render();
                }
                else
                {
                    // Sleep for a short time to avoid busy waiting
                    await Task.Delay(1);
                }
            }

            // Show appropriate end screen
            if (state == GameState.Victory)
            {
                ShowVictory();
            }
            else
            {
                ShowGameOver();
            }
        }
        
        private void HandleInput()
        {
            if (player == null) return;

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Read all available key events and update their press times
            while (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                var key = keyInfo.Key;

                bool isNewPress = !keyPressTime.ContainsKey(key) ||
                                 (currentTime - keyPressTime[key]) > GetKeyThreshold(key);

                keyPressTime[key] = currentTime;

                // Handle one-time actions (only on new press, not repeats)
                if (isNewPress)
                {
                    switch (key)
                    {
                        case ConsoleKey.Spacebar:
                        case ConsoleKey.UpArrow:
                            player.StartJump();
                            break;
                        case ConsoleKey.F:
                        case ConsoleKey.K:
                            if (world != null)
                                player.Attack(world.GetCurrentRoom());
                            break;
                        case ConsoleKey.G:
                        case ConsoleKey.L:
                            player.ShootProjectile();
                            break;
                        case ConsoleKey.R:
                            if (state == GameState.GameOver)
                            {
                                StartGame();
                            }
                            break;
                        case ConsoleKey.Escape:
                        case ConsoleKey.Q:
                            isRunning = false;
                            break;
                    }
                }
            }

            // Check which keys are currently "held" (seen recently)
            var expiredKeys = new List<ConsoleKey>();
            foreach (var kvp in keyPressTime)
            {
                if (currentTime - kvp.Value > GetKeyThreshold(kvp.Key))
                {
                    expiredKeys.Add(kvp.Key);
                }
            }

            // Remove expired keys
            foreach (var key in expiredKeys)
            {
                keyPressTime.Remove(key);
            }

            // Update continuous movement states based on held keys
            bool movingLeft = keyPressTime.ContainsKey(ConsoleKey.LeftArrow) || keyPressTime.ContainsKey(ConsoleKey.A);
            bool movingRight = keyPressTime.ContainsKey(ConsoleKey.RightArrow) || keyPressTime.ContainsKey(ConsoleKey.D);
            bool jumping = keyPressTime.ContainsKey(ConsoleKey.Spacebar) ||
                          keyPressTime.ContainsKey(ConsoleKey.UpArrow);
            bool aimingUp = keyPressTime.ContainsKey(ConsoleKey.W) || keyPressTime.ContainsKey(ConsoleKey.I);

            // Update player movement states
            if (movingLeft)
                player.StartMoveLeft();
            else
                player.StopMoveLeft();

            if (movingRight)
                player.StartMoveRight();
            else
                player.StopMoveRight();

            // Handle aiming up
            if (aimingUp)
                player.StartAimUp();
            else
                player.StopAimUp();

            // Handle jump release for variable jump height
            if (!jumping)
            {
                player.StopJump();
            }
        }
        
        private void Update(float deltaTime)
        {
            if (state != GameState.Playing || player == null || world == null) return;

            // Update player physics
            player.Update(deltaTime, world.GetCurrentRoom());

            // Update world (room transition cooldown)
            world.Update(deltaTime);

            // Check for room transitions
            world.CheckRoomTransition(player);

            // Check for collectibles
            world.CheckCollectibles(player);

            // Update enemies and check for boss defeat
            bool bossDefeated = world.UpdateEnemies(deltaTime, player);

            // Check if boss was defeated
            if (bossDefeated)
            {
                state = GameState.Victory;
                isRunning = false;
            }

            // Check if player died
            if (player.Health <= 0)
            {
                state = GameState.GameOver;
                isRunning = false;
            }
        }
        
        private void Render()
        {
            if (world != null && player != null)
            {
                renderer.Render(world, player, state);
            }
        }
        
        private void ShowVictory()
        {
            Console.Clear();
            if (player != null)
            {
                renderer.DrawVictory(player);
            }

            // Wait for restart or quit
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.R)
                    {
                        state = GameState.Menu;
                        ShowMenu();
                        Run().Wait();
                        return;
                    }
                    else if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                    {
                        return;
                    }
                }
            }
        }

        private void ShowGameOver()
        {
            Console.Clear();
            if (player != null)
            {
                renderer.DrawGameOver(player);
            }

            // Wait for restart or quit
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.R)
                    {
                        state = GameState.Menu;
                        ShowMenu();
                        Run().Wait();
                        return;
                    }
                    else if (key == ConsoleKey.Q || key == ConsoleKey.Escape)
                    {
                        return;
                    }
                }
            }
        }
    }
    
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}