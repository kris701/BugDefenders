﻿using Microsoft.Xna.Framework;
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
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureBuilder.GetTexture(new Guid("4a39d624-3171-41cd-b172-c853cea36d14")),
                Width = 1000,
                Height = 1000
            });
            AddControl(0, new TileControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                FillColor = TextureBuilder.GetTexture(new Guid("960c3e27-bfa4-40db-8397-ce47655eb169"))
            });
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new GameSetupView.GameSetupView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 350,
                Width = 300,
                Height = 100,
                FillColor = TextureBuilder.GetTexture(new Guid("c89588ae-a69f-41bf-a9df-e4fc53071c0b")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new SettingsView.SettingsView(Parent)))
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 475,
                Width = 300,
                Height = 100,
                FillColor = TextureBuilder.GetTexture(new Guid("40aa2d8a-7509-4bef-bf1c-9c3e1dffdd08")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });
            AddControl(0, new ButtonControl(this, clicked: (x) => Parent.Exit())
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 600,
                Width = 300,
                Height = 100,
                FillColor = TextureBuilder.GetTexture(new Guid("e8fd55a0-ea74-48d8-b625-4352d6f1c564")),
                FillClickedColor = BasicTextures.GetClickedTexture()
            });


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
            base.Initialize();
        }
    }
}
