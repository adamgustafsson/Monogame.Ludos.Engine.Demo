using Ludos.Engine.Graphics;
using Ludos.Engine.Managers;
using Ludos.Engine.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LudosEngineDemo
{
    public class MenuController : GameState
    {
        private Texture2D _startMenuBackgroundTexture;
        private SpriteFont _buttonFont;
        private SpriteFont _headerFont;
        private List<GUIComponent> _contentBasedStartMenuComponents;
        private List<GUIComponent> _proceduralStartMenuComponents;
        private List<GUIComponent> _contentBasedPauseMenuComponents;
        private string _buttonText1 = "Core logic demo";
        private string _buttonText2 = "Complete demo";
        private string _buttonText3 = "Toggle procedural menu";

        public MenuTypes MenuType { get; set; }

        public enum MenuTypes
        {
            ContentBased = 1,
            Procedural = 2,
            PauseMenu = 3
        }

        public MenuController(LudosGame game, GraphicsDevice graphicsDevice, ContentManager content, InputManager inputManager)
            : base(game, graphicsDevice, content, inputManager) 
        {
            MenuType = MenuTypes.ContentBased;  
            _buttonFont = content.Load<SpriteFont>("Fonts/pixel");
            _headerFont = content.Load<SpriteFont>("Fonts/seguibl");

            LoadContentBasedComponents();
            LoadProceduralGeneratedComponents();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var components in GetCurrentGuiComponents())
            {
                components.Update(gameTime);
            }

        }
        public override void PostUpdate(GameTime gameTime)
        {

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (MenuType == MenuTypes.ContentBased)
                spriteBatch.Draw(_startMenuBackgroundTexture, Vector2.Zero, Color.White);
            
            foreach (var components in GetCurrentGuiComponents()) 
                components.Draw(gameTime, spriteBatch);

            if (MenuType == MenuTypes.Procedural)
            {
                spriteBatch.DrawString(_headerFont, "LUDOS ENGINE", new Vector2(127, 24), new Color(70, 74, 115));
                spriteBatch.DrawString(_headerFont, "LUDOS ENGINE", new Vector2(127, 21), new Color(211, 216, 184));
            }

        }

        private void LoadContentBasedComponents()
        {
            _startMenuBackgroundTexture = Content.Load<Texture2D>("Assets/GUI/Textures/startmenu-bg");
            var startMenuButtonTexture = Content.Load<Texture2D>("Assets/GUI/Buttons/menubutton");

            var APosition = new Vector2(160, 98);
            var BPosition = new Vector2(160, 123);
            var CPosition = new Vector2(160, 148);

            var buttonA = new Button(startMenuButtonTexture, _buttonFont, InputManager)
            {
                Text = _buttonText1,
                Position = APosition,
                UseFontShading = true
            };

            var buttonB = (buttonA.Clone() as Button);
            buttonB.Text = _buttonText2;
            buttonB.Position = BPosition;

            var buttonC = (buttonA.Clone() as Button);
            buttonC.Text = _buttonText3;
            buttonC.Position = CPosition;

            buttonA.Click += StartLevelOne_Click;
            buttonB.Click += StartLevelTwo_Click;
            buttonC.Click += ChangeMenuType_Click;

            _contentBasedStartMenuComponents = new List<GUIComponent>() { buttonA, buttonB, buttonC };


            var pausemenuBackground = new ProceduralTexture(Graphics, new Rectangle(0, 0, 480, 270))
            {
                TextureColors = new Color[] { Color.Black },
                Transparancy = 0.25f
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

            _contentBasedPauseMenuComponents = new List<GUIComponent>() { pausemenuBackground, unpauseButton, quitButton, exitButton };
            //LoadPauseMenuComponents();
        }

        private void LoadProceduralGeneratedComponents()
        {
            _proceduralStartMenuComponents = new List<GUIComponent>() {
                new ProceduralTexture(Graphics, new Rectangle(-1, -1, 242, 137)) {
                    TextureColors = new Color[] { new Color(61, 77, 178) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195)
                },
                new ProceduralTexture(Graphics, new Rectangle(240, -1, 241, 137)) {
                    TextureColors = new Color[] { new Color(61, 77, 178) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195)
                },
                new ProceduralTexture(Graphics, new Rectangle(-1, 135, 242, 137)) {
                    TextureColors = new Color[] { new Color(89, 71, 196) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195)
                },
                new ProceduralTexture(Graphics, new Rectangle(240, 135, 242, 137)) {
                    TextureColors = new Color[] { new Color(89, 71, 196) },
                    BorderWidth = 1,
                    BorderColor = new Color(224, 230, 195)
                },
                new ProceduralTexture(Graphics, new Rectangle(146, 82, 187, 108)) {
                    TextureColors = new Color[] {new Color(224, 230, 195), new Color(211, 216, 184), new Color(211, 216, 184), new Color(193, 198, 169)},
                    BorderWidth = 2,
                    BorderColor = new Color(70, 74, 115)
                }
            };

            var prButtonA = new ProceduralButton(Graphics, _buttonFont, InputManager, new Rectangle(160, 98, 159, 22))
            {
                Text = _buttonText1,
                UseFontShading = true,
                ButtonColor = new Color(102, 153, 204),
                BorderColor = new Color(70, 74, 115),
                BorderWidth = 2
            };

            var prButtonB = (prButtonA.Clone() as ProceduralButton);
            prButtonB.Text = _buttonText2;
            prButtonB.Position = new Vector2(160, 123);

            var prButtonC = (prButtonA.Clone() as ProceduralButton);
            prButtonC.Text = "Toggle content menu";
            prButtonC.Position = new Vector2(160, 148);

            prButtonA.Click += StartLevelOne_Click;
            prButtonB.Click += StartLevelTwo_Click;
            prButtonC.Click += ChangeMenuType_Click;

            _proceduralStartMenuComponents.AddRange(new List<GUIComponent>() { prButtonA, prButtonB, prButtonC });
        }

        private List<GUIComponent> GetCurrentGuiComponents()
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
            Game.LoadMap("Level1");
            Game.ChangeState(States.Game);
        }

        private void StartLevelTwo_Click(object sender, EventArgs e)
        {
            Game.LoadMap("Level2");
            Game.ChangeState(States.Game);
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Game.GameIsPaused = false;
            Game.ChangeState(States.Menu);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void Unpause_Click(object sender, EventArgs e)
        {
            Game.GameIsPaused = false;
        }

        private void ChangeMenuType_Click(object sender, EventArgs e)
        {
            MenuType = MenuType == MenuTypes.ContentBased ? MenuTypes.Procedural : MenuTypes.ContentBased;
        }
    }
}
