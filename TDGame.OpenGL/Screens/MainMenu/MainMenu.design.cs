﻿using Microsoft.Xna.Framework;
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
            AddControl(0, new ButtonControl(Parent, clicked: (x) => Parent.Exit())
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 500,
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
                Height = 35,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"Current User: {Parent.CurrentUser.Name}",
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
            });
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new UsersScreen.UsersScreen(Parent)))
            {
                X = 50,
                Y = 905,
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
