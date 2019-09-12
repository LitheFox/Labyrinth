using System;
using System.Threading;

namespace Labyrinth_Redux
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static bool shouldrestart = true;

        static void Main()
        {
            while (shouldrestart)
            {
                Game1 game = new Game1();
                game.Run();
                game.Dispose();
            }

        }
    }
#endif
}
