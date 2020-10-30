using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Monogame.Platformer.Engine
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Map _map;
        float AspectRatio;
        Point OldWindowSize;
        RenderTarget2D OffScreenRenderTarget;
        // Add the class
        FpsCounter.FpsCounter fps = new FpsCounter.FpsCounter();
        SpriteFont font;
        Texture2D _backGround;
        Texture2D _player;

        public Game1()
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

            if (Window.ClientBounds.Width != OldWindowSize.X)
            { // We're changing the width
                // Set the new backbuffer size
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / AspectRatio);
            }
            if (Window.ClientBounds.Height != OldWindowSize.Y)
            { // we're changing the height
                // Set the new backbuffer size
                _graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * AspectRatio);
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            }

            _graphics.ApplyChanges();

            // Update the old window size with what it is currently
            OldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

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
            AspectRatio = GraphicsDevice.Viewport.AspectRatio;
            OldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            OffScreenRenderTarget = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _map = TMXContentProcessor.LoadTMX("Levels/Level1/map1.tmx", "Levels/Level1/TileImages", Content);
            //_map = TMXContentProcessor.LoadTMX("Levels/Level-3_org.tmx", "Levels/TileTextures", Content);

            font = Content.Load<SpriteFont>("Fonts/Segoe");
            _backGround = Content.Load<Texture2D>("Levels/Level1/TileImages/background");
            _player = Content.Load<Texture2D>("Assets/player");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            fps.Update(gameTime);

            base.Update(gameTime);
        }

        protected override bool BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(OffScreenRenderTarget);
            return base.BeginDraw();
        }
        protected override void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(OffScreenRenderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
            base.EndDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var rect = new Rectangle(0, 0, 1920, 1080);

            // TODO: Add your drawing code here
            //_spriteBatch.Begin();
            Matrix transform = Matrix.CreateScale(4);

            Map.InitObjectDrawing(GraphicsDevice);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);
            //_spriteBatch.Begin();
            _spriteBatch.Draw(_backGround, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_player, new Vector2(5, 100), Color.White);
            _map.DrawLayer(_spriteBatch, 0, rect, 0f);
            //_map.DrawLayer(_spriteBatch, 1, rect, 0f);
            //_map.DrawLayer(_spriteBatch, 2, rect, 0f);
            //_map.DrawObjectLayer(_spriteBatch, 0, rect, 0f);

            // Draw the fps msg


            _spriteBatch.End();

            _spriteBatch.Begin();

            fps.DrawFps(_spriteBatch, font, new Vector2(10f, 10f), Color.WhiteSmoke);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
