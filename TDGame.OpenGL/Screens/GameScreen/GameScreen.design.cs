using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TDGame.Core.Turret;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

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

        private TileControl _buyingPreviewTile;
        private TileControl _buyingPreviewRangeTile;

        private TileControl _turretSelectRangeTile;

        private List<ButtonControl> _turretButtons = new List<ButtonControl>();

        private List<UpgradePanel> _turretUpgradePanels = new List<UpgradePanel>();
        private List<LabelControl> _nextEnemyPanels = new List<LabelControl>();

        private TextboxControl _turretStatesTextbox;

        public override void Initialize()
        {
#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new GameScreen(Parent, _currentMap, _currentGameStyle)))
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
            SetupGameField();
            SetupGameControlsField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y, 320, 205);
            SetupPurchasingField(_gameArea.X + _gameArea.Width + 10, _gameArea.Y + 215, 320, 435);
            SetupUpgradeField(_gameArea.X, _gameArea.Y + _gameArea.Height + 10, _gameArea.Width, 200);
            SetupNextEnemyPanel(_gameArea.X, _gameArea.Y + _gameArea.Height + 20 + 200, 980, 110);
            SetupTurretStatsPanel(_gameArea.X + _gameArea.Width + 10, _gameArea.Y + _gameArea.Height + 10, 320, 200);

            base.Initialize();
        }

        private void SetupGameField()
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = TextureBuilder.GetTexture(_game.Map.ID),
                    X = _gameArea.X,
                    Y = _gameArea.Y,
                    Height = _gameArea.Height,
                    Width = _gameArea.Width
                }
            });
            _turretSelectRangeTile = new TileControl(this)
            {
                IsVisible = false,
                Alpha = 100
            };
            AddControl(0, _turretSelectRangeTile);
        }

        private void SetupGameControlsField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = xOffset,
                    Y = yOffset,
                    Height = height,
                    Width = width
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "TDGame",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = xOffset,
                Y = yOffset,
                Height = 30,
                Width = width
            });
            _scoreLabel = new LabelControl(this)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                Text = $"Score : 0",
                Font = BasicFonts.GetFont(16),
                X = xOffset + 5,
                Y = yOffset + 30,
                Height = 30,
                Width = width - 10
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _scoreLabel
            });
            _startButton = new ButtonControl(this, clicked: StartButton_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"Pause",
                Font = BasicFonts.GetFont(16),
                X = xOffset + 5,
                Y = yOffset + 135 ,
                Height = 30,
                Width = 100
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _startButton
            });

            AddControl(1, new BorderControl(this)
            {
                Child = new ButtonControl(this, clicked: (x) => { SwitchView(new MainMenu.MainMenu(Parent)); })
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.White),
                    FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                    Text = $"Exit",
                    Font = BasicFonts.GetFont(16),
                    X = xOffset + 5,
                    Y = yOffset + 170,
                    Height = 30,
                    Width = 100
                }
            });

            _moneyLabel = new LabelControl(this)
            {
                Text = $"Money: {_game.Money}$",
                Font = BasicFonts.GetFont(16),
                X = xOffset + 5,
                Y = yOffset + 65,
                Height = 30,
                Width = 310
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _moneyLabel
            });

            _hpLabel = new LabelControl(this)
            {
                Text = $"HP: {_game.HP}",
                Font = BasicFonts.GetFont(16),
                X = xOffset + 5,
                Y = yOffset + 100,
                Height = 30,
                Width = 310
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _hpLabel
            });

            _autoRunButton = new ButtonControl(this, clicked: AutoRunButton_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"[ ] Auto-Wave",
                Font = BasicFonts.GetFont(16),
                X = xOffset + 110,
                Y = yOffset + 135,
                Height = 30,
                Width = 205
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _autoRunButton
            });

            _sendWave = new ButtonControl(this, clicked: (s) => { _game.QueueEnemies(); })
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"Send Wave",
                Font = BasicFonts.GetFont(16),
                X = xOffset + 110,
                Y = yOffset + 170,
                Height = 30,
                Width = 205
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _sendWave
            });
        }

        private void SetupPurchasingField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = xOffset,
                    Y = yOffset,
                    Height = height,
                    Width = width
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "Turrets",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = xOffset,
                Y = yOffset,
                Height = 30,
                Width = width
            });

            var turrets = TurretBuilder.GetTurrets();
            int offset = 0;
            foreach(var turretName in turrets)
            {
                var turret = TurretBuilder.GetTurret(turretName);
                var newTurretButton = new ButtonControl(this, clicked: BuyTurret_Click)
                {
                    Text = $"[{turret.Cost}$] {turret.Name}",
                    Font = BasicFonts.GetFont(12),
                    FillColor = BasicTextures.GetBasicRectange(Color.FloralWhite),
                    FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                    X = xOffset + 5,
                    Y = yOffset + 35 + (offset++ * 35),
                    Height = 30,
                    Width = width - 10,
                    Tag = turretName
                };
                _turretButtons.Add(newTurretButton);
                AddControl(1, new BorderControl(this)
                {
                    Child = newTurretButton
                });
            }

            _buyingPreviewRangeTile = new TileControl(this)
            {
                IsVisible = false,
                Alpha = 100
            };
            AddControl(10, _buyingPreviewRangeTile);
            _buyingPreviewTile = new TileControl(this)
            {
                IsVisible = false
            };
            AddControl(10, _buyingPreviewTile);
        }

        private void SetupUpgradeField(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = xOffset,
                    Y = yOffset,
                    Height = height,
                    Width = width
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "Upgrades",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            int x = xOffset + 5;
            int y = yOffset + 40;
            int itemXOffset = 0;
            int itemWidth = 205;
            int margin = 10;
            for(int i = 0; i < 3; i++)
            {
                var newItem = new UpgradePanel(this, BuyUpgrade_Click)
                {
                    X = x + (itemXOffset++ * (itemWidth + margin)),
                    Y = y,
                    Height = 135,
                    Width = itemWidth,
                };
                newItem.TurnInvisible();
                _turretUpgradePanels.Add(newItem);
                AddControl(1, newItem);
            }
        }

        private void SetupNextEnemyPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = xOffset,
                    Y = yOffset,
                    Height = height,
                    Width = width
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "Next Enemies",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            int x = xOffset + 5;
            int y = yOffset + 40;
            int itemXOffset = 0;
            int itemWidth = 185;
            int margin = 10;
            for (int i = 0; i < 5; i++)
            {
                var newItem = new LabelControl(this)
                {
                    X = x + (itemXOffset++ * (itemWidth + margin)),
                    Y = y,
                    Height = 65,
                    Width = itemWidth,
                    FillColor = BasicTextures.GetBasicRectange(Color.CadetBlue),
                    Font = BasicFonts.GetFont(12),
                };
                _nextEnemyPanels.Add(newItem);
                AddControl(1, newItem);
            }
        }

        private void SetupTurretStatsPanel(int xOffset, int yOffset, int width, int height)
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = xOffset,
                    Y = yOffset,
                    Height = height,
                    Width = width
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "Turret Stats",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = xOffset,
                Y = yOffset,
                Height = 35,
                Width = width
            });

            _turretStatesTextbox = new TextboxControl(this)
            {
                Font = BasicFonts.GetFont(10),
                Text = "Select a Turret",
                X = xOffset + 5,
                Y = yOffset + 40,
                Width = width - 10,
                Height = height - 45,
                FillColor = BasicTextures.GetBasicRectange(Color.Beige)
            };
            AddControl(1, _turretStatesTextbox);
        }
    }
}
