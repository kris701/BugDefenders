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

namespace TDGame.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverScreen : BaseScreen
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureManager.GetTexture(new Guid("1d50ad58-0503-4fc6-95e4-1d18abb8c485")),
                Width = 1000,
                Height = 1000
            });

            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                Text = "Game Over!",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(72),
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 225,
                Text = $"Score: {_score}",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(24),
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 275,
                Text = $"Played for {_gameTime.ToString("hh\\:mm\\:ss")}",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(24),
            });

            AddControl(1, new BorderControl(this)
            {
                Thickness = 5,
                Child = new TileControl(this)
                {
                    HorizontalAlignment = Alignment.Middle,
                    Y = 350,
                    Width = 500,
                    Height = 500,
                    FillColor = _screen
                }
            });

            AddControl(1, new BorderControl(this)
            {
                Child = new ButtonControl(this, clicked: (s) => { SwitchView(new MainMenu.MainMenu(Parent)); })
                {
                    HorizontalAlignment = Alignment.Middle,
                    Y = 875,
                    Width = 400,
                    Height = 75,
                    Font = BasicFonts.GetFont(24),
                    Text = "Main Menu",
                    FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                    FillClickedColor = BasicTextures.GetClickedTexture(),
                    FillDisabledColor = BasicTextures.GetDisabledTexture(),
                }
            });

#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new GameOverScreen(Parent, _screen, _score, _gameTime)))
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
