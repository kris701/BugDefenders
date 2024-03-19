using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.SettingsView
{
    public partial class SettingsView : BaseBugDefenderView
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
        private readonly List<BugDefenderButtonControl> _scaleButtons = new List<BugDefenderButtonControl>();

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
        private readonly List<BugDefenderButtonControl> _musicButtons = new List<BugDefenderButtonControl>();

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
        private readonly List<BugDefenderButtonControl> _soundEffectsButtons = new List<BugDefenderButtonControl>();

        private BugDefenderButtonControl _isFullScreen;
        private BugDefenderButtonControl _isVSync;
        private BugDefenderButtonControl _isFPSCounter;
        private readonly List<BugDefenderButtonControl> _texturePacksButtons = new List<BugDefenderButtonControl>();

        [MemberNotNull(nameof(_isFullScreen), nameof(_isVSync), nameof(_isFPSCounter))]
        public override void Initialize()
        {
            BasicMenuHelper.GenerateBaseMenu(
                this,
                Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Settings",
                "Game settings. Each user have their own settings.");

            SetupScreenSettingsView(220);
            SetupTextureSettingsView(390);
            SetupMusicSettingsView(500);
            SetupSoundEffectsSettingsView(600);

            AddControl(0, BasicMenuHelper.GetAcceptButton(Parent, "Apply", (x) =>
            {
                var oldSettings = Parent.UserManager.CurrentUser.UserData.Copy();
                var newSettings = _settings.Copy();
                Parent.UserManager.CurrentUser.UserData = newSettings;
                Parent.ApplySettings();
                SwitchView(new AcceptView.AcceptView(Parent, oldSettings, newSettings));
            }));
            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Back", (e) => { SwitchView(new MainMenu.MainMenuView(Parent)); }));

#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new SettingsView(Parent)))
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

        [MemberNotNull(nameof(_isFullScreen), nameof(_isVSync), nameof(_isFPSCounter))]
        private void SetupScreenSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = yOffset + 100,
                Text = "Other",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            _isFullScreen = new BugDefenderButtonControl(Parent, clicked: (x) =>
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
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isFullScreen);
            _isVSync = new BugDefenderButtonControl(Parent, clicked: (x) =>
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
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isVSync);
            _isFPSCounter = new BugDefenderButtonControl(Parent, clicked: (x) =>
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
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            };
            AddControl(1, _isFPSCounter);

            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = yOffset + 10,
                Text = "UI Scale",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _resolutionOptions.Count; i++)
            {
                var newControl = new BugDefenderButtonControl(Parent, clicked: (s) =>
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
                    FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    FillDisabledColor = Parent.TextureController.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                    Tag = _resolutionOptions[i]
                };
                AddControl(1, newControl);
                _scaleButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }

        private void SetupTextureSettingsView(int yOffset)
        {
            AddControl(1, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = yOffset,
                Text = "Texture packs",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            var packs = Parent.ResourcePackController.GetResourcePacks();
            for (int i = 0; i < packs.Count; i++)
            {
                var newControl = new BugDefenderButtonControl(Parent, clicked: (s) =>
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
                    Text = Parent.ResourcePackController.GetResourcePack(packs[i]).Name,
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = yOffset,
                Text = "Music Volume",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _musicOptions.Count; i++)
            {
                var newControl = new BugDefenderButtonControl(Parent, clicked: (s) =>
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
                    FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = yOffset,
                Text = "Sound Effects Volume",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White
            });

            for (int i = 0; i < _soundEffectOptions.Count; i++)
            {
                var newControl = new BugDefenderButtonControl(Parent, clicked: (s) =>
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
                    FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    Tag = _soundEffectOptions[i]
                };
                AddControl(1, newControl);
                _soundEffectsButtons.Add(newControl);
            }

            UpdateScreenSettingsButtons();
        }
    }
}
