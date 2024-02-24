﻿using Microsoft.Xna.Framework;
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

        private List<float> _soundOptions = new List<float>()
        {
            0,
            0.2f,
            0.4f,
            0.6f,
            0.8f,
            1f
        };
        private List<ButtonControl> _musicButtons = new List<ButtonControl>();
        private List<ButtonControl> _soundEffectsButtons = new List<ButtonControl>();

        private ButtonControl _isFullScreen;
        private ButtonControl _isVSync;
        private List<ButtonControl> _texturePacksButtons = new List<ButtonControl>();

        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = UIResourceManager.GetTexture(new Guid("0739c674-5f0e-497a-a619-8ba39fd545b3")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Settings",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Game settings. Each user have their own settings.",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            SetupScreenSettingsView(220);
            SetupTextureSettingsView(390);
            SetupMusicSettingsView(500);
            SetupSoundEffectsSettingsView(600);

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                Parent.CurrentUser.UserData = _settings;
                Parent.ApplySettings();
                Parent.UserManager.SaveUser(Parent.CurrentUser);
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                X = 50,
                Y = 900,
                Width = 200,
                Height = 50,
                Text = "Apply",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Cancel",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new SettingsView(Parent)))
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
            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "UI Scale",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _scaleOptions.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is float value)
                        _settings.Scale = value;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 35,
                    X = 110 + (i * (710 / _scaleOptions.Count + 10)),
                    Width = 710 / _scaleOptions.Count,
                    Height = 40,
                    Text = $"{Math.Round(_scaleOptions[i] * 100, 0)}%",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _scaleOptions[i]
                };
                AddControl(1, newControl);
                _scaleButtons.Add(newControl);
            }

            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset + 80,
                Text = "Other",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            _isFullScreen = new ButtonControl(Parent, clicked: (x) =>
            {
                _settings.IsFullscreen = !_settings.IsFullscreen;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 110,
                X = 110,
                Width = 200,
                Height = 50,
                Text = "Fullscreen",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isFullScreen);
            _isVSync = new ButtonControl(Parent, clicked: (x) =>
            {
                _settings.IsVsync = !_settings.IsVsync;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 110,
                X = 320,
                Width = 200,
                Height = 50,
                Text = "VSync",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isVSync);

            UpdateScreenSettingsButtons();
        }

        private void SetupTextureSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "Texture packs",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            var packs = UIResourceManager.GetTexturePacks();
            for (int i = 0; i < packs.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is Guid str)
                        _settings.TexturePack = str;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 40,
                    X = 110 + (i * (770 / packs.Count + 10)),
                    Width = 770 / packs.Count,
                    Height = 50,
                    Text = UIResourceManager.GetTexturePack(packs[i]).Name,
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = packs[i]
                };
                AddControl(1, newControl);
                _texturePacksButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }

        private void SetupMusicSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "Music Volume",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _soundOptions.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is float value)
                        _settings.MusicVolume = value;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 35,
                    X = 110 + (i * (710 / _soundOptions.Count + 10)),
                    Width = 710 / _soundOptions.Count,
                    Height = 40,
                    Text = $"{Math.Round(_soundOptions[i] * 100, 0)}%",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _soundOptions[i]
                };
                AddControl(1, newControl);
                _musicButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }

        private void SetupSoundEffectsSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "Sound Effects Volume",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _soundOptions.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is float value)
                        _settings.EffectsVolume = value;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 35,
                    X = 110 + (i * (710 / _soundOptions.Count + 10)),
                    Width = 710 / _soundOptions.Count,
                    Height = 40,
                    Text = $"{Math.Round(_soundOptions[i] * 100, 0)}%",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _soundOptions[i]
                };
                AddControl(1, newControl);
                _soundEffectsButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }
    }
}
