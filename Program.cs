using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetroidvaniaGame
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up console
            Console.CursorVisible = false;
            Console.Clear();
            
            Game game = new Game();
            await game.Run();
        }
    }
}
