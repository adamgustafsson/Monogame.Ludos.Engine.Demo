using FuncWorks.XNA.XTiled;
using Ludos.Engine.Managers;
using Ludos.Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace LudosEngineDemo
{
    class Game1 : LudosGame
    {
        private InputManager _inputManager;
        private GameState _currentState;
        private GameState _nextState;

        public Game1()
            : base(graphicsScale: 4)
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.ApplyChanges();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            _inputManager = new InputManager(new System.Drawing.Size(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight))
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
                    { InputName.Select, new Input { Key = Keys.LeftShift, Button = Buttons.LeftShoulder} }
                }
            };

            GameStates = new GameState[]
            {
                new MenuController(this, GraphicsDevice, Content, _inputManager),
                new GameController(this, GraphicsDevice, Content, _inputManager)
            };

            //_menuController = new MenuController(this, GraphicsDevice, Content, _inputManager);
            //_currentState = new GameController(this, GraphicsDevice, Content, _inputManager, levelIndex: 0);

            _currentState = GameStates[States.Menu];

            base.LoadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            _inputManager.Update(Window.ClientBounds);
            
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix transform = Matrix.CreateScale(GraphicsScale);

            Map.InitObjectDrawing(GraphicsDevice);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, transform);
             _currentState.Draw(gameTime, _spriteBatch);

            if(GameIsPaused)
            {
                (GameStates[States.Menu] as MenuController).MenuType = MenuController.MenuTypes.PauseMenu;
                GameStates[States.Menu].Draw(gameTime, _spriteBatch);
            } 
            else if ((GameStates[States.Menu] as MenuController).MenuType == MenuController.MenuTypes.PauseMenu)
            {
                (GameStates[States.Menu] as MenuController).MenuType = MenuController.MenuTypes.ContentBased;
            }

            _spriteBatch.End();

            if (_currentState is GameController && !GameIsPaused)
            {
                _spriteBatch.Begin();
                (_currentState as GameController).DrawDebugPanel(gameTime, _spriteBatch);
                _spriteBatch.End();
            }

            base.Draw(gameTime);

        }
        public override void ChangeState(int gameStateIndex)
        {
            _nextState = GameStates[gameStateIndex];
        }

        public override void LoadMap(string mapName)
        {
            (GameStates[States.Game] as GameController).LoadMap(mapName.EndsWith(".tmx") ? mapName : mapName + ".tmx");
        }
    }

}
