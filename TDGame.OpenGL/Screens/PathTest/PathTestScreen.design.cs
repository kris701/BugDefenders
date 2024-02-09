using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TDGame.Core.Turret;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private CanvasControl _gameCanvas;
        private LabelControl _moneyLabel;
        private LabelControl _hpLabel;

        private PanelControl _buyingPreviewPanel;
        private ButtonControl _buyGatlingTurretButton;
        private ButtonControl _buyRocketTurretButton;
        private ButtonControl _buyMissileTurretButton;

        public override void Initialize()
        {
            _moneyLabel = new LabelControl()
            {
                Text = $"Money: {_game.Money}$",
                Font = BasicFonts.Font10pt
            };
            _hpLabel = new LabelControl()
            {
                Text = $"HP: {_game.HP}$",
                Font = BasicFonts.Font10pt
            };
            _buyGatlingTurretButton = new ButtonControl(clicked: BuyGatlingTurret_Click)
            {
                Text = $"{TurretBuilder.GetTurret("turret1").Cost}$ Buy Gatling Turret",
                Font = BasicFonts.Font10pt
            };
            _buyRocketTurretButton = new ButtonControl(clicked: BuyRocketTurret_Click)
            {
                Text = $"{TurretBuilder.GetTurret("turret2").Cost}$ Buy Rocket Turret",
                Font = BasicFonts.Font10pt
            };
            _buyMissileTurretButton = new ButtonControl(clicked: BuyMissileTurret_Click)
            {
                Text = $"{TurretBuilder.GetTurret("turret3").Cost}$ Buy Missile Turret",
                Font = BasicFonts.Font10pt
            };
            _buyingPreviewPanel = new PanelControl()
            {
                IsEnabled = false,
                Width = 10,
                Height = 10
            };
            _gameCanvas = new CanvasControl();
            Container = new GridControl()
            {
                ColumnDefinitions = new List<int>() { 2, 1 },
                Margin = 5,
                Children = new List<IControl>()
                {
                    new CanvasControl(){
                        Children = new List<IControl>()
                        {
                            _buyingPreviewPanel
                        }
                    },
                    _gameCanvas,
                    new StackPanelControl()
                    {
                        Column = 1,
                        FillColor = BasicTextures.GetBasicTexture(Color.Green),
                        Children = new List<IControl>()
                        {
                            _moneyLabel,
                            _hpLabel,
                            _buyGatlingTurretButton,
                            _buyRocketTurretButton,
                            _buyMissileTurretButton
                        }
                    }
                }
            };
            base.Initialize();
        }
    }
}
