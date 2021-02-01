
using Ludos.Engine.Core;
using Ludos.Engine.Graphics;
using Ludos.Engine.Managers;
using Ludos.Engine.Model;
using Ludos.Engine.Model.World;
using Ludos.Engine.Utilities.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LudosEngineDemo.View
{
    public class GameView
    {
        private readonly LudosGame _game;
        private readonly ContentManager _content;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly LudosPlayer _player;
        private readonly Camera2D _camera;
        private readonly InputManager _inputManager;
        private readonly TMXManager _tmxManager;
        private readonly DebugManager _debugManager;

        private Texture2D _gameBackgroundTexture;
        private Texture2D _playerTexture16x16;
        private Texture2D _playerTexture16x24;
        private Texture2D _platform;

        private AnimationManager _animationManager;


        public GameView(LudosGame game, ContentManager content, GraphicsDevice graphicsDevice, InputManager inputManage, TMXManager tmxManager, LudosPlayer ludosPlayer)
        {
            _game = game;
            _content = content;
            _graphicsDevice = graphicsDevice;
            _inputManager = inputManage;
            _tmxManager = tmxManager;
            _player = ludosPlayer;
            _camera = new Camera2D(_graphicsDevice, _player, cameraScale: 4);
            _debugManager = new DebugManager(_content, _graphicsDevice, _inputManager, _camera, _tmxManager, _player);

            LoadContent();
        }

        public void LoadContent()
        {
            _gameBackgroundTexture = _content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _playerTexture16x16 = _content.Load<Texture2D>("Assets/player");
            _playerTexture16x24 = _content.Load<Texture2D>("Assets/player16x24");
            _platform = _content.Load<Texture2D>("Assets/platform");


            var playerSprite = _content.Load<Texture2D>("Assets/Player/player-spritesheet");
            var playerSpriteFrameSize = new Point(10, 5);
            var largeOffset = new Vector2(-5, -12);
            var smallOffset = new Vector2(0, -11);

            var animations = new Dictionary<Actor.State, Animation>()
            {
                { Actor.State.Idle, new Animation(playerSprite, _player, startFrame: new Point(0, 0), playerSpriteFrameSize, frameCount: 4) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.Running, new Animation(playerSprite, _player, startFrame: new Point(0, 1), playerSpriteFrameSize, frameCount: 10) {
                    PositionOffset = smallOffset,
                    UseVelocityBasedFrameSpeed = true,
                    FrameSpeed = 5.5f,
                    Scale = 1.5f} },
                { Actor.State.Jumping,  new Animation(playerSprite, _player, startFrame: new Point(2, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f} },
                { Actor.State.Falling,  new Animation(playerSprite, _player, startFrame: new Point(3, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.WallClinging,  new Animation(playerSprite, _player, startFrame: new Point(4, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset =smallOffset, Scale = 1.5f } },
                { Actor.State.Climbing,  new Animation(playerSprite, _player, startFrame: new Point(0, 3), playerSpriteFrameSize, frameCount: 2) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.ClimbingIdle,  new Animation(playerSprite, _player, startFrame: new Point(0, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f } }
            };

            _animationManager = new AnimationManager(_camera, animations);
        }

        public void Update(GameTime gameTime)
        {
            if (_inputManager.IsInputDown(InputName.Pause) && _inputManager.GetPreviousKeyboardState().IsKeyUp(_inputManager.UserControls[InputName.Pause].Key))
                _game.GameIsPaused = !_game.GameIsPaused;

            if (!_game.GameIsPaused)
            {
                _camera.Update();
                _debugManager.Update(gameTime);
            }

            //_playerAnimationRunRight.FrameSpeed = 5.5f / _player.Velocity.X;
            //_playerAnimationRunLeft.FrameSpeed = 5.5f / -_player.Velocity.X;
            if (!_game.GameIsPaused)
                _animationManager.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_tmxManager.CurrentMapName != "Level3")
            {
                spriteBatch.Draw(_gameBackgroundTexture, Vector2.Zero, Color.White);
            }

            DrawTmxMap(spriteBatch);
            DrawPlayer(spriteBatch);

            _debugManager.DrawScaledContent(spriteBatch);

            _animationManager.Draw(spriteBatch);
        }

        public void DrawDebugPanel(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _debugManager.DrawDebugPanel(spriteBatch, gameTime);
        }

        private void DrawTmxMap(SpriteBatch spriteBatch)
        {

            _tmxManager.DrawTileLayers(spriteBatch, _camera.CameraBounds, 0f);

            foreach (var platform in _tmxManager.MovingPlatforms)
            {
                spriteBatch.Draw(_platform, _camera.VisualizeCordinates(platform.Bounds), Color.White);
            }
        }

        private void DrawPlayer(SpriteBatch spriteBatch)
        {
            if (_tmxManager.CurrentMapName == "Level1")
            {
                spriteBatch.Draw(_playerTexture16x16, _camera.VisualizeCordinates(_player.Bounds), Color.White);
            }
            else
            {
                //spriteBatch.Draw(_playerTexture16x24, _camera.VisualizeCordinates(_player.Bounds), Color.White);
            }
        }
    }
}
