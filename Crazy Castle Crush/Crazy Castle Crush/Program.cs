using System;

namespace Crazy_Castle_Crush
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CCCGame game = new CCCGame())
            {
                game.Run();
            }
        }
    }
#endif
}

