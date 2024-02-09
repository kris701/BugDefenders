using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TDGame.Core.Turret;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private CanvasControl _gameCanvas;
        private LabelControl _moneyLabel;
        private LabelControl _hpLabel;

        private ButtonControl _startButton;
        private ButtonControl _sendWave;
        private ButtonControl _autoRunButton;
        private StackPanelControl _nextWavePanel;

        private PanelControl _buyingPreviewPanel;
        private PanelControl _buyingPreviewRangePanel;
        private ButtonControl _buyGatlingTurretButton;
        private ButtonControl _buyRocketTurretButton;
        private ButtonControl _buyMissileTurretButton;

        public override void Initialize()
        {
            _moneyLabel = new LabelControl()
            {
                Text = $"Money: {_game.Money}$",
                Font = BasicFonts.Font24pt
            };
            _hpLabel = new LabelControl()
            {
                Text = $"HP: {_game.HP}",
                Font = BasicFonts.Font24pt
            };
            _buyGatlingTurretButton = new ButtonControl(clicked: BuyGatlingTurret_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Font = BasicFonts.Font16pt,
                Text = $"{TurretBuilder.GetTurret("turret1").Cost}$ Buy Gatling Turret",
            };
            _buyRocketTurretButton = new ButtonControl(clicked: BuyRocketTurret_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Font = BasicFonts.Font16pt,
                Text = $"{TurretBuilder.GetTurret("turret2").Cost}$ Buy Rocket Turret"
            };
            _buyMissileTurretButton = new ButtonControl(clicked: BuyMissileTurret_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Font = BasicFonts.Font16pt,
                Text = $"{TurretBuilder.GetTurret("turret3").Cost}$ Buy Missile Turret",
            };
            _buyingPreviewPanel = new PanelControl()
            {
                IsVisible = false,
                Width = 25,
                Height = 25
            };
            _buyingPreviewRangePanel = new PanelControl()
            {
                IsVisible = false,
                Width = 10,
                Height = 10
            };
            _gameCanvas = new CanvasControl() {
                FillColor = TextureBuilder.GetTexture(_game.Map.ID)
            };
            _startButton = new ButtonControl(clicked: StartButton_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"Pause",
                Font = BasicFonts.Font24pt
            };
            _autoRunButton = new ButtonControl(clicked: AutoRunButton_Click)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"[ ] Auto-Wave",
                Font = BasicFonts.Font24pt
            };
            _sendWave = new ButtonControl(clicked: (s) => { _game.QueueEnemies(); })
            {
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray),
                Text = $"Send Wave",
                Font = BasicFonts.Font24pt
            };
            _nextWavePanel = new StackPanelControl()
            {
                Margin = 2
            };
            Container = new GridControl()
            {
                ColumnDefinitions = new List<int>() { 2, 1 },
                Margin = 5,
                Children = new List<IControl>()
                {
                    new CanvasControl(){
                        Children = new List<IControl>()
                        {
                            _buyingPreviewRangePanel,
                            _buyingPreviewPanel
                        }
                    },
                    _gameCanvas,
                    new BorderControl()
                    {
                        Column = 1,
                        Child = new StackPanelControl()
                        {
                            FillColor = BasicTextures.GetBasicRectange(Color.Green),
                            Margin = 5,
                            Children = new List<IControl>()
                            {
                                _moneyLabel,
                                _hpLabel,
                                new BorderControl()
                                {
                                    Child = new StackPanelControl()
                                    {
                                        Margin = 2,
                                        Children = new List<IControl>()
                                        {
                                            _buyGatlingTurretButton,
                                            _buyRocketTurretButton,
                                            _buyMissileTurretButton
                                        }
                                    }
                                },
                                new BorderControl()
                                {
                                    Child = new StackPanelControl()
                                    {
                                        Margin = 2,
                                        Children = new List<IControl>()
                                        {
                                            _startButton,
                                            _sendWave,
                                            _autoRunButton
                                        }
                                    }
                                },
                                new BorderControl()
                                {
                                    Child = new StackPanelControl()
                                    {
                                        Margin = 2,
                                        Children = new List<IControl>()
                                        {
                                            new LabelControl()
                                            {
                                                Text = "Next Waves:",
                                                Font = BasicFonts.Font10pt
                                            },
                                            _nextWavePanel
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            base.Initialize();
        }
    }
}
