namespace LudosEngineDemo
{
    using System.Collections.Generic;
    using System.Linq;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Core;
    using Ludos.Engine.Managers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class LudosEngineDemo : LudosGame
    {
        public LudosEngineDemo()
            : base(graphicsScale: 4)
        {
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        public static bool DoExitGame { get; set; }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.ApplyChanges();

            var tmxMapInfoList = new List<TMXMapInfo>()
            {
                new TMXMapInfo { TmxFilePath = "Levels/Level1/Level1.tmx", ResourcePath = "Levels/Level1/TileImages", NonDefaultLayerNames = null, MovingPlatformSize = new Point(48, 16) },
                new TMXMapInfo { TmxFilePath = "Levels/Level2/Level2.tmx", ResourcePath = "Levels/Level2/TileImages", NonDefaultLayerNames = null, MovingPlatformSize = new Point(48, 16) },
                new TMXMapInfo { TmxFilePath = "Levels/Level3/Level3.tmx", ResourcePath = "Levels/Level3/TileImages", NonDefaultLayerNames = null, MovingPlatformSize = new Point(48, 16) },
            };

            var userControls = new Dictionary<string, Input>()
                {
                    { InputName.MoveUp, new Input { Key = Keys.W, Button = Buttons.LeftThumbstickUp } },
                    { InputName.MoveDown, new Input { Key = Keys.S, Button = Buttons.LeftThumbstickDown } },
                    { InputName.MoveLeft, new Input { Key = Keys.A, Button = Buttons.LeftThumbstickLeft } },
                    { InputName.MoveRight, new Input { Key = Keys.D, Button = Buttons.LeftThumbstickRight } },
                    { InputName.Jump, new Input { Key = Keys.Space, Button = Buttons.A } },
                    { InputName.ActionButton1, new Input { Key = Keys.N, Button = Buttons.B } },
                    { InputName.ActionButton2, new Input { Key = Keys.M, Button = Buttons.X } },
                    { InputName.Pause, new Input { Key = Keys.Escape, Button = Buttons.Start } },
                    { InputName.Select, new Input { Key = Keys.LeftShift, Button = Buttons.LeftShoulder } },
                };

            InitializeTmxManager(tmxMapInfoList);
            InitializeInputManager(userControls);

            GameStates = new IGameState[]
            {
                new GameController(Content, Services) { IsActive = true },
                new MenuController(Content, Services), // { IsActive = true },
            };

            Map.InitObjectDrawing(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (DoExitGame)
            {
                Exit();
            }

            foreach (var activeState in GameStates.Where(x => x.IsActive))
            {
                activeState.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 16, 33));
            Matrix transform = Matrix.CreateScale(GraphicsScale);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);

            foreach (var activeState in GameStates.Where(x => x.IsActive))
            {
                activeState.Draw(gameTime, SpriteBatch);
            }

            SpriteBatch.End();

            if (!GameIsPaused)
            {
                var gameController = GameStates.Where(x => x is GameController && x.IsActive).FirstOrDefault();

                if (gameController != null && !GameIsPaused)
                {
                    SpriteBatch.Begin();
                    (gameController as GameController).DrawDebugPanel(gameTime, SpriteBatch);
                    SpriteBatch.End();
                }
            }

            base.Draw(gameTime);
        }
    }
}
