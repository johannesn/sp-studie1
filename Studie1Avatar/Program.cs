using System;

namespace Studie1Avatar
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Avatar game = new Avatar())
            {
                game.Run();
            }
        }
    }
#endif
}

