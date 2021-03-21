namespace LudosEngineDemo
{
    using System;
    using System.Collections.Generic;
    using Ludos.Engine.Core;
    using Ludos.Engine.Graphics;
    using Ludos.Engine.Input;
    using Ludos.Engine.Tmx;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class MenuController : IGameState
    {
        private Texture2D _startMenuBackgroundTexture;
        private Texture2D _startMenuButtonTexture;
        private SpriteFont _buttonFont;
        private SpriteFont _headerFont;
        private List<TextureComponent> _contentBasedStartMenuComponents;
        private List<TextureComponent> _proceduralStartMenuComponents;
        private List<TextureComponent> _contentBasedPauseMenuComponents;

        private GraphicsDevice _graphics;
        private ContentManager _content;
        private InputManager _inputManager;
        private TMXManager _tmxManager;

        private MenuTypes _prevMenuType;

        private string _buttonText1 = "Core logic demo";
        private string _buttonText2 = "Complete demo";
        private string _buttonText3 = "Toggle procedural menu";

        public MenuController(ContentManager content, GameServiceContainer services)
        {
            _content = content;
            _graphics = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            _inputManager = services.GetService<InputManager>();
            _tmxManager = services.GetService<TMXManager>();

            MenuType = MenuTypes.ContentBased;

            LoadContent(content);

            LoadContentBasedComponents();
            LoadProceduralGeneratedComponents();
        }

        public enum MenuTypes
        {
            ContentBased = 1,
            Procedural = 2,
            PauseMenu = 3,
        }

        public MenuTypes MenuType { get; set; }
        public bool IsActive { get; set; }

        public void LoadContent(ContentManager content)
        {
            _buttonFont = content.Load<SpriteFont>("Fonts/pixel");
            _headerFont = content.Load<SpriteFont>("Fonts/seguibl");
            _startMenuBackgroundTexture = _content.Load<Texture2D>("Assets/GUI/Textures/startmenu-bg");
            _startMenuButtonTexture = _content.Load<Texture2D>("Assets/GUI/Buttons/menubutton");
        }

        public void Update(GameTime gameTime)
        {
            if (LudosGame.GameIsPaused && MenuType != MenuTypes.PauseMenu)
            {
                _prevMenuType = MenuType;
                MenuType = MenuTypes.PauseMenu;
            }
            else if (!LudosGame.GameIsPaused && MenuType == MenuTypes.PauseMenu)
            {
                MenuType = _prevMenuType;
            }

            foreach (var components in GetCurrentGuiComponents())
            {
                components.Update(gameTime);
            }
        }

        public void PostUpdate(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (MenuType == MenuTypes.ContentBased)
            {
                spriteBatch.Draw(_startMenuBackgroundTexture, Vector2.Zero, Color.White);
            }

            foreach (var components in GetCurrentGuiComponents())
            {
                components.Draw(gameTime, spriteBatch);
            }

            if (MenuType == MenuTypes.Procedural)
            {
                spriteBatch.DrawString(_headerFont, "LUDOS ENGINE", new Vector2(127, 24), new Color(70, 74, 115));
                spriteBatch.DrawString(_headerFont, "LUDOS ENGINE", new Vector2(127, 21), new Color(211, 216, 184));
            }
        }

        private void LoadContentBasedComponents()
        {
            var positionA = new Vector2(160, 98);
            var positionB = new Vector2(160, 123);
            var positionC = new Vector2(160, 148);

            var buttonA = new Button(_startMenuButtonTexture, _buttonFont, _inputManager)
            {
                Text = _buttonText1,
                Position = positionA,
                UseFontShading = true,
            };

            var buttonB = buttonA.Clone() as Button;
            buttonB.Text = _buttonText2;
            buttonB.Position = positionB;

            var buttonC = buttonA.Clone() as Button;
            buttonC.Text = _buttonText3;
            buttonC.Position = positionC;

            buttonA.Click += StartLevelOne_Click;
            buttonB.Click += StartLevelTwo_Click;
            buttonC.Click += ChangeMenuType_Click;

            _contentBasedStartMenuComponents = new List<TextureComponent>() { buttonA, buttonB, buttonC };

            var pausemenuBackground = new ProceduralTexture(_graphics, new Rectangle(0, 0, 480, 270))
            {
                TextureColors = new Color[] { Color.Black },
                Transparancy = 0.25f,
            };

            var unpauseButton = buttonA.Clone() as Button;
            unpauseButton.Click -= StartLevelOne_Click;
            unpauseButton.Click += Unpause_Click;
            unpauseButton.Text = "Continue";

            var quitButton = buttonB.Clone() as Button;
            quitButton.Click -= StartLevelTwo_Click;
            quitButton.Click += Quit_Click;
            quitButton.Text = "Quit to mainmenu";

            var exitButton = buttonC.Clone() as Button;
            exitButton.Click -= ChangeMenuType_Click;
            exitButton.Click += Exit_Click;
            exitButton.Text = "Exit game";

            _contentBasedPauseMenuComponents = new List<TextureComponent>() { pausemenuBackground, unpauseButton, quitButton, exitButton };
        }

        private void LoadProceduralGeneratedComponents()
        {
            _proceduralStartMenuComponents = new List<TextureComponent>()
            {
                new ProceduralTexture(_graphics, new Rectangle(-1, -1, 242, 137))
                {
                    TextureColors = new Color[] { new Color(61, 77, 178) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195),
                },
                new ProceduralTexture(_graphics, new Rectangle(240, -1, 241, 137))
                {
                    TextureColors = new Color[] { new Color(61, 77, 178) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195),
                },
                new ProceduralTexture(_graphics, new Rectangle(-1, 135, 242, 137))
                {
                    TextureColors = new Color[] { new Color(89, 71, 196) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195),
                },
                new ProceduralTexture(_graphics, new Rectangle(240, 135, 242, 137))
                {
                    TextureColors = new Color[] { new Color(89, 71, 196) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195),
                },
                new ProceduralTexture(_graphics, new Rectangle(146, 82, 187, 108))
                {
                    TextureColors = new Color[] { new Color(224, 230, 195), new Color(211, 216, 184), new Color(211, 216, 184), new Color(193, 198, 169) },
                    BorderWidth = 2,
                    BorderColor = new Color(70, 74, 115),
                },
            };

            var proceduralButtonA = new ProceduralButton(_graphics, _buttonFont, _inputManager, new Rectangle(160, 98, 159, 22))
            {
                Text = _buttonText1,
                UseFontShading = true,
                ButtonColor = new Color(102, 153, 204),
                BorderColor = new Color(70, 74, 115),
                BorderWidth = 2,
            };

            var proceduralButtonB = proceduralButtonA.Clone() as ProceduralButton;
            proceduralButtonB.Text = _buttonText2;
            proceduralButtonB.Position = new Vector2(160, 123);

            var proceduralButtonC = proceduralButtonA.Clone() as ProceduralButton;
            proceduralButtonC.Text = "Toggle content menu";
            proceduralButtonC.Position = new Vector2(160, 148);

            proceduralButtonA.Click += StartLevelOne_Click;
            proceduralButtonB.Click += StartLevelTwo_Click;
            proceduralButtonC.Click += ChangeMenuType_Click;

            _proceduralStartMenuComponents.AddRange(new List<TextureComponent>() { proceduralButtonA, proceduralButtonB, proceduralButtonC });
        }

        private List<TextureComponent> GetCurrentGuiComponents()
        {
            return MenuType switch
            {
                MenuTypes.ContentBased => _contentBasedStartMenuComponents,
                MenuTypes.Procedural => _proceduralStartMenuComponents,
                MenuTypes.PauseMenu => _contentBasedPauseMenuComponents,
                _ => throw new Exception("Invalid menu type"),
            };
        }

        private void StartLevelOne_Click(object sender, EventArgs e)
        {
            _tmxManager.LoadMap("Level1");
            IsActive = false;
            LudosGame.GameStates[States.Game].IsActive = true;
        }

        private void StartLevelTwo_Click(object sender, EventArgs e)
        {
            _tmxManager.LoadMap("Level3");
            IsActive = false;
            LudosGame.GameStates[States.Game].IsActive = true;
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            LudosGame.GameIsPaused = false;
            LudosGame.GameStates[States.Game].IsActive = false;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            LudosEngineDemo.DoExitGame = true;
        }

        private void Unpause_Click(object sender, EventArgs e)
        {
            LudosGame.GameIsPaused = false;
        }

        private void ChangeMenuType_Click(object sender, EventArgs e)
        {
            MenuType = MenuType == MenuTypes.ContentBased ? MenuTypes.Procedural : MenuTypes.ContentBased;
        }
    }
}
