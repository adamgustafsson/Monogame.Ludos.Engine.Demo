using System;

namespace LudosEngineDemo
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new LudosEngineDemo())
                game.Run();
        }
    }
}
