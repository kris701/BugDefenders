using BugDefender.Core.Game;
using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views.GameView;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static BugDefender.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace BugDefender.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseAnimatedView
    {
        private LabelControl _moneyLabel;
        private LabelControl _scoreLabel;

        private ButtonControl _startButton;
        private ButtonControl _sendWave;
        private ButtonControl _mainMenuButton;
        private ButtonControl _saveAndExitButton;
        private ButtonControl _autoRunButton;

        private LabelControl _playtimeLabel;

        private AnimatedTileControl _buyingPreviewTile;
        private TileControl _buyingPreviewRangeTile;

        private TileControl _turretSelectRangeTile;

        private readonly List<EnemyQueueControl> _nextEnemyPanels = new List<EnemyQueueControl>();

        private readonly List<ButtonControl> _turretTargetingModes = new List<ButtonControl>();

        private TextboxControl _turretStatesTextbox;
        private ButtonControl _sellTurretButton;

        private readonly PageHandler<ButtonControl> _turretPageHandler = new PageHandler<ButtonControl>()
        {
            ItemsPrPage = 13,
            ButtonSize = 25,
            ButtonFontSize = 10
        };

        private readonly PageHandler<UpgradePanel> _upgradePageHandler = new PageHandler<UpgradePanel>()
        {
            ItemsPrPage = 3,
            ButtonSize = 25,
            ButtonFontSize = 10,
            Margin = 30,
            IsVisible = false
        };

        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("32b08b60-c8b9-450a-90b3-73086261e87f")),
                Width = GameWindow.BaseScreenSize.X,
                Height = GameWindow.BaseScreenSize.Y
            });

            SetupGameField(_gameArea.X, _gameArea.Y, _gameArea.Width, _gameArea.Height);
            SetupGameControlsField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y, 320, 160);

            if (_game.Context.Challenge != null)
                SetupChallengeInfoField(_gameArea.X + _gameArea.Width + 10 + 330, _gameArea.Y, 320, 160);
            else
                SetupBaseGameField(_gameArea.X + _gameArea.Width + 10 + 330, _gameArea.Y, 320, 160);

            SetupPurchasingField(_gameArea.X + _gameArea.Width + 10 + 330, _gameArea.Y + 160, 320, 570);
            SetupUpgradeField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y + 160, 320, 570);
            SetupNextEnemyPanel(_gameArea.X, _gameArea.Y + _gameArea.Height + 5, _gameArea.Width, 110);
            SetupTurretStatsPanel(_gameArea.X + _gameArea.Width + 10, 745, 650, 330);

            if (CheatsHelper.Cheats.Count > 0)
            {
                AddControl(100, new LabelControl()
                {
                    Text = $"Cheats On!",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.HotPink,
                    X = _gameArea.X + 40,
                    Y = _gameArea.Y + 10,
                    Height = 30,
                    Width = 100
                });
            }

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameScreen(Parent, new GameContext()
            {
                Map = _game.Context.Map,
                GameStyle = _game.Context.GameStyle
            })))
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

        private void SetupGameField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(_game.Context.Map.ID),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(100, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("86f37f1c-921f-484a-98da-4b0790f51d70")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            _turretSelectRangeTile = new TileControl()
            {
                IsVisible = false,
                Alpha = 25,
                ViewPort = _gameArea
            };
            AddControl(0, _turretSelectRangeTile);
        }

        private void SetupGameControlsField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            _scoreLabel = new LabelControl()
            {
                Text = $"Wave 0, Score 0, HP: 50",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 5,
                Height = 30,
                Width = width - 10
            };
            AddControl(1, _scoreLabel);
            _moneyLabel = new LabelControl()
            {
                Text = $"Money: {_game.Context.Money}$",
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 30,
                Height = 30,
                Width = width - 10
            };
            AddControl(1, _moneyLabel);

            _startButton = new ButtonControl(Parent, clicked: StartButton_Click)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"Pause",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 60,
                Height = 30,
                Width = 100
            };
            AddControl(1, _startButton);

            _mainMenuButton = new ButtonControl(Parent, clicked: (x) => { GoToMainMenu(); })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                Text = $"Exit",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 90,
                Height = 30,
                Width = 100
            };
            AddControl(1, _mainMenuButton);
            _saveAndExitButton = new ButtonControl(Parent, clicked: (x) => { SaveAndGoToMainMenu(); })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                Text = $"Save and Exit",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 110,
                Y = yOffset + 90,
                Height = 30,
                Width = 200,
                IsEnabled = false
            };
            AddControl(1, _saveAndExitButton);

            _autoRunButton = new ButtonControl(Parent, clicked: AutoRunButton_Click)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"[ ] Auto-Wave",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 110,
                Y = yOffset + 60,
                Height = 30,
                Width = 200
            };
            AddControl(1, _autoRunButton);

            _sendWave = new ButtonControl(Parent, clicked: (s) =>
            {
                _game.EnemiesModule.QueueEnemies();
                if (_game.Context.GameStyle.MoneyPrWave > 0 || _game.Context.Turrets.Any(x => x.TurretInfo is InvestmentTurretDefinition))
                    Parent.UIResources.PlaySoundEffectOnce(new Guid("e6908fa1-85b3-4f18-9bf0-cc4fa97a29c1"));
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"Send Wave",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 120,
                Height = 30,
                Width = width - 20
            };
            AddControl(1, _sendWave);
        }

        private void SetupBaseGameField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = "Bug Defenders",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 15,
                Height = 30,
                Width = width
            });
            _playtimeLabel = new LabelControl()
            {
                Text = "Game Time: ",
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 45,
                Height = 30,
                Width = width
            };
            AddControl(1, _playtimeLabel);
            AddControl(1, new LabelControl()
            {
                Text = $"Map: {_game.Context.Map.Name}",
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 75,
                Height = 30,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = $"Game Style: {_game.Context.GameStyle.Name}",
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 105,
                Height = 30,
                Width = width
            });
        }

        private void SetupChallengeInfoField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = "Challenge Info",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 15,
                Height = 30,
                Width = width
            });
            if (_game.Context.Challenge == null)
                return;
            var sb = new StringBuilder();
            foreach (var req in _game.Context.Challenge.Criterias)
                sb.AppendLine(req.ToString());
            AddControl(1, new LabelControl()
            {
                Text = $"Challenge: {_game.Context.Challenge.Name}",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 45,
                Height = 30,
                Width = width
            });
            AddControl(1, new TextboxControl()
            {
                Text = sb.ToString(),
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 75,
                Height = 80,
                Width = width
            });
        }

        private void SetupPurchasingField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("98e37f25-6313-4e41-8805-2eabcde084ff")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = "Turrets",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 5,
                Height = 30,
                Width = width
            });

            _turretPageHandler.LeftButtonX = xOffset + 10;
            _turretPageHandler.LeftButtonY = yOffset + 10;
            _turretPageHandler.RightButtonX = xOffset + width - 35;
            _turretPageHandler.RightButtonY = yOffset + 10;
            _turretPageHandler.X = xOffset + 10;
            _turretPageHandler.Y = yOffset + 35;
            var optionIDs = ResourceManager.Turrets.GetResources();
            var sorted = new List<TurretDefinition>();
            foreach (var id in optionIDs)
                sorted.Add(ResourceManager.Turrets.GetResource(id));
            sorted = sorted.OrderBy(x => x.AvailableAtWave).ThenByDescending(x => x.Cost).ToList();

            var controlList = new List<ButtonControl>();
            foreach (var turret in sorted)
            {
                controlList.Add(new ButtonControl(Parent, BuyTurret_Click)
                {
                    Height = 30,
                    Width = width - 20,
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    FillDisabledColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                    Font = BasicFonts.GetFont(10),
                    Text = $"[{turret.Cost}$] {turret.Name}",
                    FontColor = Color.White,
                    Tag = turret
                });
            }
            _turretPageHandler.Initialize(controlList, this);

            _buyingPreviewRangeTile = new TileControl()
            {
                IsVisible = false,
                Alpha = 25,
                ViewPort = _gameArea
            };
            AddControl(10, _buyingPreviewRangeTile);
            _buyingPreviewTile = new AnimatedTileControl()
            {
                IsVisible = false,
                ViewPort = _gameArea
            };
            AddControl(10, _buyingPreviewTile);
        }

        private void SetupUpgradeField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("98e37f25-6313-4e41-8805-2eabcde084ff")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = "Upgrades",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            _upgradePageHandler.LeftButtonX = xOffset + 10;
            _upgradePageHandler.LeftButtonY = yOffset + 10;
            _upgradePageHandler.RightButtonX = xOffset + width - 35;
            _upgradePageHandler.RightButtonY = yOffset + 10;
            _upgradePageHandler.X = xOffset + 10;
            _upgradePageHandler.Y = yOffset + 35;
            var controlList = new List<UpgradePanel>();
            for (int i = 0; i < 9; i++)
            {
                controlList.Add(new UpgradePanel(Parent, BuyUpgrade_Click)
                {

                });
            }
            _upgradePageHandler.MinPage = 0;
            _upgradePageHandler.MaxPage = 0;
            _upgradePageHandler.Initialize(controlList, this);
        }

        private void SetupNextEnemyPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("2712c649-3e74-44ca-b8a9-c3032aba217e")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = "Next Enemies",
                Font = BasicFonts.GetFont(10),
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
            for (int i = 0; i < 4; i++)
            {
                var newItem = new EnemyQueueControl(Parent)
                {
                    X = x + (itemXOffset++ * (itemWidth + margin)),
                    Y = y
                };
                _nextEnemyPanels.Add(newItem);
                AddControl(1, newItem);
            }
        }

        private void SetupTurretStatsPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("90447608-bd7a-478c-9bfd-fddb26c731b7")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl()
            {
                Text = "Turret Stats",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 5,
                Height = 35,
                Width = width
            });

            _sellTurretButton = new ButtonControl(Parent, clicked: SellTurret_Click)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = "Sell Turret",
                X = xOffset + 10,
                Y = yOffset + 40,
                Width = width - 20,
                Height = 30,
                IsEnabled = false
            };
            AddControl(1, _sellTurretButton);
            _turretStatesTextbox = new TextboxControl()
            {
                Font = BasicFonts.GetFont(8),
                FontColor = Color.White,
                Text = "Select a Turret",
                X = xOffset + 10,
                Y = yOffset + 75,
                Width = width - 20,
                Height = height - 115
            };
            AddControl(1, _turretStatesTextbox);

            var values = Enum.GetValues(typeof(TargetingTypes)).Cast<TargetingTypes>().ToList();
            var buttonWidth = (width - 20) / (values.Count - 1) - 5;
            var count = 0;
            foreach (TargetingTypes option in values.Skip(1))
            {
                var newControl = new ButtonControl(Parent, (e) =>
                {
                    if (_selectedTurret != null)
                        _selectedTurret.TargetingType = option;
                    foreach (var button in _turretTargetingModes)
                        button.FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")); ;
                    e.FillColor = Parent.UIResources.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"));
                })
                {
                    FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    FillDisabledColor = Parent.UIResources.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                    Font = BasicFonts.GetFont(10),
                    FontColor = Color.White,
                    Text = $"{Enum.GetName(typeof(TargetingTypes), option)}",
                    X = xOffset + 12 + (count++ * (buttonWidth + 5)),
                    Y = yOffset + 285,
                    Height = 25,
                    Width = buttonWidth,
                    IsEnabled = false,
                };
                _turretTargetingModes.Add(newControl);
                AddControl(1, newControl);
            }
        }
    }
}
