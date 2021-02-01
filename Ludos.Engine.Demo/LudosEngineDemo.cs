namespace LudosEngineDemo
{
    using System.Collections.Generic;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Core;
    using Ludos.Engine.Managers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class LudosEngineDemo : LudosGame
    {
        private InputManager _inputManager;

        private Model.GameModel _gameModel;
        private View.GameView _gameView;

        private GameState _currentState;
        private GameState _queuedState;

        public LudosEngineDemo()
            : base(graphicsScale: 4)
        {
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.ApplyChanges();

            _inputManager = new InputManager(new System.Drawing.Size(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight))
            {
                UserControls = new Dictionary<string, Input>()
                {
                    { InputName.MoveUp, new Input { Key = Keys.W, Button = Buttons.LeftThumbstickUp } },
                    { InputName.MoveDown, new Input { Key = Keys.S, Button = Buttons.LeftThumbstickDown } },
                    { InputName.MoveLeft, new Input { Key = Keys.A, Button = Buttons.LeftThumbstickLeft } },
                    { InputName.MoveRight, new Input { Key = Keys.D, Button = Buttons.LeftThumbstickRight} },
                    { InputName.Jump, new Input { Key = Keys.Space, Button = Buttons.A } },
                    { InputName.ActionButton1, new Input { Key = Keys.N, Button = Buttons.B} },
                    { InputName.ActionButton2, new Input { Key = Keys.M, Button = Buttons.X} },
                    { InputName.Pause, new Input { Key = Keys.Escape, Button = Buttons.Start } },
                    { InputName.Select, new Input { Key = Keys.LeftShift, Button = Buttons.LeftShoulder } },
                },
            };

            //_gameModel = new GameModel();

            GameStates = new GameState[]
            {
                new MenuController(this, GraphicsDevice, Content, _inputManager),
                new GameController(this, GraphicsDevice, Content, _inputManager),
            };

            _currentState = GameStates[States.Game];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(Window.ClientBounds);
            
            if (_queuedState != null) {
                _currentState = _queuedState;
                _queuedState = null;
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            if (GameIsPaused)
            {
                GameStates[States.Menu].Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 16, 33));

            Matrix transform = Matrix.CreateScale(GraphicsScale);

            Map.InitObjectDrawing(GraphicsDevice);

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);
             _currentState.Draw(gameTime, SpriteBatch);

            if(GameIsPaused)
            {
                (GameStates[States.Menu] as MenuController).MenuType = MenuController.MenuTypes.PauseMenu;
                GameStates[States.Menu].Draw(gameTime, SpriteBatch);
            } 
            else if ((GameStates[States.Menu] as MenuController).MenuType == MenuController.MenuTypes.PauseMenu)
            {
                (GameStates[States.Menu] as MenuController).MenuType = MenuController.MenuTypes.ContentBased;
            }

            SpriteBatch.End();

            if (_currentState is GameController && !GameIsPaused)
            {
                SpriteBatch.Begin();
                (_currentState as GameController).DrawDebugPanel(gameTime, SpriteBatch);
                SpriteBatch.End();
            }

            base.Draw(gameTime);

        }
        public override void ChangeState(int gameStateIndex)
        {
            _queuedState = GameStates[gameStateIndex];
        }

        public override void LoadMap(string mapName)
        {
            (GameStates[States.Game] as GameController).LoadMap(mapName.EndsWith(".tmx") ? mapName : mapName + ".tmx");
        }
    }

}
