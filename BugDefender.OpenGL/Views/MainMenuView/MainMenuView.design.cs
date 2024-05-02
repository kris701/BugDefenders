using BugDefender.OpenGL.Controls;
using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.MainMenu
{
    public partial class MainMenuView : BaseBugDefenderView
    {
        private TextInputControl _cheatsInput;

        [MemberNotNull(nameof(_cheatsInput))]
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
            AddControl(0, new BugDefenderButtonControl(Parent, (x) => SwitchView(new LoadGameView.LoadGameView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 300,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Load Game",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, (x) => SwitchView(new CampaignGameSetupView.CampaignGameSetupView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 350,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "New Campaign Game",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, (x) => SwitchView(new SurvivalGameSetupView.SurvivalGameSetupView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 400,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "New Survival Game",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new SettingsView.SettingsView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 450,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Settings",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new HighScoresView.HighScoresView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 500,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "High Scores",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new PermaBuffsView.PermaBuffsView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 550,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Buffs",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new AchivementsView.AchivementsView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 600,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Achivements",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new ChallengeView.ChallengeView(Parent)))
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 650,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Challenges",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => (Parent as Game).Exit())
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 700,
                Width = 400,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Exit",
                FontColor = Color.Red,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });

            var userButton = BasicMenuHelper.GetCancelButton(Parent, $"User: {Parent.UserManager.CurrentUser.Name}", (e) => { SwitchView(new UsersScreen.UsersScreenView(Parent)); });
            userButton.Width = 400;
            userButton.Font = BasicFonts.GetFont(16);
            userButton.FillColor = Parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            userButton.FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8"));
            AddControl(0, userButton);
            AddControl(0, BasicMenuHelper.GetAcceptButton(Parent, "Itch IO", (x) => LinkHelper.OpenUrl("https://kris701.itch.io/bug-defenders")));

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
