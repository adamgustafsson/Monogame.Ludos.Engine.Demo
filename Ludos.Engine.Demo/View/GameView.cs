namespace LudosEngineDemo.View
{
    using System.Collections.Generic;
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Ludos.Engine.Graphics;
    using Ludos.Engine.Input;
    using Ludos.Engine.Tmx;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class GameView
    {
        private readonly ContentManager _content;
        private readonly LudosPlayer _player;
        private readonly Camera2D _camera;
        private readonly InputManager _inputManager;
        private readonly TMXManager _tmxManager;
        private readonly DebugManager _debugManager;

        private Texture2D _playerTexture16x16;
        private Texture2D _playerTexture16x24;
        private Texture2D _platform;
        private Texture2D _playerSprite;
        private Texture2D _staticBackground;
        private List<ScrollingTexture> _parallaxBackgrounds;

        private AnimationManager _animationManager;

        public GameView(ContentManager content, InputManager inputManage, TMXManager tmxManager, LudosPlayer ludosPlayer)
        {
            _content = content;
            _inputManager = inputManage;
            _tmxManager = tmxManager;
            _player = ludosPlayer;

            var graphicsDevice2 = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            _camera = new Camera2D(graphicsDevice2, _player, cameraScale: 4);
            _debugManager = new DebugManager(_content, graphicsDevice2, _inputManager, _camera, _tmxManager, _player);

            LoadContent();

            var playerSpriteFrameSize = new Point(10, 5);
            var largeOffset = new Vector2(-5, -12);
            var smallOffset = new Vector2(0, -11);

            var animations = new Dictionary<Actor.State, Animation>()
            {
                { Actor.State.Idle, new Animation(_playerSprite, _player, startFrame: new Point(0, 0), playerSpriteFrameSize, frameCount: 4) { PositionOffset = smallOffset, Scale = 1.5f } },
                {
                    Actor.State.Running, new Animation(_playerSprite, _player, startFrame: new Point(0, 1), playerSpriteFrameSize, frameCount: 10)
                    {
                        PositionOffset = smallOffset,
                        UseVelocityBasedFrameSpeed = true,
                        FrameSpeed = 5.5f,
                        Scale = 1.5f,
                    }
                },
                { Actor.State.Jumping, new Animation(_playerSprite, _player, startFrame: new Point(2, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.Falling, new Animation(_playerSprite, _player, startFrame: new Point(3, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.WallClinging, new Animation(_playerSprite, _player, startFrame: new Point(4, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.Climbing, new Animation(_playerSprite, _player, startFrame: new Point(0, 3), playerSpriteFrameSize, frameCount: 2) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.ClimbingIdle, new Animation(_playerSprite, _player, startFrame: new Point(0, 3), playerSpriteFrameSize, frameCount: 1) { PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.Swimming, new Animation(_playerSprite, _player, startFrame: new Point(5, 3), playerSpriteFrameSize, frameCount: 2) { FrameSpeed = 0.5f, PositionOffset = smallOffset, Scale = 1.5f } },
                { Actor.State.Diving, new Animation(_playerSprite, _player, startFrame: new Point(7, 3), playerSpriteFrameSize, frameCount: 2) { FrameSpeed = 0.5f, PositionOffset = smallOffset, Scale = 1.5f } },
            };

            _animationManager = new AnimationManager(_camera, animations);
        }

        public void LoadContent()
        {
            _staticBackground = _content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _playerTexture16x16 = _content.Load<Texture2D>("Assets/player");
            _playerTexture16x24 = _content.Load<Texture2D>("Assets/player16x24");
            _platform = _content.Load<Texture2D>("Assets/platform");
            _playerSprite = _content.Load<Texture2D>("Assets/Player/player-spritesheet");

            _parallaxBackgrounds = new List<ScrollingTexture>
            {
                new ScrollingTexture(_content.Load<Texture2D>("Assets/Parallax/Jungle/bg1"), _camera, new Vector2(0, 0), offsetY: 0),
                new ScrollingTexture(_content.Load<Texture2D>("Assets/Parallax/Jungle/bg2"), _camera, new Vector2(0.25f, 0.25f), offsetY: 25),
                new ScrollingTexture(_content.Load<Texture2D>("Assets/Parallax/Jungle/bg3"), _camera, new Vector2(0.50f, 0.50f), offsetY: 30),
                new ScrollingTexture(_content.Load<Texture2D>("Assets/Parallax/Jungle/bg4"), _camera, new Vector2(0.75f, 0.75f), offsetY: 35),
            };
        }

        public void Update(GameTime gameTime)
        {
            if (_inputManager.IsInputDown(InputName.Pause) && _inputManager.GetPreviousKeyboardState().IsKeyUp(_inputManager.UserControls[InputName.Pause].Key))
            {
                LudosGame.GameIsPaused = !LudosGame.GameIsPaused;
            }

            if (!LudosGame.GameIsPaused)
            {
                _camera.Update(gameTime);

                foreach (var bg in _parallaxBackgrounds)
                {
                    bg.Update(gameTime);
                }

                _debugManager.Update(gameTime);
            }

            LudosGame.GameStates[States.Menu].IsActive = LudosGame.GameIsPaused;

            if (_tmxManager.CurrentMapName == "Level3" && !LudosGame.GameIsPaused)
            {
                _animationManager.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_tmxManager.CurrentMapName != "Level3")
            {
                spriteBatch.Draw(_staticBackground, Vector2.Zero, Color.White);
            }
            else
            {
                foreach (var bg in _parallaxBackgrounds)
                {
                    bg.Draw(gameTime, spriteBatch);
                }
            }

            DrawTmxMap(spriteBatch);
            DrawPlayer(spriteBatch);

            if (_tmxManager.CurrentMapName != "Level3")
            {
                spriteBatch.Draw(_playerTexture16x16, _camera.VisualizeCordinates(_player.Bounds), Color.White);
            }
            else
            {
                _animationManager.Draw(spriteBatch);
            }

            _debugManager.DrawScaledContent(spriteBatch);
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
                if (_tmxManager.CurrentMapName == "Level3")
                {
                    spriteBatch.Draw(_platform, _camera.VisualizeCordinates(platform.Bounds), Color.White);
                }
                else
                {
                    _debugManager.DrawRectancgle(spriteBatch, platform.Bounds, new Color(20, 27, 63));
                }
            }
        }

        private void DrawPlayer(SpriteBatch spriteBatch)
        {
        }
    }
}
