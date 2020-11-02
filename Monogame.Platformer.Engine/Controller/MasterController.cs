﻿using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogame.Platformer.Engine.Model;
using System;
using System.Net.Http.Headers;

namespace Monogame.Platformer.Engine.Controller
{
    class MasterController : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Map _map;
        
        private float _aspectRatio;
        private Point _oldWindowSize;
        private RenderTarget2D _offScreenRenderTarget;
        
        private FpsCounter.FpsCounter _fpsCounter = new FpsCounter.FpsCounter();
        
        private SpriteFont _fpsFont;
        private Texture2D _backGround;
        private Texture2D _playerTexture;
        private Player _player;

        public MasterController()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
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
            _map = TMXContentProcessor.LoadTMX("Levels/Level1/map1.tmx", "Levels/Level1/TileImages", Content);
            //_map = TMXContentProcessor.LoadTMX("Levels/Level-3_org.tmx", "Levels/TileTextures", Content);

            _fpsFont = Content.Load<SpriteFont>("Fonts/Segoe");
            _backGround = Content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _playerTexture = Content.Load<Texture2D>("Assets/player");

            _player = new Player(new Point(10, 175), _map);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _fpsCounter.Update(gameTime);
            _player.Update((float)gameTime.ElapsedGameTime.TotalSeconds, Keyboard.GetState());

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

            Matrix transform = Matrix.CreateScale(4);

            Map.InitObjectDrawing(GraphicsDevice);
     
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            _spriteBatch.Draw(_backGround, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_playerTexture, _player.GetPositionV(), Color.White);
            _map.DrawLayer(_spriteBatch, 0, rect, 0f);

            Texture2D r = new Texture2D(_graphics.GraphicsDevice, _player.Bounds.Width, _player.Bounds.Height);

            Color[] data = new Color[_player.Bounds.Width * _player.Bounds.Height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White * 0.50f;
            r.SetData(data);

            Vector2 coor = _player.GetPositionV();
            _spriteBatch.Draw(r, coor, Color.White);

            _spriteBatch.End();

            _spriteBatch.Begin();  
            
            _fpsCounter.DrawFps(_spriteBatch, _fpsFont, new Vector2(10f, 10f), Color.WhiteSmoke);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}