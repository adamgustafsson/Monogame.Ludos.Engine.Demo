using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Model;
using Model.Actors;
using System;
using System.Collections.Generic;
using Utillities.Debug;

namespace Monogame.Platformer.Engine.Controller
{
    class MasterController : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model.World.TMXManager _tmxManager;

        private float _aspectRatio;
        private Point _oldWindowSize;
        private RenderTarget2D _offScreenRenderTarget;
        
        private Texture2D _backGround;
        private Texture2D _playerTexture;
        private Player _player;

        private View.Camera2D _camera;

        private DebugInfo _debugInfo;

        public MasterController()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;

            _debugInfo = new DebugInfo();
       }

        public void OnResize(Object sender, EventArgs e)
        {
            // Remove this event handler, so we don't call it when we change the window size in here
            Window.ClientSizeChanged -= OnResize;

            if (Window.ClientBounds.Width != _oldWindowSize.X)
            { // We're changing the width
                // Set the new backbuffer size
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / _aspectRatio);
            }
            if (Window.ClientBounds.Height != _oldWindowSize.Y)
            { // we're changing the height
                // Set the new backbuffer size
                _graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * _aspectRatio);
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            }

            _graphics.ApplyChanges();

            // Update the old window size with what it is currently
            _oldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

            // add this event handler back
            Window.ClientSizeChanged += OnResize;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this._graphics.PreferredBackBufferHeight = 1080;
            this._graphics.PreferredBackBufferWidth = 1920;
            this._graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Set up initial values
            _aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            _oldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            _offScreenRenderTarget = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backGround = Content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _playerTexture = Content.Load<Texture2D>("Assets/player");


            var map = new Model.World.TmxMapInfo
            {
                Name = "map1.tmx",
                Path = "Levels/Level1/",
                ResourcePath = "Levels/Level1/TileImages",
                NonDefaultLayerNames = null,
                MovingPlatformSize = new Point(48, 16)
            };
             
            _tmxManager = new Model.World.TMXManager(Content, new List<Model.World.TmxMapInfo> { map });
            _player = new Player(new System.Drawing.PointF(1271, 280), _tmxManager);
            _debugInfo.LoadContent(Content, _graphics.GraphicsDevice);

            _camera = new View.Camera2D(_graphics.GraphicsDevice, _player, cameraScale: 4);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _tmxManager.Update(gameTime, _camera.CameraBounds);
            _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds, Keyboard.GetState());
            _camera.Update();
            _debugInfo.Update(gameTime);

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(_offScreenRenderTarget);
            return base.BeginDraw();
        }
        protected override void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_offScreenRenderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
            base.EndDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var rect = new Rectangle(0, 0, 1920, 1080);
            var playerPos = _camera.VisualizeCordinates(_player.GetPositionV());

            Matrix transform = Matrix.CreateScale(4);

            Map.InitObjectDrawing(GraphicsDevice);
     
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            _spriteBatch.Draw(_backGround, Vector2.Zero, Color.White);
            _tmxManager.CurrentMap.DrawLayer(_spriteBatch, 0, _camera.CameraBounds, 0f);
            //_spriteBatch.Draw(_playerTexture, playerPos, Color.White);

            #region Debug

            _debugInfo.DrawRectancgle(_spriteBatch, (int)_player.Bounds.Width, (int)_player.Bounds.Height, playerPos, transparancy: 0.50f);
            //_debugInfo.DrawRectancgle(_spriteBatch, (int)_player.BottomDetectBounds.Width, (int)_player.BottomDetectBounds.Height, _camera.VisualizeCordinates(_player.BottomDetectBounds), color: Color.Red, transparancy: 0.50f);
            //_debugInfo.DrawRectancgle(_spriteBatch, (int)_camera.MovementBounds.Width, (int)_camera.MovementBounds.Height, _camera.VisualizeCordinates(_camera.MovementBounds), transparancy: 0.50f);
            //_tmxManager.CurrentMap.DrawObjectLayer(_spriteBatch, 0, Utillities.Utilities.Round(_camera.CameraBounds), 0f);

            #endregion

            foreach (var platform in _tmxManager.MovingPlatforms)
            {
                var platPos = _camera.VisualizeCordinates(new Vector2(platform.Bounds.X, platform.Bounds.Y));
                _debugInfo.DrawRectancgle(_spriteBatch, (int)platform.Bounds.Width, (int)platform.Bounds.Height, platPos, Color.DarkBlue);
                //_debugInfo.DrawRectancgle(_spriteBatch, (int)platform.DetectionBounds.Width, (int)platform.DetectionBounds.Height, platPos, color: Color.Green);
            }

            _spriteBatch.End();

            _spriteBatch.Begin();

            _debugInfo.DrawDebugInfo(gameTime, _spriteBatch, _player);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
