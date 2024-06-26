﻿using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Resources;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.GameView;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace BugDefender.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseBugDefenderView
    {
        private LabelControl _moneyLabel;
        private LabelControl _scoreLabel;

        private BugDefenderButtonControl _sendWave;
        private BugDefenderButtonControl _saveAndExitButton;
        private ConfirmationControl _confirmationControl;

        private LabelControl _playtimeLabel;

        private AnimatedTileControl _buyingPreviewTile;
        private TileControl _buyingPreviewRangeTile;
        private TileControl _buyingPreviewSizeTile;

        private TileControl _turretSelectRangeTile;
        private TileControl _hurtGameAreaTile;

        private readonly List<EnemyQueueControl> _nextEnemyPanels = new List<EnemyQueueControl>();

        private TextboxControl _gameInfoTextbox;
        private TurretInfoPanel _turretInfoPanel;
        private EnemyInfoPanel _enemyInfoPanel;

        private PageHandler<TurretPurchasePanel> _turretPageHandler;
        private PageHandler<UpgradePanel> _upgradePageHandler;

        [MemberNotNull(nameof(_moneyLabel), nameof(_scoreLabel), nameof(_sendWave),
            nameof(_saveAndExitButton), nameof(_upgradePageHandler), nameof(_buyingPreviewTile),
            nameof(_buyingPreviewRangeTile), nameof(_turretInfoPanel), nameof(_enemyInfoPanel),
            nameof(_turretPageHandler), nameof(_turretSelectRangeTile), nameof(_hurtGameAreaTile),
            nameof(_playtimeLabel), nameof(_gameInfoTextbox), nameof(_buyingPreviewSizeTile), nameof(_confirmationControl))]
        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("32b08b60-c8b9-450a-90b3-73086261e87f")),
                Width = IWindow.BaseScreenSize.X,
                Height = IWindow.BaseScreenSize.Y
            });

            SetupGameField(_gameArea.X, _gameArea.Y, _gameArea.Width, _gameArea.Height);
            SetupGameControlsField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y, 320, 160);
            SetupBaseGameField(_gameArea.X + _gameArea.Width + 10 + 330, _gameArea.Y, 320, 160);
            SetupPurchasingField(_gameArea.X + _gameArea.Width + 10 + 330, _gameArea.Y + 160, 320, 570);
            SetupUpgradeField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y + 160, 320, 570);
            SetupNextEnemyPanel(_gameArea.X, _gameArea.Y + _gameArea.Height + 5, _gameArea.Width, 110);
            _turretInfoPanel = new TurretInfoPanel(Parent, _game.Context.GameStyle, SellTurret_Click)
            {
                X = _gameArea.X + _gameArea.Width + 10,
                Y = 745
            };
            AddControl(101, _turretInfoPanel);
            _enemyInfoPanel = new EnemyInfoPanel(Parent)
            {
                X = _gameArea.X + _gameArea.Width + 10,
                Y = 745,
                IsVisible = false
            };
            AddControl(101, _enemyInfoPanel);
            _confirmationControl = new ConfirmationControl(Parent,
                (s) => { SaveAndGoToMainMenu(); },
                (s) =>
                {
                    _confirmationControl!.IsVisible = false;
                    _game.Running = true;
                })
            {
                IsVisible = false
            };
            AddControl(200, _confirmationControl);

            if (CheatsHelper.Cheats.Count > 0)
            {
                AddControl(200, new LabelControl()
                {
                    Text = $"Cheats On!",
                    Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                    FontColor = Color.HotPink,
                    X = _gameArea.X + 40,
                    Y = _gameArea.Y + 10,
                    Height = 30,
                    Width = 100
                });
            }

            base.Initialize();

#if DEBUG
            if (_game.Criterias.Count > 0)
            {
                AddControl(999, new ButtonControl(Parent, (e) =>
                {
                    _game.EndGame();
                })
                {
                    Text = $"Complete!",
                    Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                    FontColor = Color.HotPink,
                    X = 20,
                    Y = 20,
                    Height = 30,
                    Width = 100
                });
            }
