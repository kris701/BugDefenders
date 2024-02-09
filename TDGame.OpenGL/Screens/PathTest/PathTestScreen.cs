using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.Core;
using TDGame.Core.Enemies;
using TDGame.Core.Maps;
using TDGame.Core.Turret;
using TDGame.Core.Turrets;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private TDGame.Core.Game _game;
        private TurretDefinition? _buyingTurret;

        public PathTestScreen(IEngine parent) : base(parent)
        {
            _game = new TDGame.Core.Game("map1", "easy");
            _game.AddTurret(TurretBuilder.GetTurret("turret1"), new WayPoint(175, 300));
            _game.AddTurret(TurretBuilder.GetTurret("turret2"), new WayPoint(225, 300));
            _game.AddTurret(TurretBuilder.GetTurret("turret3"), new WayPoint(275, 600));
            OnUpdate += Game_OnUpdate;
            Initialize();
        }

        private void Game_OnUpdate(GameTime gameTime)
        {
            _game.Update(gameTime.ElapsedGameTime);
            _gameCanvas.Children.Clear();
            foreach (var blockingTile in _game.Map.BlockingTiles)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.GetBasicTexture(Color.Black),
                    Width = blockingTile.Width,
                    Height = blockingTile.Height,
                    X = blockingTile.X,
                    Y = blockingTile.Y,
                };
                _gameCanvas.Children.Add(newPanel);
            }
            foreach (var enemy in _game.CurrentEnemies)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.GetBasicTexture(Color.White),
                    Width = 10,
                    Height = 10,
                    X = enemy.X,
                    Y = enemy.Y,
                };
                _gameCanvas.Children.Add(newPanel);
            }
            foreach (var turret in _game.Turrets)
            {
                var newPanel = new PanelControl();
                switch (turret.Type)
                {
                    case TurretType.Bullets:
                        newPanel = new PanelControl()
                        {
                            FillColor = BasicTextures.GetBasicTexture(Color.Red),
                            Width = turret.Size,
                            Height = turret.Size,
                            X = turret.X,
                            Y = turret.Y,
                        };
                        break;
                    case TurretType.Rockets:
                        newPanel = new PanelControl()
                        {
                            FillColor = BasicTextures.GetBasicTexture(Color.Blue),
                            Width = turret.Size,
                            Height = turret.Size,
                            X = turret.X,
                            Y = turret.Y,
                        };
                        break;
                    case TurretType.Missile:
                        newPanel = new PanelControl()
                        {
                            FillColor = BasicTextures.GetBasicTexture(Color.Yellow),
                            Width = turret.Size,
                            Height = turret.Size,
                            X = turret.X,
                            Y = turret.Y,
                        };
                        break;
                }
                _gameCanvas.Children.Add(newPanel);
            }
            foreach (var rocket in _game.Rockets)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.GetBasicTexture(Color.Cyan),
                    Width = 10,
                    Height = 10,
                    X = rocket.X,
                    Y = rocket.Y,
                };
                _gameCanvas.Children.Add(newPanel);
            }
            foreach (var missile in _game.Missiles)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.GetBasicTexture(Color.Gold),
                    Width = 10,
                    Height = 10,
                    X = missile.X,
                    Y = missile.Y,
                };
                _gameCanvas.Children.Add(newPanel);
            }

            _moneyLabel.Text = $"Money: {_game.Money}$";
            _hpLabel.Text = $"HP: {_game.HP}$";

            if (_buyingPreviewPanel.IsEnabled)
            {
                var mouseState = Mouse.GetState();
                _buyingPreviewPanel.X = mouseState.X;
                _buyingPreviewPanel.Y = mouseState.Y;
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_game.AddTurret(_buyingTurret, new WayPoint(mouseState.X, mouseState.Y)))
                        _buyingPreviewPanel.IsEnabled = false;
                } else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingPreviewPanel.IsEnabled = false;
                }
            }
        }

        private void BuyGatlingTurret_Click(ButtonControl parent)
        {
            _buyingPreviewPanel.IsEnabled = true;
            _buyingPreviewPanel.FillColor = BasicTextures.GetBasicTexture(Color.Red);
            _buyingTurret = TurretBuilder.GetTurret("turret1");
        }

        private void BuyRocketTurret_Click(ButtonControl parent)
        {
            _buyingPreviewPanel.IsEnabled = true;
            _buyingPreviewPanel.FillColor = BasicTextures.GetBasicTexture(Color.Blue);
            _buyingTurret = TurretBuilder.GetTurret("turret2");
        }

        private void BuyMissileTurret_Click(ButtonControl parent)
        {
            _buyingPreviewPanel.IsEnabled = true;
            _buyingPreviewPanel.FillColor = BasicTextures.GetBasicTexture(Color.Yellow);
            _buyingTurret = TurretBuilder.GetTurret("turret3");
        }
    }
}
