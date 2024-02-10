using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TDGame.Core.Turret;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private LabelControl _moneyLabel;
        private LabelControl _hpLabel;

        private ButtonControl _startButton;
        private ButtonControl _sendWave;
        private ButtonControl _autoRunButton;

        private TileControl _buyingPreviewTile;
        private TileControl _buyingPreviewRangeTile;

        private TileControl _turretSelectRangeTile;

        private List<ButtonControl> _turretButtons = new List<ButtonControl>();

        public override void Initialize()
        {
#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new PathTestScreen(Parent, _currentMap, _currentGameStyle)))
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
            SetupGameControlsField();
            SetupPurchasingField();
            SetupUpgradeField();


            
            base.Initialize();
        }

        private void SetupGameField()
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    ForceFit = true,
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

        private void SetupGameControlsField()
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = _gameArea.X + _gameArea.Width + 10,
                    Y = _gameArea.Y,
                    Height = 170,
                    Width = 320
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "TDGame",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = _gameArea.X + _gameArea.Width + 10,
                Y = _gameArea.Y,
                Height = 30,
                Width = 320
            });
            _startButton = new ButtonControl(this, clicked: StartButton_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"Pause",
                Font = BasicFonts.GetFont(16),
                X = _gameArea.X + _gameArea.Width + 15,
                Y = _gameArea.Y + 100,
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
                    X = _gameArea.X + _gameArea.Width + 15,
                    Y = _gameArea.Y + 135,
                    Height = 30,
                    Width = 100
                }
            });

            _moneyLabel = new LabelControl(this)
            {
                Text = $"Money: {_game.Money}$",
                Font = BasicFonts.GetFont(16),
                X = _gameArea.X + _gameArea.Width + 15,
                Y = _gameArea.Y + 35,
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
                X = _gameArea.X + _gameArea.Width + 15,
                Y = _gameArea.Y + 65,
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
                X = _gameArea.X + _gameArea.Width + 15 + 105,
                Y = _gameArea.Y + 100,
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
                X = _gameArea.X + _gameArea.Width + 15 + 105,
                Y = _gameArea.Y + 135,
                Height = 30,
                Width = 205
            };
            AddControl(1, new BorderControl(this)
            {
                Child = _sendWave
            });
        }

        private void SetupPurchasingField()
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = _gameArea.X + _gameArea.Width + 10,
                    Y = _gameArea.Y + 180,
                    Height = 470,
                    Width = 320
                }
            });
            AddControl(1, new LabelControl(this)
            {
                Text = "Turrets",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                X = _gameArea.X + _gameArea.Width + 10,
                Y = _gameArea.Y + 180,
                Height = 30,
                Width = 320
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
                    FillColor = BasicTextures.GetBasicRectange(Color.White),
                    FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                    X = _gameArea.X + _gameArea.Width + 15,
                    Y = _gameArea.Y + 215 + (offset++ * 35),
                    Height = 30,
                    Width = 310,
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
                IsVisible = false,
                ForceFit = true,
                Width = 50,
                Height = 50
            };
            AddControl(10, _buyingPreviewTile);
        }

        private void SetupUpgradeField()
        {
            AddControl(0, new BorderControl(this)
            {
                Child = new TileControl(this)
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Beige),
                    Alpha = 100,
                    X = _gameArea.X,
                    Y = _gameArea.Y + _gameArea.Height + 10,
                    Height = 320,
                    Width = 1000 - 20
                }
            });
        }
    }
}
