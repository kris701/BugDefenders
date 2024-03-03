using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Screens.SettingsView
{
    public partial class SettingsView : BaseAnimatedView
    {
        private readonly List<Point> _resolutionOptions = new List<Point>()
        {
            new Point(480, 270),
            new Point(960, 540),
            new Point(1280, 720),
            new Point(1600, 900),
            new Point(1920, 1080),
            new Point(2400, 1350),
            new Point(3840, 2160),
        };
        private readonly List<ButtonControl> _scaleButtons = new List<ButtonControl>();

        private readonly List<float> _musicOptions = new List<float>()
        {
            0,
            0.1f,
            0.2f,
            0.3f,
            0.4f,
            0.5f,
            0.6f,
            0.7f,
            0.8f,
            0.9f,
            1f
        };
        private readonly List<ButtonControl> _musicButtons = new List<ButtonControl>();

        private readonly List<float> _soundEffectOptions = new List<float>()
        {
            0,
            0.1f,
            0.2f,
            0.3f,
            0.4f,
            0.5f,
            0.6f,
            0.7f,
            0.8f,
            0.9f,
            1f
        };
        private readonly List<ButtonControl> _soundEffectsButtons = new List<ButtonControl>();

        private ButtonControl _isFullScreen;
        private ButtonControl _isVSync;
        private ButtonControl _isFPSCounter;
        private readonly List<ButtonControl> _texturePacksButtons = new List<ButtonControl>();

        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = GameWindow.BaseScreenSize.X,
                Height = GameWindow.BaseScreenSize.Y
            });

            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Settings",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl()
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
                var oldSettings = Parent.CurrentUser.UserData.Copy();
                var newSettings = _settings.Copy();
                Parent.CurrentUser.UserData = newSettings;
                Parent.ApplySettings();
                SwitchView(new AcceptView.AcceptView(Parent, oldSettings, newSettings));
            })
            {
                X = 50,
                Y = 980,
                Width = 200,
                Height = 50,
                Text = "Apply",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 980,
                X = 1670,
                Width = 200,
                Height = 50,
                Text = "Cancel",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "UI Scale",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _resolutionOptions.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is Point value)
                    {
                        _settings.ScreenWidth = value.X;
                        _settings.ScreenHeight = value.Y;
                    }
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 35,
                    X = 110 + (i * (1585 / _resolutionOptions.Count + 10)),
                    Width = 1585 / _resolutionOptions.Count,
                    Height = 40,
                    Text = $"{_resolutionOptions[i].X}x{_resolutionOptions[i].Y}",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _resolutionOptions[i]
                };
                AddControl(1, newControl);
                _scaleButtons.Add(newControl);
            }

            AddControl(1, new LabelControl()
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
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isVSync);
            _isFPSCounter = new ButtonControl(Parent, clicked: (x) =>
            {
                _settings.FPSCounter = !_settings.FPSCounter;
                UpdateScreenSettingsButtons();
            })
            {
                Y = yOffset + 110,
                X = 530,
                Width = 200,
                Height = 50,
                Text = "FPS Counter",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isFPSCounter);

            UpdateScreenSettingsButtons();
        }

        private void SetupTextureSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "Texture packs",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            var packs = Parent.UIResources.GetTexturePacks();
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
                    X = 110 + (i * (1585 / packs.Count + 10)),
                    Width = 1585 / packs.Count,
                    Height = 50,
                    Text = Parent.UIResources.GetTexturePack(packs[i]).Name,
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = packs[i]
                };
                AddControl(1, newControl);
                _texturePacksButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }

        private void SetupMusicSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "Music Volume",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _musicOptions.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is float value)
                        _settings.MusicVolume = value;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 35,
                    X = 110 + (i * (1585 / _musicOptions.Count + 10)),
                    Width = 1585 / _musicOptions.Count,
                    Height = 40,
                    Text = $"{Math.Round(_musicOptions[i] * 100, 0)}%",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _musicOptions[i]
                };
                AddControl(1, newControl);
                _musicButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }

        private void SetupSoundEffectsSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = Alignment.Middle,
                Y = yOffset,
                Text = "Sound Effects Volume",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _soundEffectOptions.Count; i++)
            {
                var newControl = new ButtonControl(Parent, clicked: (s) =>
                {
                    if (s.Tag is float value)
                        _settings.EffectsVolume = value;
                    UpdateScreenSettingsButtons();
                })
                {
                    Y = yOffset + 35,
                    X = 110 + (i * (1585 / _soundEffectOptions.Count + 10)),
                    Width = 1585 / _soundEffectOptions.Count,
                    Height = 40,
                    Text = $"{Math.Round(_soundEffectOptions[i] * 100, 0)}%",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _soundEffectOptions[i]
                };
                AddControl(1, newControl);
                _soundEffectsButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }
    }
}
