namespace LudosEngineDemo.View
{
    using System.Collections.Generic;
    using System.Linq;
    using global::LudosEngineDemo.Model;
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Ludos.Engine.Graphics;
    using Ludos.Engine.Input;
    using Ludos.Engine.Level;
    using Ludos.Engine.Particles;
    using Ludos.Engine.Sound;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class GameView
    {
        private readonly LudosPlayer _player;
        private readonly Camera2D _camera;
        private readonly InputManager _inputManager;
        private readonly LevelManager _tmxManager;
        private readonly SoundManager _soundManager;
        private readonly DebugManager _debugManager;

        private Texture2D _playerTexture16x16;
        private Texture2D _playerTexture16x24;
        private Texture2D _playerTexture16x32;
        private Texture2D _coreBoxTexture;
        private Texture2D _boxTexture;
        private Texture2D _platform;
        private Texture2D _playerSprite;
        private Texture2D _staticBackground;
        private List<ScrollingTexture> _parallaxBackgrounds;

        private SpriteFont _debugToolFont;

        private AnimationManager _animationManager;
        private ParticleManager _particleManager;

        private GameModel _gameModel;

        public GameView(GameServiceContainer services, LudosPlayer ludosPlayer, GameModel gameModel)
        {
            var contentManager = services.GetService<ContentManager>();
            var graphicsDevice = contentManager.GetGraphicsDevice();

            _player = ludosPlayer;
            _camera = new Camera2D(graphicsDevice, _player, cameraScale: 4);
            _gameModel = gameModel;

            LoadContent(contentManager);

            _inputManager = services.GetService<InputManager>();
            _tmxManager = services.GetService<LevelManager>();
            _animationManager = new AnimationManager(_camera, SetUpPlayerAnimations());
            _particleManager = new ParticleManager(graphicsDevice, _camera, SetUpParticles());
            _debugManager = new DebugManager(services, _camera, _player, _debugToolFont);
            _soundManager = new SoundManager(contentManager, new SoundInfo
            {
                SoundEffectsPath = "Assets/Sound/Effects",
                SoundEffectTitles = new List<string>() { "Jump1", "Splash" },
                SoundTracksPath = "Assets/Sound/Tracks",
                SoundTracksTitles = new List<string>() { "Arcade-Heroes" },
            });

            _soundManager.MusicEnabled = false;
            _soundManager.SoundEnabled = false;
        }

        ~GameView()
        {
            _soundManager.StopSoundTrack();
        }

        public void LoadContent(ContentManager content)
        {
            _staticBackground = content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _playerTexture16x16 = content.Load<Texture2D>("Assets/player");
            _playerTexture16x24 = content.Load<Texture2D>("Assets/player16x24");
            _playerTexture16x32 = content.Load<Texture2D>("Assets/player16x32");
            _coreBoxTexture = content.Load<Texture2D>("Assets/box");
            _boxTexture = content.Load<Texture2D>("Assets/box2");
            _platform = content.Load<Texture2D>("Assets/platform");
            _playerSprite = content.Load<Texture2D>("Assets/Player/player-spritesheet");
            _debugToolFont = content.Load<SpriteFont>("Fonts/Segoe");

            _parallaxBackgrounds = new List<ScrollingTexture>
            {
                new ScrollingTexture(content.Load<Texture2D>("Assets/Parallax/Jungle/bg1"), _camera, new Vector2(0, 0), offsetY: 0),
                new ScrollingTexture(content.Load<Texture2D>("Assets/Parallax/Jungle/bg2"), _camera, new Vector2(0.25f), offsetY: 20),
                new ScrollingTexture(content.Load<Texture2D>("Assets/Parallax/Jungle/bg3"), _camera, new Vector2(0.50f), offsetY: 45),
                new ScrollingTexture(content.Load<Texture2D>("Assets/Parallax/Jungle/bg4"), _camera, new Vector2(0.75f), offsetY: 65),
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
                _soundManager.PlaySoundTrack("Arcade-Heroes");

                if (_player.CurrentState == Actor.State.Jumping && _player.PreviousState != Actor.State.Jumping)
                {
                    _soundManager.PlaySound(0);
                }

                if (_player.CurrentState == Actor.State.Swimming && _player.PreviousState != Actor.State.Swimming)
                {
                    _soundManager.PlaySound(1);
                }

                _particleManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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

            _particleManager.Draw((float)gameTime.ElapsedGameTime.TotalSeconds, spriteBatch);

            foreach (var crate in _gameModel.Crates)
            {
                spriteBatch.Draw(_tmxManager.CurrentMapName != "Level3" ? _coreBoxTexture : _boxTexture, _camera.VisualizeCordinates(crate.Bounds), Color.White);
            }


            if (_tmxManager.CurrentMapName != "Level3")
            {
                var ptexture = _player.Bounds.Height <= 16 ? _playerTexture16x16 : (_player.Bounds.Height > 24 ? _playerTexture16x32 : _playerTexture16x24);
                spriteBatch.Draw(ptexture, _camera.VisualizeCordinates(_player.Bounds), Color.White);
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

        private Dictionary<Actor.State, Animation> SetUpPlayerAnimations()
        {
            var playerSpriteFrameSize = new Point(10, 5);
            var largeOffset = new Vector2(-5, -12);
            var smallOffset = new Vector2(-1, -11);

            return new Dictionary<Actor.State, Animation>()
            {
                { Actor.State.Idle, new Animation(_playerSprite, _player, startFrame: new Point(0, 0), playerSpriteFrameSize, frameCount: 4, scale: 1.5f) },
                {
                    Actor.State.Running, new Animation(_playerSprite, _player, startFrame: new Point(0, 1), playerSpriteFrameSize, frameCount: 10)
                    {
                        PositionOffset = smallOffset,
                        UseVelocityBasedFrameSpeed = true,
                        FrameSpeed = 5.5f,
                        Scale = 1.5f,
                    }
                },
                { Actor.State.Jumping, new Animation(_playerSprite, _player, startFrame: new Point(2, 3), playerSpriteFrameSize, frameCount: 1, scale: 1.5f) },
                { Actor.State.Falling, new Animation(_playerSprite, _player, startFrame: new Point(3, 3), playerSpriteFrameSize, frameCount: 1, scale: 1.5f) },
                { Actor.State.WallClinging, new Animation(_playerSprite, _player, startFrame: new Point(4, 3), playerSpriteFrameSize, frameCount: 1, scale: 1.5f)},
                { Actor.State.Climbing, new Animation(_playerSprite, _player, startFrame: new Point(0, 3), playerSpriteFrameSize, frameCount: 2, scale: 1.5f) },
                { Actor.State.ClimbingIdle, new Animation(_playerSprite, _player, startFrame: new Point(0, 3), playerSpriteFrameSize, frameCount: 1, scale: 1.5f) },
                { Actor.State.Swimming, new Animation(_playerSprite, _player, startFrame: new Point(5, 3), playerSpriteFrameSize, frameCount: 2, scale: 1.5f) { FrameSpeed = 0.5f } },
                { Actor.State.Diving, new Animation(_playerSprite, _player, startFrame: new Point(7, 3), playerSpriteFrameSize, frameCount: 2, scale: 1.5f) { FrameSpeed = 0.5f, } },
            };
        }

        private List<ParticleSystemDefinition> SetUpParticles()
        {
            var particlePositions = _tmxManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerParticles, _tmxManager.GetCurrentMapBounds()).Where(x => x.Type == "torch").Select(x => x.Bounds.Center.ToVector2() - new Vector2(1, 0.5f)).ToList();

            var fireParticleSystemDef = new ParticleSystemDefinition
            {
                ParticleType = typeof(FireParticle),
                Amount = 25,
                Positions = particlePositions,
                Scale = 1.3f,
                DoRepeat = true,
                Type = ParticleSystemType.StaticPositions,
            };

            var explosionParticleSystemDef = new ParticleSystemDefinition
            {
                ParticleType = typeof(ExplosionParticle),
                Amount = 150,
                Positions = new List<Vector2>() { Vector2.Zero },
                Scale = 1f,
                DoRepeat = false,
                Type = ParticleSystemType.RenderOnTrigger,
            };

            return new List<ParticleSystemDefinition>() { fireParticleSystemDef, explosionParticleSystemDef };
        }
    }
}
