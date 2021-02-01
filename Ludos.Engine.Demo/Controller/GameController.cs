using Ludos.Engine.Managers;
using Ludos.Engine.Model;
using Ludos.Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LudosEngineDemo
{
    public class GameController : GameState
    {
        private readonly TMXManager _tmxManager;
        private readonly LudosPlayer _player;
        private readonly View.GameView _gameView;

        public GameController(LudosGame game, GraphicsDevice graphicsDevice, ContentManager content, InputManager inputManager)
            : base(game, graphicsDevice, content, inputManager) 
        {
            var level1 = new TMXMapInfo
            {
                TmxFilePath = "Levels/Level1/Level1.tmx",
                ResourcePath = "Levels/Level1/TileImages",
                NonDefaultLayerNames = null,
                MovingPlatformSize = new Point(48, 16)
            };
            var level2 = new TMXMapInfo
            {
                TmxFilePath = "Levels/Level2/Level2.tmx",
                ResourcePath = "Levels/Level2/TileImages",
                NonDefaultLayerNames = null,
                MovingPlatformSize = new Point(48, 16)
            };

            var level3 = new TMXMapInfo
            {
                TmxFilePath = "Levels/Level3/Level3.tmx",
                ResourcePath = "Levels/Level3/TileImages",
                NonDefaultLayerNames = null,
                MovingPlatformSize = new Point(48, 16)
            };

            _tmxManager = new TMXManager(Content, new List<TMXMapInfo> { level1, level2, level3 });
            _player = new LudosPlayer(new Vector2(500, 280), new Point(16, 16), _tmxManager, InputManager) { HorizontalAcceleration = 0.035f };
            _gameView = new View.GameView(Game, Content, Graphics, InputManager, _tmxManager, _player);

            LoadMap("Level3.tmx");
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.GameIsPaused)
            {
                _tmxManager.Update(gameTime);
                _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            _gameView.Update(gameTime);
        }
        public override void PostUpdate(GameTime gameTime)
        {

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameView.Draw(gameTime, spriteBatch);
        }
        public void DrawDebugPanel(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _gameView.DrawDebugPanel(gameTime, spriteBatch);
        }
        public void LoadMap(string levelName)
        {
            //_player.Bounds.Height = levelName == "Level2.tmx" ? 24 : 16;
            _player.ResetToStartPosition();
            _tmxManager.LoadMap(levelName);
        }
    }
}