#endif
        }

        [MemberNotNull(nameof(_turretSelectRangeTile), nameof(_hurtGameAreaTile))]
        private void SetupGameField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(50, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(_game.Context.Map.ID),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(100, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("86f37f1c-921f-484a-98da-4b0790f51d70")),
                X = xOffset - 50,
                Y = yOffset - 50,
                Height = height + 100,
                Width = width + 100
            });
            _hurtGameAreaTile = new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("3715b90d-7cc7-4528-99fc-84c8497d06c1")),
                X = xOffset - 50,
                Y = yOffset - 50,
                Height = height + 100,
                Width = width + 100,
                IsVisible = false
            };
            AddControl(100, _hurtGameAreaTile);
            _turretSelectRangeTile = new TileControl()
            {
                IsVisible = false,
                Alpha = 100,
                ViewPort = _gameArea
            };
            AddControl(101, _turretSelectRangeTile);
        }

        [MemberNotNull(nameof(_scoreLabel), nameof(_moneyLabel), nameof(_saveAndExitButton),
            nameof(_sendWave))]
        private void SetupGameControlsField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(101, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            _scoreLabel = new LabelControl()
            {
                Text = $"Wave 0, Score 0, HP: 50",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 5,
                Height = 30,
                Width = width - 10
            };
            AddControl(101, _scoreLabel);
            _moneyLabel = new LabelControl()
            {
                Text = $"Money: {_game.Context.Money}$",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 30,
                Height = 30,
                Width = width - 10
            };
            AddControl(101, _moneyLabel);

            AddControl(101, new BugDefenderButtonControl(Parent, clicked: StartButton_Click)
            {
                FillColor = Parent.Textures.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"Pause",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 60,
                Height = 30,
                Width = 100
            });

            _saveAndExitButton = new BugDefenderButtonControl(Parent,
                (x) =>
                {
                    _game.Running = false;
                    _confirmationControl.IsVisible = true;
                }
                )
            {
                FillColor = Parent.Textures.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = Parent.Textures.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                Text = $"Save and Exit",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                X = xOffset + 110,
                Y = yOffset + 60,
                Height = 30,
                Width = 200,
                IsEnabled = false
            };
            AddControl(101, _saveAndExitButton);

            AddControl(101, new BugDefenderButtonControl(Parent, clicked: AutoRunButton_Click)
            {
                FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                X = xOffset + 10,
                Y = yOffset + 90,
                Height = 30,
                Width = width - 20,
                Text = $"[ ] Auto-Wave",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White
            });

            _sendWave = new BugDefenderButtonControl(Parent, clicked: (s) =>
            {
                _game.EnemiesModule.QueueEnemies();
                if (_game.Context.GameStyle.MoneyPrWave > 0 || _game.Context.Turrets.Any(x => x.TurretInfo is InvestmentTurretDefinition))
                    Parent.Audio.PlaySoundEffectOnce(new Guid("e6908fa1-85b3-4f18-9bf0-cc4fa97a29c1"));
            })
            {
                FillColor = Parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"Send Wave",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 120,
                Height = 30,
                Width = width - 20
            };
            AddControl(101, _sendWave);
        }

        [MemberNotNull(nameof(_playtimeLabel), nameof(_gameInfoTextbox))]
        private void SetupBaseGameField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(101, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            var title = "Bug Defenders";
            if (_game.Criterias.Count > 0)
                title = "Win Criterias";
            AddControl(101, new LabelControl()
            {
                Text = title,
                Font = Parent.Fonts.GetFont(FontSizes.Ptx24),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 15,
                Height = 30,
                Width = width
            });
            _playtimeLabel = new LabelControl()
            {
                Text = "Game Time: ",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx12),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 45,
                Height = 30,
                Width = width
            };
            AddControl(101, _playtimeLabel);

            var sb = new StringBuilder();
            if (_game.Criterias.Count > 0)
            {
                foreach (var req in _game.Criterias)
                    sb.AppendLine(req.Progress(_game.Context.Stats));
            }
            else
            {
                sb.AppendLine($"Map: {_game.Context.Map.Name}");
                sb.AppendLine($"Game Style: {_game.Context.GameStyle.Name}");
            }
            _gameInfoTextbox = new TextboxControl()
            {
                Text = sb.ToString(),
                Font = Parent.Fonts.GetFont(FontSizes.Ptx8),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 75,
                Height = 80,
                Width = width
            };
            AddControl(101, _gameInfoTextbox);
        }

        [MemberNotNull(nameof(_turretPageHandler), nameof(_buyingPreviewRangeTile), nameof(_buyingPreviewTile), nameof(_buyingPreviewSizeTile))]
        private void SetupPurchasingField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(101, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("98e37f25-6313-4e41-8805-2eabcde084ff")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(101, new LabelControl()
            {
                Text = "Turrets",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 5,
                Height = 30,
                Width = width
            });

            var optionIDs = ResourceManager.Turrets.GetResources();
            if (_game.Context.GameStyle.TurretWhiteList.Count > 0)
                optionIDs.RemoveAll(x => !_game.Context.GameStyle.TurretWhiteList.Contains(x));
            else if (_game.Context.GameStyle.TurretBlackList.Count > 0)
                optionIDs.RemoveAll(_game.Context.GameStyle.TurretBlackList.Contains);

            var sorted = new List<TurretDefinition>();
            foreach (var id in optionIDs)
                sorted.Add(ResourceManager.Turrets.GetResource(id));
            sorted = sorted.OrderBy(x => x.AvailableAtWave).ThenBy(x => x.Cost).ToList();

            var controlList = new List<TurretPurchasePanel>();
            foreach (var turret in sorted)
                controlList.Add(new TurretPurchasePanel(Parent, turret, BuyTurret_Click));
            _turretPageHandler = new PageHandler<TurretPurchasePanel>(this, controlList)
            {
                ItemsPrPage = 7,
                ButtonSize = 25,
                ButtonFontSize = FontSizes.Ptx10,
                LeftButtonX = 5,
                LeftButtonY = -25,
                RightButtonX = width - 50,
                RightButtonY = -25,
                X = xOffset + 10,
                Y = yOffset + 35,
                Margin = 5,
                Width = width,
                Height = height
            };
            AddControl(101, _turretPageHandler);

            _buyingPreviewRangeTile = new TileControl()
            {
                IsVisible = false,
                Alpha = 150,
                ViewPort = _gameArea
            };
            AddControl(101, _buyingPreviewRangeTile);
            _buyingPreviewSizeTile = new TileControl()
            {
                IsVisible = false,
                ViewPort = _gameArea
            };
            AddControl(101, _buyingPreviewSizeTile);
            _buyingPreviewTile = new AnimatedTileControl()
            {
                IsVisible = false,
                ViewPort = _gameArea
            };
            AddControl(101, _buyingPreviewTile);
        }

        [MemberNotNull(nameof(_upgradePageHandler))]
        private void SetupUpgradeField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(101, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("98e37f25-6313-4e41-8805-2eabcde084ff")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(101, new LabelControl()
            {
                Text = "Upgrades",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx16),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            var controlList = new List<UpgradePanel>();
            for (int i = 0; i < 9; i++)
            {
                controlList.Add(new UpgradePanel(Parent, BuyUpgrade_Click)
                {
                    IsVisible = false
                });
            }
            _upgradePageHandler = new PageHandler<UpgradePanel>(this, controlList)
            {
                ItemsPrPage = 3,
                ButtonSize = 25,
                ButtonFontSize = FontSizes.Ptx10,
                Margin = 20,
                LeftButtonX = 5,
                LeftButtonY = -25,
                RightButtonX = width - 50,
                RightButtonY = -25,
                X = xOffset + 10,
                Y = yOffset + 35,
                Width = width,
                Height = height
            };
            _upgradePageHandler.MinPage = 0;
            _upgradePageHandler.MaxPage = 0;
            _upgradePageHandler.MaxItem = 0;
            AddControl(101, _upgradePageHandler);
        }

        private void SetupNextEnemyPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(101, new TileControl()
            {
                FillColor = Parent.Textures.GetTexture(new Guid("2712c649-3e74-44ca-b8a9-c3032aba217e")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(101, new LabelControl()
            {
                Text = "Next Enemies",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            int x = xOffset + 20;
            int y = yOffset + 30;
            int itemXOffset = 0;
            int itemWidth = 220;
            int margin = 10;
            _nextEnemyPanels.Clear();
            for (int i = 0; i < 4; i++)
            {
                var newItem = new EnemyQueueControl(Parent, SelectEnemy)
                {
                    X = x + (itemXOffset++ * (itemWidth + margin)),
                    Y = y
                };
                _nextEnemyPanels.Add(newItem);
                AddControl(101, newItem);
            }
        }
    }
}
