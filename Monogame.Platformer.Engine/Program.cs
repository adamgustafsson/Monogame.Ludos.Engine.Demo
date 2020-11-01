using Monogame.Platformer.Engine.Controller;
using System;

namespace Monogame.Platformer.Engine
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (MasterController game = new MasterController())
            {
                game.Run();
            }
        }
    }
}
