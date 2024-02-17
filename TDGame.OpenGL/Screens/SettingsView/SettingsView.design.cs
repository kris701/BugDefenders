using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.SettingsView
{
    public partial class SettingsView : BaseScreen
    {
        private List<float> _scaleOptions = new List<float>()
        {
            0.25f,
            0.50f,
            0.75f,
            1f,
            1.25f,
            1.5f,
            1.75f
        };
        private List<ButtonControl> _scaleButtons = new List<ButtonControl>();

        private ButtonControl _isFullScreen;
        private ButtonControl _isVSync;
        private List<ButtonControl> _texturePacksButtons = new List<ButtonControl>();

        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureManager.GetTexture(new Guid("0739c674-5f0e-497a-a619-8ba39fd545b3")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 75,
                Text = "Settings",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(72),
            });

            SetupScreenSettingsView(200);
            SetupTextureSettingsView(560);

            AddControl(0, new ButtonControl(this, clicked: (x) =>
            {
                Parent.Settings = _settings;
                Parent.ApplySettings();
                Parent.SaveUserData(Parent.CurrentUser);
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                Width = 200,
                Height = 50,
                Text = "Apply",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
            });

            AddControl(0, new ButtonControl(this, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                X = 800,
                Width = 200,
                Height = 50,
                Text = "Cancel",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
            });

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
                FillColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Width = 800,
                Y = yOffset,
                Text = "Screen",
                Font = BasicFonts.GetFont(48),
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray)
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset + 75,
                Text = "Scale",
                Font = BasicFonts.GetFont(24),
            });

            for (int i = 0; i < _scaleOptions.Count; i++)
            {
                var newControl = new ButtonControl(this, clicked: (s) =>
                {
                    if (s.Tag is float value)
                        _settings.Scale = value;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 125,
                    X = 110 + (i * (710 / _scaleOptions.Count + 10)),
                    Width = 710 / _scaleOptions.Count,
                    Height = 50,
                    Text = $"{Math.Round(_scaleOptions[i] * 100, 0)}%",
                    Font = BasicFonts.GetFont(16),
                    FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                    FillClickedColor = BasicTextures.GetClickedTexture(),
                    Tag = _scaleOptions[i]
                };
                AddControl(1, newControl);
                _scaleButtons.Add(newControl);
            }

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
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
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
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
            };
            AddControl(1, _isVSync);

            UpdateScreenSettingsButtons();
        }

        private void SetupTextureSettingsView(int yOffset)
        {
            AddControl(1, new TileControl(this)
            {
                X = 100,
                Y = yOffset,
                Width = 800,
                Height = 200,
                FillColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
            AddControl(1, new LabelControl(this)
            {
                HorizontalAlignment = Alignment.Middle,
                Width = 800,
                Y = yOffset,
                Text = "Textures",
                Font = BasicFonts.GetFont(48),
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray)
            });

            var packs = TextureManager.GetTexturePacks();
            for (int i = 0; i < packs.Count; i++)
            {
                var newControl = new ButtonControl(this, clicked: (s) =>
                {
                    if (s.Tag is Guid str)
                        _settings.TexturePack = str;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 125,
                    X = 110 + (i * (770 / packs.Count + 10)),
                    Width = 770 / packs.Count,
                    Height = 50,
                    Text = TextureManager.GetTexturePack(packs[i]).Name,
                    Font = BasicFonts.GetFont(16),
                    FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                    FillClickedColor = BasicTextures.GetClickedTexture(),
                    Tag = packs[i]
                };
                AddControl(1, newControl);
                _texturePacksButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }
    }
}
