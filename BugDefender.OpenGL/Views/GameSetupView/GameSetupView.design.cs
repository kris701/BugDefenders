﻿using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Screens.GameSetupView
{
    public partial class GameSetupView : BaseAnimatedView
    {
        private TileControl _mapPreviewTile;
        private LabelControl _mapNameLabel;
        private TextboxControl _mapDescriptionTextbox;
        private ButtonControl _startButton;

        private readonly int _selectionsPrPage = 5;
        private readonly List<List<ButtonControl>> _mapPages = new List<List<ButtonControl>>();
        private int _currentMapPage = 0;
        private readonly List<List<ButtonControl>> _gameStylePages = new List<List<ButtonControl>>();
        private int _currentGameStylePage = 0;

        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Game Setup",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Select a map and a gamemode to start.  ",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            SetupPreviewPanel();
            SetupMapsView();
            SetupGameStyleView();

            _startButton = new ButtonControl(Parent, StartButton_Click)
            {
                X = 50,
                Y = 900,
                Width = 200,
                Height = 50,
                Text = "Start",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                IsEnabled = false
            };
            AddControl(0, _startButton);
            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameSetupView(Parent)))
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

        private void SetupPreviewPanel()
        {
            AddControl(1, new TileControl(Parent)
            {
                X = 50,
                Y = 210,
                Height = 350,
                Width = 900,
                FillColor = Parent.UIResources.GetTexture(new Guid("02f8c9e2-e4c0-4310-934a-62c84cbb7384")),
            });
            _mapPreviewTile = new TileControl(Parent)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Black),
                X = 75,
                Y = 235,
                Width = 300,
                Height = 300,
            };
            AddControl(1, new BorderControl(Parent, _mapPreviewTile)
            {
                Thickness = 3,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue)
            });

            _mapNameLabel = new LabelControl(Parent)
            {
                Text = "Select A Map",
                Font = BasicFonts.GetFont(16),
                X = 450,
                Y = 220,
                Height = 50,
                Width = 400,
                FontColor = Color.White,
            };
            AddControl(1, _mapNameLabel);
            _mapDescriptionTextbox = new TextboxControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                X = 380,
                Y = 270,
                Height = 250,
                Width = 550,
                FontColor = Color.White
            };
            AddControl(1, _mapDescriptionTextbox);
        }

        private void SetupMapsView()
        {
            AddControl(1, new TileControl(Parent)
            {
                X = 50,
                Y = 550,
                Height = 350,
                Width = 440,
                FillColor = Parent.UIResources.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(24),
                Text = "Maps",
                X = 80,
                Y = 560,
                Height = 50,
                Width = 400,
                FontColor = Color.White
            });
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentMapPage--;
                if (_currentMapPage < 0)
                    _currentMapPage = 0;
                if (_currentMapPage >= _mapPages.Count)
                    _currentMapPage = _mapPages.Count - 1;
                UpdateMapSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $"<",
                X = 70,
                Y = 560,
                Height = 50,
                Width = 50,
                IsVisible = ResourceManager.Maps.GetResources().Count > _selectionsPrPage
            });
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentMapPage++;
                if (_currentMapPage < 0)
                    _currentMapPage = 0;
                if (_currentMapPage >= _mapPages.Count)
                    _currentMapPage = _mapPages.Count - 1;
                UpdateMapSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $">",
                X = 420,
                Y = 560,
                Height = 50,
                Width = 50,
                IsVisible = ResourceManager.Maps.GetResources().Count > _selectionsPrPage
            });

            int count = 0;
            int page = -1;
            int offset = 0;
            foreach (var mapID in ResourceManager.Maps.GetResources())
            {
                if (count % _selectionsPrPage == 0)
                {
                    page++;
                    _mapPages.Add(new List<ButtonControl>());
                    offset = 0;
                }
                var newButton = new ButtonControl(Parent, clicked: SelectMap_Click)
                {
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = BasicFonts.GetFont(16),
                    Text = $"{ResourceManager.Maps.GetResource(mapID).Name}",
                    FontColor = Color.White,
                    X = 70,
                    Y = 610 + offset++ * 50,
                    Height = 50,
                    Width = 400,
                    Tag = mapID,
                    IsVisible = false
                };
                _mapPages[page].Add(newButton);
                AddControl(2, newButton);
                count++;
            }

            UpdateMapSelectionPages();
        }

        private void SetupGameStyleView()
        {
            AddControl(1, new TileControl(Parent)
            {
                X = 510,
                Y = 550,
                Height = 350,
                Width = 440,
                FillColor = Parent.UIResources.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(24),
                Text = "Game Styles",
                X = 540,
                Y = 560,
                Height = 50,
                Width = 400,
                FontColor = Color.White
            });

            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentGameStylePage--;
                if (_currentGameStylePage < 0)
                    _currentGameStylePage = 0;
                if (_currentGameStylePage >= _gameStylePages.Count)
                    _currentGameStylePage = _gameStylePages.Count - 1;
                UpdateGameStyleSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $"<",
                X = 530,
                Y = 560,
                Height = 50,
                Width = 50,
                IsVisible = ResourceManager.GameStyles.GetResources().Count > _selectionsPrPage
            });
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentGameStylePage++;
                if (_currentGameStylePage < 0)
                    _currentGameStylePage = 0;
                if (_currentGameStylePage >= _gameStylePages.Count)
                    _currentGameStylePage = _gameStylePages.Count - 1;
                UpdateGameStyleSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $">",
                X = 880,
                Y = 560,
                Height = 50,
                Width = 50,
                IsVisible = ResourceManager.GameStyles.GetResources().Count > _selectionsPrPage
            });

            int count = 0;
            int page = -1;
            int offset = 0;
            foreach (var gameStyleID in ResourceManager.GameStyles.GetResources())
            {
                if (count % _selectionsPrPage == 0)
                {
                    page++;
                    _gameStylePages.Add(new List<ButtonControl>());
                    offset = 0;
                }
                var newButton = new ButtonControl(Parent, clicked: SelectGameStyle_Click)
                {
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = BasicFonts.GetFont(16),
                    Text = $"{ResourceManager.GameStyles.GetResource(gameStyleID).Name}",
                    FontColor = Color.White,
                    X = 530,
                    Y = 610 + offset++ * 50,
                    Height = 50,
                    Width = 400,
                    Tag = gameStyleID,
                    IsVisible = false
                };
                _gameStylePages[page].Add(newButton);
                AddControl(2, newButton);
                count++;
            }

            UpdateGameStyleSelectionPages();
        }
    }
}
