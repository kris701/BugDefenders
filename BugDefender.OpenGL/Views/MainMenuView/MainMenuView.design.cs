using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.MainMenu
{
    public partial class MainMenuView : BaseBugDefenderView
    {
        private BugDefenderButtonControl _startGameButton;
        private BugDefenderButtonControl _continueButton;
        private TextInputControl _cheatsInput;

        [MemberNotNull(nameof(_startGameButton), nameof(_continueButton), nameof(_cheatsInput))]
        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = IWindow.BaseScreenSize.X,
                Height = IWindow.BaseScreenSize.Y
            });
            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 1050,
                Font = BasicFonts.GetFont(12),
                Text = "Copyright Kristian Skov Johansen",
                FontColor = Color.White,
            });
            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 200,
                Font = BasicFonts.GetFont(72),
                Text = "Bug Defenders",
                FontColor = Color.White,
            });
            _continueButton = new BugDefenderButtonControl(Parent, ContinueGame)
            {
                X = 960 - 300,
                Y = 300,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Continue Game",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture(),
                IsVisible = false
            };
            AddControl(0, _continueButton);
            _startGameButton = new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new GameSetupView.GameSetupView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 300,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Start Game",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            };
            AddControl(0, _startGameButton);
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new SettingsView.SettingsView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 350,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Settings",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new HighScoresView.HighScoresView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 400,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "High Scores",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new PermaBuffsView.PermaBuffsView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 450,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Buffs",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new AchivementsView.AchivementsView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 500,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Achivements",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new ChallengeView.ChallengeView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 550,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Challenges",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => (Parent as Game).Exit())
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 600,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Exit",
                FontColor = Color.Red,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });

            AddControl(0, new LabelControl()
            {
                X = 250,
                Y = 980,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                Text = $"User: {Parent.UserManager.CurrentUser.Name}",
                FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"))
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new UsersScreen.UsersScreenView(Parent)))
            {
                X = 50,
                Y = 980,
                Width = 200,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                Text = "Users",
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => LinkHelper.OpenUrl("https://kris701.itch.io/bug-defenders"))
            {
                X = 1700,
                Y = 980,
                Width = 200,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                Text = "Itch IO",
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

            _cheatsInput = new BugDefendersTextInputControl(Parent, OnEnterCheat)
            {
                Width = 800,
                Height = 100,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.TextureController.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Limit = 50,
                IsVisible = false,
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 700
            };
            AddControl(1, _cheatsInput);

#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new MainMenuView(Parent)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = BasicFonts.GetFont(10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif
            base.Initialize();
        }
    }
}
