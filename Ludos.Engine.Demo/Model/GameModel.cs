namespace LudosEngineDemo.Model
{
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Managers;
    using Microsoft.Xna.Framework;

    public class GameModel
    {
        private readonly TMXManager _tmxManager;

        public GameModel(Player player, TMXManager tmxManager)
        {
            Player = player;
            _tmxManager = tmxManager;
        }
        public static bool GameIsPaused { get; set; }

        public Player Player { get; }

        public Map GetCurrentMap{ get; set; }

        public void Update(GameTime gameTime)
        {
            if (GameIsPaused)
            {
                _tmxManager.Update(gameTime);
                Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public void LoadTmxMap(string mapName)
        {
            _tmxManager.LoadMap(mapName.EndsWith(".tmx") ? mapName : mapName + ".tmx");
        }
    }
}
