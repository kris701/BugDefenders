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

namespace TDGame.OpenGL.Screens.SettingsView
{
    public partial class SettingsView : BaseScreen
    {
        private ButtonControl _scaleButtonOne;
        private ButtonControl _scaleButtonTwo;
        private ButtonControl _scaleButtonThree;

        private ButtonControl _isFullScreen;
        private ButtonControl _isVSync;

        public override void Initialize()
        {
#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new SettingsView(Parent)))
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

            AddControl(0, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                Text = "Settings",
                Font = BasicFonts.GetFont(72),
            });

            SetupScreenSettingsView(200);

            AddControl(0, new ButtonControl(this, clicked: (x) => {
                Parent.Settings = _settings;
                Parent.ApplySettings();
                Parent.SaveSettings();
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                Width = 200,
                Height = 50,
                Text = "Apply",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });

            AddControl(0, new ButtonControl(this, clicked: (x) => {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                X = 800,
                Width = 200,
                Height = 50,
                Text = "Cancel",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
            base.Initialize();
        }

        private void SetupScreenSettingsView(int yOffset)
        {
            AddControl(1, new TileControl(this)
            {
                X = 100,
                Y = yOffset,
                Width = 800,
                Height = 350,
                FillColor = BasicTextures.GetBasicRectange(Color.Chartreuse)
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Width = 800,
                Y = yOffset,
                Text = "Screen",
                Font = BasicFonts.GetFont(48),
                FillColor = BasicTextures.GetBasicRectange(Color.Red)
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset + 75,
                Text = "Scale",
                Font = BasicFonts.GetFont(24),
            });

            _scaleButtonOne = new ButtonControl(this, clicked: (x) =>
            {
                _settings.Scale = 0.5f;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 125,
                X = 150,
                Width = 200,
                Height = 50,
                Text = "50%",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            };
            AddControl(1, _scaleButtonOne);
            _scaleButtonTwo = new ButtonControl(this, clicked: (x) =>
            {
                _settings.Scale = 1f;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 125,
                X = 400,
                Width = 200,
                Height = 50,
                Text = "100%",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            };
            AddControl(1, _scaleButtonTwo);
            _scaleButtonThree = new ButtonControl(this, clicked: (x) =>
            {
                _settings.Scale = 2f;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 125,
                X = 650,
                Width = 200,
                Height = 50,
                Text = "200%",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            };
            AddControl(1, _scaleButtonThree);

            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset + 200,
                Text = "Other",
                Font = BasicFonts.GetFont(24),
            });

            _isFullScreen = new ButtonControl(this, clicked: (x) =>
            {
                _settings.IsFullscreen = !_settings.IsFullscreen;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 250,
                X = 150,
                Width = 200,
                Height = 50,
                Text = "Fullscreen",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            };
            AddControl(1, _isFullScreen);
            _isVSync = new ButtonControl(this, clicked: (x) =>
            {
                _settings.IsVsync = !_settings.IsVsync;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 250,
                X = 400,
                Width = 200,
                Height = 50,
                Text = "VSync",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            };
            AddControl(1, _isVSync);

            UpdateScreenSettingsButtons();
        }
    }
}
