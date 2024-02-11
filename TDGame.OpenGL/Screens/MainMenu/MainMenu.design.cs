using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Screens.GameSetupView;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public override void Initialize()
        {
#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new MainMenu(Parent)))
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

            AddControl(0, new TileControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                FillColor = TextureBuilder.GetTexture(new Guid("960c3e27-bfa4-40db-8397-ce47655eb169")),
                Rotation = 1
            });
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new GameSetupView.GameSetupView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 300,
                Width = 200,
                Height = 50,
                Text = "Start Game",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new SettingsView.SettingsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 375,
                Width = 200,
                Height = 50,
                Text = "Settings",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
            AddControl(0, new ButtonControl(this, clicked: (x) => Parent.Exit())
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 550,
                Width = 200,
                Height = 50,
                Text = "Exit",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
            base.Initialize();
        }
    }
}
