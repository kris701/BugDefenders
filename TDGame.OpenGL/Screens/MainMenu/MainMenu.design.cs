using Microsoft.Xna.Framework;
using System;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("4a39d624-3171-41cd-b172-c853cea36d14")),
                Width = 1000,
                Height = 1000
            });
            AddControl(0, new TileControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                FillColor = TextureManager.GetTexture(new Guid("960c3e27-bfa4-40db-8397-ce47655eb169"))
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameSetupView.GameSetupView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 350,
                Width = 300,
                Height = 100,
                FillColor = TextureManager.GetTexture(new Guid("c89588ae-a69f-41bf-a9df-e4fc53071c0b")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new SettingsView.SettingsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 475,
                Width = 300,
                Height = 100,
                FillColor = TextureManager.GetTexture(new Guid("40aa2d8a-7509-4bef-bf1c-9c3e1dffdd08")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new HighScoresView.HighScoresView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 600,
                Width = 300,
                Height = 100,
                FillColor = TextureManager.GetTexture(new Guid("410d9075-6e22-4ff7-9aff-1dcc3be9cd42")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new PermaBuffsView.PermaBuffsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 725,
                Width = 300,
                Height = 100,
                FillColor = TextureManager.GetTexture(new Guid("66038205-f63a-4f3d-a057-d2ad99496daa")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            bool claimable = false;
            var buffs = ResourceManager.Buffs.GetResources();
            foreach(var id in buffs)
            {
                if (Parent.CurrentUser.Buffs.Contains(id))
                    continue;
                var buff = ResourceManager.Buffs.GetResource(id);
                if (buff.IsValid(Parent.CurrentUser))
                {
                    claimable = true;
                    break;
                }
            }
            if (claimable)
            {
                AddControl(0, new TileControl(Parent)
                {
                    X = 375,
                    Y = 760,
                    Width = 25,
                    Height = 25,
                    FillColor = BasicTextures.GetBasicCircle(Color.Yellow, 20),
                });
            }
            AddControl(0, new ButtonControl(Parent, clicked: (x) => Parent.Exit())
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 850,
                Width = 300,
                Height = 100,
                FillColor = TextureManager.GetTexture(new Guid("e8fd55a0-ea74-48d8-b625-4352d6f1c564")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });

            AddControl(0, new LabelControl(Parent)
            {
                X = 5,
                Y = 900,
                Width = 200,
                Height = 35,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"Current User: {Parent.CurrentUser.Name}",
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new UsersScreen.UsersScreen(Parent)))
            {
                X = 5,
                Y = 945,
                Width = 200,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                Text = "Users",
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new MainMenu(Parent)))
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
