namespace LudosEngineDemo
{
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Ludos.Engine.Level;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class GameController : IGameState
    {
        private readonly LevelManager _tmxManager;
        private readonly LudosPlayer _player;
        private string _currentMap;
        private View.GameView _gameView;
        private GameServiceContainer _services;
        private Model.GameModel _gameModel;

        public GameController(GameServiceContainer services)
        {
            _services = services;
            _tmxManager = _services.GetService<LevelManager>();

            var startPos = new Vector2(100, 100);

            _player = new LudosPlayer(startPos, new Point(16, 16), _services) { HorizontalAcceleration = 0.035f };
            _player.DecelerationIsActive = false;
            _gameModel = new Model.GameModel(_services, _player);
            _player.AdditionalCollisionObjects = _gameModel.Crates;

            _gameView = new View.GameView(_services, _player, _gameModel);
            _currentMap = _tmxManager.CurrentMapName;
        }

        public bool IsActive { get; set; }

        public void Update(GameTime gameTime)
        {
            if (_currentMap != _tmxManager.CurrentMapName)
            {
                _player.ResetToStartPosition();
                _currentMap = _tmxManager.CurrentMapName;
                _gameView = new View.GameView(_services, _player, _gameModel);
            }

            if (!LudosGame.GameIsPaused)
            {
                _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                _gameModel.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
