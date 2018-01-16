using System;

namespace Uno
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Intro();
            game.Reset();
            game.Run();
        }
    }
}
