using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using static TDGame.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseScreen
    {
        private LabelControl _moneyLabel;
        private LabelControl _hpLabel;
        private LabelControl _scoreLabel;

        private ButtonControl _startButton;
        private ButtonControl _sendWave;
        private ButtonControl _autoRunButton;

        private AnimatedTileControl _buyingPreviewTile;
        private TileControl _buyingPreviewRangeTile;

        private TileControl _turretSelectRangeTile;

        private List<EnemyQueueControl> _nextEnemyPanels = new List<EnemyQueueControl>();

        private List<ButtonControl> _turretTargetingModes = new List<ButtonControl>();

        private TextboxControl _turretStatesTextbox;
        private ButtonControl _sellTurretButton;

        private int _turretSelectionsPrPage = 13;
        private List<List<ButtonControl>> _turretPages = new List<List<ButtonControl>>();
        private int _currentTurretPage = 0;

        private ButtonControl _upgradesLeftButton;
        private ButtonControl _upgradesRightButton;
        private int _upgradeSelectionsPrPage = 3;
        private List<List<UpgradePanel>> _turretUpgradePages = new List<List<UpgradePanel>>();
        private int _currentTurretUpgradePage = 0;

        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("32b08b60-c8b9-450a-90b3-73086261e87f")),
                Width = 1000,
                Height = 1000
            });

            SetupGameField();
            SetupGameControlsField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y - 5, 320, 160);
            SetupPurchasingField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y + 160, 320, 490);
            SetupUpgradeField(_gameArea.X, _gameArea.Y + _gameArea.Height + 10, _gameArea.Width, 200);
            SetupNextEnemyPanel(_gameArea.X, _gameArea.Y + _gameArea.Height + 20 + 200, _gameArea.Width, 110);
            SetupTurretStatsPanel(_gameArea.X + _gameArea.Width + 10, _gameArea.Y + _gameArea.Height + 10, 320, 320);

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new GameScreen(Parent, _currentMap, _currentGameStyle)))
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

        private void SetupGameField()
        {
            AddControl(0, new BorderControl(Parent)
            {
                Thickness = 2,
                BorderBrush = BasicTextures.GetBasicRectange(Color.Blue),
                Child = new TileControl(Parent)
                {
                    FillColor = TextureManager.GetTexture(_game.Map.ID),
                    X = _gameArea.X,
                    Y = _gameArea.Y,
                    Height = _gameArea.Height,
                    Width = _gameArea.Width
                }
            });
            _turretSelectRangeTile = new TileControl(Parent)
            {
                IsVisible = false,
                Alpha = 25
            };
            AddControl(0, _turretSelectRangeTile);
        }

        private void SetupGameControlsField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl(Parent)
            {
                Text = "TDGame",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                X = xOffset,
                Y = yOffset + 5,
                Height = 30,
                Width = width
            });
            _scoreLabel = new LabelControl(Parent)
            {
                Text = $"Wave 0, Score 0",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 25,
                Height = 30,
                Width = width - 10
            };
            AddControl(1, _scoreLabel);
            _moneyLabel = new LabelControl(Parent)
            {
                Text = $"Money: {_game.Money}$",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 45,
                Height = 30,
                Width = 310
            };
            AddControl(1, _moneyLabel);
            _hpLabel = new LabelControl(Parent)
            {
                Text = $"HP: {_game.HP}",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 5,
                Y = yOffset + 65,
                Height = 30,
                Width = 310
            };
            AddControl(1, _hpLabel);

            _startButton = new ButtonControl(Parent, clicked: StartButton_Click)
            {
                FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"Pause",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 90,
                Height = 30,
                Width = 100
            };
            AddControl(1, _startButton);

            AddControl(1, new ButtonControl(Parent, clicked: (x) => { GameOver(); })
            {
                FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"End Game",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 10,
                Y = yOffset + 120,
                Height = 30,
                Width = 100
            });

            _autoRunButton = new ButtonControl(Parent, clicked: AutoRunButton_Click)
            {
                FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"[ ] Auto-Wave",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 110,
                Y = yOffset + 90,
                Height = 30,
                Width = 200
            };
            AddControl(1, _autoRunButton);

            _sendWave = new ButtonControl(Parent, clicked: (s) => { _game.QueueEnemies(); })
            {
                FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Text = $"Send Wave",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset + 110,
                Y = yOffset + 120,
                Height = 30,
                Width = 200
            };
            AddControl(1, _sendWave);
        }

        private void SetupPurchasingField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("98e37f25-6313-4e41-8805-2eabcde084ff")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl(Parent)
            {
                Text = "Turrets",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset + 5,
                Height = 30,
                Width = width
            });

            AddControl(1, new ButtonControl(Parent, clicked: (s) => {
                _currentTurretPage--;
                if (_currentTurretPage < 0)
                    _currentTurretPage = 0;
                if (_currentTurretPage >= _turretPages.Count)
                    _currentTurretPage = _turretPages.Count - 1;
                UpdateTurretSelectionPages();
            })
            {
                FillColor = TextureManager.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = TextureManager.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(10),
                Text = $"<",
                X = xOffset + 10,
                Y = yOffset + 10,
                Height = 25,
                Width = 25,
                IsVisible = ResourceManager.Turrets.GetResources().Count > _turretSelectionsPrPage
            });
            AddControl(1, new ButtonControl(Parent, clicked: (s) => {
                _currentTurretPage++;
                if (_currentTurretPage < 0)
                    _currentTurretPage = 0;
                if (_currentTurretPage >= _turretPages.Count)
                    _currentTurretPage = _turretPages.Count - 1;
                UpdateTurretSelectionPages();
            })
            {
                FillColor = TextureManager.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = TextureManager.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(10),
                Text = $">",
                X = xOffset + 285,
                Y = yOffset + 10,
                Height = 25,
                Width = 25,
                IsVisible = ResourceManager.Turrets.GetResources().Count > _turretSelectionsPrPage
            });

            int count = 1;
            int page = 0;
            int offset = 0;
            _turretPages.Add(new List<ButtonControl>());
            foreach (var turretID in ResourceManager.Turrets.GetResources())
            {
                if (count++ % (_turretSelectionsPrPage + 1) == 0)
                {
                    page++;
                    _turretPages.Add(new List<ButtonControl>());
                    offset = 0;
                }
                var turret = ResourceManager.Turrets.GetResource(turretID);
                var newButton = new ButtonControl(Parent, clicked: BuyTurret_Click)
                {
                    FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = TextureManager.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    FillDisabledColor = TextureManager.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                    Font = BasicFonts.GetFont(10),
                    Text = $"[{turret.Cost}$] {turret.Name}",
                    FontColor = Color.White,
                    X = xOffset + 10,
                    Y = yOffset + 35 + (offset++ * 35),
                    Height = 30,
                    Width = width - 20,
                    Tag = turret,
                    IsVisible = false
                };
                _turretPages[page].Add(newButton);
                AddControl(2, newButton);
            }

            UpdateTurretSelectionPages();

            _buyingPreviewRangeTile = new TileControl(Parent)
            {
                IsVisible = false,
                Alpha = 25
            };
            AddControl(10, _buyingPreviewRangeTile);
            _buyingPreviewTile = new AnimatedTileControl(Parent)
            {
                IsVisible = false
            };
            AddControl(10, _buyingPreviewTile);
        }

        private void SetupUpgradeField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("f367b2e1-0b9f-40da-95af-dac50b16a0ad")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl(Parent)
            {
                Text = "Upgrades",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            _upgradesLeftButton = new ButtonControl(Parent, clicked: (s) =>
            {
                _currentTurretUpgradePage--;
                if (_currentTurretUpgradePage < 0)
                    _currentTurretUpgradePage = 0;
                if (_currentTurretUpgradePage >= _turretUpgradePages.Count)
                    _currentTurretUpgradePage = _turretUpgradePages.Count - 1;
                UpdateTurretUpgradeSelectionPages();
            })
            {
                FillColor = TextureManager.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = TextureManager.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(10),
                Text = $"<",
                X = xOffset + 10,
                Y = yOffset + 10,
                Height = 25,
                Width = 25,
                IsVisible = ResourceManager.Turrets.GetResources().Count > _turretSelectionsPrPage
            };
            AddControl(1, _upgradesLeftButton);
            _upgradesRightButton = new ButtonControl(Parent, clicked: (s) =>
            {
                _currentTurretUpgradePage++;
                if (_currentTurretUpgradePage < 0)
                    _currentTurretUpgradePage = 0;
                if (_currentTurretUpgradePage >= _turretUpgradePages.Count)
                    _currentTurretUpgradePage = _turretUpgradePages.Count - 1;
                UpdateTurretUpgradeSelectionPages();
            })
            {
                FillColor = TextureManager.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = TextureManager.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(10),
                Text = $">",
                X = xOffset + 615,
                Y = yOffset + 10,
                Height = 25,
                Width = 25,
                IsVisible = ResourceManager.Turrets.GetResources().Count > _turretSelectionsPrPage
            };
            AddControl(1, _upgradesRightButton);
        }

        private void SetupNextEnemyPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("2712c649-3e74-44ca-b8a9-c3032aba217e")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl(Parent)
            {
                Text = "Next Enemies",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            int x = xOffset + 15;
            int y = yOffset + 30;
            int itemXOffset = 0;
            int itemWidth = 200;
            int margin = 10;
            for (int i = 0; i < 3; i++)
            {
                var newItem = new EnemyQueueControl(Parent)
                {
                    X = x + (itemXOffset++ * (itemWidth + margin)),
                    Y = y,
                    Height = 70,
                    Width = itemWidth,
                    FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                };
                _nextEnemyPanels.Add(newItem);
                AddControl(1, newItem);
            }
        }

        private void SetupTurretStatsPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = TextureManager.GetTexture(new Guid("90447608-bd7a-478c-9bfd-fddb26c731b7")),
                X = xOffset,
                Y = yOffset,
                Height = height,
                Width = width
            });
            AddControl(1, new LabelControl(Parent)
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
                FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = TextureManager.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = TextureManager.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
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
            _turretStatesTextbox = new TextboxControl(Parent)
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
                        button.FillColor = TextureManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")); ;
                    e.FillColor = TextureManager.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"));
                })
                {
                    FillColor = TextureManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = TextureManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    FillDisabledColor = TextureManager.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
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
