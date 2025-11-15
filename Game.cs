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
        
        public Game()
        {
            state = GameState.Menu;
            renderer = new Renderer();
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
            
            ShowGameOver();
        }
        
        private void HandleInput()
        {
            if (player == null) return;
            
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.A:
                        player.MoveLeft();
                        break;
                    case ConsoleKey.RightArrow:
                    case ConsoleKey.D:
                        player.MoveRight();
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        player.Jump();
                        break;
                    case ConsoleKey.F:
                    case ConsoleKey.K:
                        // Short range melee attack
                        if (world != null)
                            player.Attack(world.GetCurrentRoom());
                        break;
                    case ConsoleKey.E:
                        player.Dash();
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
        
        private void Update(float deltaTime)
        {
            if (state != GameState.Playing || player == null || world == null) return;
            
            // Update player physics
            player.Update(deltaTime, world.GetCurrentRoom());
            
            // Check for room transitions
            world.CheckRoomTransition(player);
            
            // Check for collectibles
            world.CheckCollectibles(player);
            
            // Update enemies
            world.UpdateEnemies(deltaTime, player);
            
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
        GameOver
    }
}