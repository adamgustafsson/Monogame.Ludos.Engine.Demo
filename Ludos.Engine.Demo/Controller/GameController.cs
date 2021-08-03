namespace LudosEngineDemo
{
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Ludos.Engine.Input;
    using Ludos.Engine.Tmx;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class GameController : IGameState
    {
        private readonly TMXManager _tmxManager;
        private readonly LudosPlayer _player;
        private readonly View.GameView _gameView;
        private string _currentMap;

        public GameController(ContentManager content, GameServiceContainer services)
        {
            _tmxManager = services.GetService<TMXManager>();
            var startPos = new Vector2(100, 280);
            _player = new LudosPlayer(startPos, new Point(16, 16), services) { HorizontalAcceleration = 0.035f };
            _gameView = new View.GameView(content, services.GetService<InputManager>(), _tmxManager, _player);
            _currentMap = _tmxManager.CurrentMapName;
        }

        public bool IsActive { get; set; }

        public void Update(GameTime gameTime)
        {
            if (_currentMap != _tmxManager.CurrentMapName)
            {
                _player.ResetToStartPosition();
                _currentMap = _tmxManager.CurrentMapName;
            }

            if (!LudosGame.GameIsPaused)
            {
                _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            _gameView.Update(gameTime);
        }

        public void PostUpdate(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameView.Draw(gameTime, spriteBatch);
        }

        public void DrawDebugPanel(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameView.DrawDebugPanel(gameTime, spriteBatch);
        }
    }
}
