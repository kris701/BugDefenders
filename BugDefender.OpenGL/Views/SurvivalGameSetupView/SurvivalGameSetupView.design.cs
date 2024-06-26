﻿using BugDefender.Core.Resources;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BugDefender.OpenGL.Screens.SurvivalGameSetupView
{
    public partial class SurvivalGameSetupView : BaseBugDefenderView
    {
        private TileControl _mapPreviewTile;
        private LabelControl _mapNameLabel;
        private TextboxControl _mapDescriptionTextbox;
        private TextboxControl _gameStyleDescriptionTextbox;
        private BugDefenderButtonControl _startButton;
        private LabelControl _totalDifficultyLabel;
        private TextInputControl _gameSaveName;
        private LabelControl _saveOverwriteWarningLabel;

        private BugDefenderButtonControl? _selectedGameStyleButton;
        private BugDefenderButtonControl? _selectedMapButton;

        private PageHandler<BugDefenderButtonControl> _mapPageHandler;
        private PageHandler<BugDefenderButtonControl> _gamestylePageHandler;

        [MemberNotNull(nameof(_mapPreviewTile), nameof(_mapNameLabel), nameof(_mapDescriptionTextbox),
            nameof(_startButton), nameof(_mapPageHandler), nameof(_gamestylePageHandler),
            nameof(_gameStyleDescriptionTextbox), nameof(_totalDifficultyLabel), nameof(_gameSaveName),
            nameof(_saveOverwriteWarningLabel))]
        public override void Initialize()
        {
            BasicMenuHelper.GenerateBaseMenu(
                this,
                Parent.Textures.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Survival Game Setup",
                "Select a map and a gamestyle to start.",
                Parent.Fonts);

            SetupPreviewPanel(50, 225, 900, 750);
            SetupMapsView(965, 225, 445, 750);
            SetupGameStyleView(1425, 225, 445, 750);

            _startButton = BasicMenuHelper.GetAcceptButton(Parent, "Start", StartButton_Click);
            _startButton.IsEnabled = false;
            AddControl(0, _startButton);
            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Back", (e) => { SwitchView(new MainMenu.MainMenuView(Parent)); }));
#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new SurvivalGameSetupView(Parent)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif

            base.Initialize();
        }

        [MemberNotNull(nameof(_mapPreviewTile), nameof(_mapNameLabel), nameof(_mapDescriptionTextbox),
            nameof(_gameStyleDescriptionTextbox), nameof(_totalDifficultyLabel), nameof(_gameSaveName),
            nameof(_saveOverwriteWarningLabel))]
        private void SetupPreviewPanel(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.Textures.GetTexture(new Guid("02f8c9e2-e4c0-4310-934a-62c84cbb7384")),
            });
            _mapPreviewTile = new TileControl()
            {
                FillColor = BasicTextures.GetBasicRectange(Color.Black),
                X = x + 50,
                Y = y + 260,
                Width = height - 300,
                Height = height - 300,
            };
            AddControl(1, new BorderControl(_mapPreviewTile)
            {
                Thickness = 3,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue)
            });

            _mapNameLabel = new LabelControl()
            {
                Text = "Select A Map",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                X = x + 50,
                Y = y + 20,
                Height = 50,
                Width = height - 300,
                FontColor = Color.White,
            };
            AddControl(1, _mapNameLabel);
            var boxHeight = height / 2;
            _mapDescriptionTextbox = new TextboxControl()
            {
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                X = x + (height - 300) + 75,
                Y = y + 20,
                Height = boxHeight - 30,
                Width = width - 550,
                FontColor = Color.White
            };
            AddControl(1, _mapDescriptionTextbox);
            _gameStyleDescriptionTextbox = new TextboxControl()
            {
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                X = x + (height - 300) + 75,
                Y = y + 10 + boxHeight,
                Height = boxHeight - 30,
                Width = width - 550,
                FontColor = Color.White
            };
            AddControl(1, _gameStyleDescriptionTextbox);
            _totalDifficultyLabel = new LabelControl()
            {
                Text = "Total Difficulty: ...",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                X = x + 50,
                Y = y + 70,
                Height = 20,
                Width = height - 300,
                FontColor = Color.White,
            };
            AddControl(1, new LabelControl()
            {
                X = x + 50,
                Y = y + 100,
                Height = 50,
                Width = height - 300,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                Text = "New Save Name",
                FontColor = Color.White
            });
            AddControl(1, _totalDifficultyLabel);
            _gameSaveName = new TextInputControl(Parent)
            {
                X = x + 50,
                Y = y + 150,
                Height = 50,
                Width = height - 300,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                Text = "New Game",
                Limit = 25,
                FontColor = Color.White,
                FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.Textures.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
            };
            _gameSaveName.OnKeyDown += NameKeyDown;
            AddControl(1, _gameSaveName);
            _saveOverwriteWarningLabel = new LabelControl()
            {
                X = x + 50,
                Y = y + 200,
                Height = 50,
                Width = height - 300,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                Text = "Overwrites existing save!",
                FontColor = Color.Red,
                IsVisible = Parent.UserManager.SaveExists(_gameSaveName.Text)
            };
            AddControl(1, _saveOverwriteWarningLabel);
        }

        [MemberNotNull(nameof(_mapPageHandler))]
        private void SetupMapsView(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.Textures.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl()
            {
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                Text = "Maps",
                X = x,
                Y = y + 10,
                Height = 50,
                Width = width,
                FontColor = Color.White
            });

            var controlList = new List<BugDefenderButtonControl>();
            var ids = ResourceManager.Maps.GetResources();
            foreach (var id in ids)
            {
                var map = ResourceManager.Maps.GetResource(id);
                controlList.Add(new BugDefenderButtonControl(Parent, SelectMap_Click)
                {
                    FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                    Text = $"{map.Name}",
                    FontColor = Color.White,
                    Height = 50,
                    Width = width - 20,
                    Tag = map
                });
            }
            _mapPageHandler = new PageHandler<BugDefenderButtonControl>(this, controlList)
            {
                LeftButtonX = 10,
                LeftButtonY = -50,
                RightButtonX = width - 80,
                RightButtonY = -50,
                ItemsPrPage = 12,
                X = x + 10,
                Y = y + 70,
                Width = width,
                Height = height
            };
            AddControl(1, _mapPageHandler);
        }


        [MemberNotNull(nameof(_gamestylePageHandler))]
        private void SetupGameStyleView(float x, float y, float width, float height)
        {
            AddControl(1, new TileControl()
            {
                X = x,
                Y = y,
                Height = height,
                Width = width,
                FillColor = Parent.Textures.GetTexture(new Guid("e5cb13c4-39e1-4906-b1d1-52e353fb0546")),
            });
            AddControl(1, new LabelControl()
            {
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                Text = "Game Styles",
                X = x,
                Y = y + 10,
                Height = 50,
                Width = width,
                FontColor = Color.White
            });

            var controlList = new List<BugDefenderButtonControl>();
            var ids = ResourceManager.GameStyles.GetResources();
            foreach (var id in ids)
            {
                var gameStyle = ResourceManager.GameStyles.GetResource(id);
                if (gameStyle.CampaignOnly)
                    continue;
                controlList.Add(new BugDefenderButtonControl(Parent, SelectGameStyle_Click)
                {
                    FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                    Text = $"{gameStyle.Name}",
                    FontColor = Color.White,
                    Height = 50,
                    Width = width - 20,
                    Tag = gameStyle
                });
            }
            _gamestylePageHandler = new PageHandler<BugDefenderButtonControl>(this, controlList)
            {
                LeftButtonX = 10,
                LeftButtonY = -50,
                RightButtonX = width - 80,
                RightButtonY = -50,
                ItemsPrPage = 12,
                X = x + 10,
                Y = y + 70,
                Width = width,
                Height = height
            };
            AddControl(1, _gamestylePageHandler);
        }
    }
}
