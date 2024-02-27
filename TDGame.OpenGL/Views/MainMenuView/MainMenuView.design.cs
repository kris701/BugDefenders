using Microsoft.Xna.Framework;
using System;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Views;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenuView : BaseView
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("4a39d624-3171-41cd-b172-c853cea36d14")),
                Width = 1000,
                Height = 1000
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 200,
                Font = BasicFonts.GetFont(72),
                Text = "TD Game",
                FontColor = Color.White,
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameSetupView.GameSetupView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 300,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Start Game",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new SettingsView.SettingsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 350,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Settings",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new HighScoresView.HighScoresView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 400,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "High Scores",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new PermaBuffsView.PermaBuffsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 450,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Buffs",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new AchivementsView.AchivementsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 500,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Achivements",
                FontColor = Color.White,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => Parent.Exit())
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 550,
                Width = 300,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                Text = "Exit",
                FontColor = Color.Red,
                FillClickedColor = BasicTextures.GetClickedTexture()
            });

            AddControl(0, new LabelControl(Parent)
            {
                X = 50,
                Y = 860,
                Width = 200,
                Height = 50,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"Current User: {Parent.CurrentUser.Name}",
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9"))
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new UsersScreen.UsersScreenView(Parent)))
            {
                X = 50,
                Y = 905,
                Width = 200,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                Text = "Users",
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new MainMenuView(Parent)))
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
