using Ludos.Engine.Graphics;
using Ludos.Engine.Managers;
using Ludos.Engine.Model;
using Ludos.Engine.Core;
using Ludos.Engine.Utilities.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LudosEngineDemo
{
    public class GameController : GameState
    {
        private readonly Texture2D _gameBackgroundTexture;
        private readonly Texture2D _playerTexture16x16;
        private readonly Texture2D _playerTexture16x24;
        private readonly TMXManager _tmxManager;
        private readonly Player _player;
        private readonly Camera2D _camera;
        private readonly DebugManager _debugManager;

        public GameController(LudosGame game, GraphicsDevice graphicsDevice, ContentManager content, InputManager inputManager)
            : base(game, graphicsDevice, content, inputManager) 
        {
            _gameBackgroundTexture = _content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _playerTexture16x16 = _content.Load<Texture2D>("Assets/player");
            _playerTexture16x24 = _content.Load<Texture2D>("Assets/player16x24");

            var level1 = new TmxMapInfo
            {
                Name = "Level1.tmx",
                Path = "Levels/Level1/",
                ResourcePath = "Levels/Level1/TileImages",
                NonDefaultLayerNames = null,
                MovingPlatformSize = new Point(48, 16)
            };
            var level2 = new TmxMapInfo
            {
                Name = "Level2.tmx",
                Path = "Levels/Level2/",
                ResourcePath = "Levels/Level2/TileImages",
                NonDefaultLayerNames = null,
                MovingPlatformSize = new Point(48, 16)
            };

            _tmxManager = new TMXManager(_content, new List<TmxMapInfo> { level1, level2 });         
            _player = new Player(new Vector2(20, 280), 16, 16, _tmxManager, inputManager);
            _camera = new Camera2D(_graphicsDevice, _player, cameraScale: 4);
            _debugManager = new DebugManager(_content, _graphicsDevice, _inputManager, _camera, _tmxManager, _player);
        }

        public override void Update(GameTime gameTime)
        {
            if (_inputManager.IsInputDown(InputName.Pause) && _inputManager.GetPreviousKeyboardState().IsKeyUp(_inputManager.UserControls[InputName.Pause].Key))
                _game.GameIsPaused = !_game.GameIsPaused;

            if (!_game.GameIsPaused)
            {
                _tmxManager.Update(gameTime);
                _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                _camera.Update();
                _debugManager.Update(gameTime);
            }
        }
        public override void PostUpdate(GameTime gameTime)
        {

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_gameBackgroundTexture, Vector2.Zero, Color.White);
            _tmxManager.CurrentMap.DrawLayer(spriteBatch, 0, _camera.CameraBounds, 0f);

            if (_player.Bounds.Height == 16)
                spriteBatch.Draw(_playerTexture16x16, _camera.VisualizeCordinates(_player.Bounds), Color.White);
            else
                spriteBatch.Draw(_playerTexture16x24, _camera.VisualizeCordinates(_player.Bounds), Color.White);

            foreach (var platform in _tmxManager.MovingPlatforms)
            {
                _debugManager.DrawRectancgle(spriteBatch, platform.Bounds, Color.DarkBlue);
            }

            _debugManager.DrawScaledContent(spriteBatch); 
        }
        public void DrawDebugPanel(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _debugManager.DrawDebugPanel(spriteBatch, gameTime);
        }

        public void LoadMap(string levelName)
        {
            _player.Bounds.Height = levelName == "Level1.tmx" ? 16 : 24;
            _player.Bounds.Location = new System.Drawing.PointF(20, 280);
            _tmxManager.LoadMap(levelName);
        }
    }
}
