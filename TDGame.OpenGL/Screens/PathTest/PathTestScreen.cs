using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Screens.MainMenu;
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
using TDGame.OpenGL.Textures;

namespace Project1.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private TDGame.Core.Game _game;
        private string _buyingTurret;

        public PathTestScreen(IEngine parent) : base(parent)
        {
            _game = new TDGame.Core.Game("map1", "easy");
            OnUpdate += Game_OnUpdate;
            Initialize();
        }

        private void StartButton_Click(ButtonControl parent)
        {
            _game.Running = !_game.Running;
            if (_game.Running)
                _startButton.Text = "Pause";
            else
                _startButton.Text = "Start";
        }

        private void AutoRunButton_Click(ButtonControl parent)
        {
            _game.AutoSpawn = !_game.AutoSpawn;
            if (_game.AutoSpawn)
                _autoRunButton.Text = "[X] Auto-Wave";
            else
                _autoRunButton.Text = "[ ] Auto-Wave";
        }

        private void Game_OnUpdate(GameTime gameTime)
        {
            if (_game.GameOver)
                SwitchView(new MainMenu.MainMenu(this.Parent));

            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            _game.Update(gameTime.ElapsedGameTime);
            _gameCanvas.Children.Clear();
            foreach (var enemy in _game.CurrentEnemies)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = TextureBuilder.GetTexture(enemy.ID),
                    Width = 20,
                    Height = 20,
                    X = enemy.X - 10,
                    Y = enemy.Y - 10
                };
                _gameCanvas.Children.Add(newPanel);
            }
            bool any = false;
            foreach (var turret in _game.Turrets)
            {
                if (mouseState.X > turret.X - turret.Size / 2 && mouseState.X < turret.X + turret.Size / 2 &&
                    mouseState.Y > turret.Y - turret.Size / 2 && mouseState.Y < turret.Y + turret.Size / 2)
                {
                    _selectTurretRangePanel.FillColor = BasicTextures.GetBasicCircle(Color.Gray, turret.Range * 2);
                    _selectTurretRangePanel.X = turret.X - _selectTurretRangePanel.FillColor.Width / 2;
                    _selectTurretRangePanel.Y = turret.Y - _selectTurretRangePanel.FillColor.Height / 2;
                    _selectTurretRangePanel.Width = _selectTurretRangePanel.FillColor.Width;
                    _selectTurretRangePanel.Height = _selectTurretRangePanel.FillColor.Height;
                    _selectTurretRangePanel.IsVisible = true;
                    any = true;
                }

                switch (turret.Type)
                {
                    case TurretType.Bullets:
                        _gameCanvas.Children.Add(new PanelControl()
                        {
                            FillColor = BasicTextures.GetBasicRectange(Color.Red),
                            Width = turret.Size,
                            Height = turret.Size,
                            X = turret.X - turret.Size / 2,
                            Y = turret.Y - turret.Size / 2
                        });
                        if (turret.Targeting != null)
                        {
                            _gameCanvas.Children.Add(new LineControl()
                            {
                                Stroke = BasicTextures.GetBasicRectange(Color.OrangeRed),
                                X = turret.X,
                                Y = turret.Y,
                                X2 = turret.Targeting.X,
                                Y2 = turret.Targeting.Y,
                                Thickness = 3
                            });
                        }
                        break;
                    case TurretType.Rockets:
                        _gameCanvas.Children.Add(new PanelControl()
                        {
                            FillColor = BasicTextures.GetBasicRectange(Color.Blue),
                            Width = turret.Size,
                            Height = turret.Size,
                            X = turret.X - turret.Size / 2,
                            Y = turret.Y - turret.Size / 2,
                        });
                        break;
                    case TurretType.Missile:
                        _gameCanvas.Children.Add(new PanelControl()
                        {
                            FillColor = BasicTextures.GetBasicRectange(Color.Yellow),
                            Width = turret.Size,
                            Height = turret.Size,
                            X = turret.X - turret.Size / 2,
                            Y = turret.Y - turret.Size / 2,
                        });
                        break;
                }
            }
            if (!any)
                _selectTurretRangePanel.IsVisible = false;
            foreach (var rocket in _game.Rockets)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Cyan),
                    Width = 10,
                    Height = 10,
                    X = rocket.X - 5,
                    Y = rocket.Y - 5,
                };
                _gameCanvas.Children.Add(newPanel);
            }
            foreach (var missile in _game.Missiles)
            {
                var newPanel = new PanelControl()
                {
                    FillColor = BasicTextures.GetBasicRectange(Color.Gold),
                    Width = 10,
                    Height = 10,
                    X = missile.X - 5,
                    Y = missile.Y - 5,
                };
                _gameCanvas.Children.Add(newPanel);
            }

            _moneyLabel.Text = $"Money: {_game.Money}$";
            _hpLabel.Text = $"HP: {_game.HP}";

            if (_game.Money < TurretBuilder.GetTurret("turret1").Cost)
                _buyGatlingTurretButton.IsEnabled = false;
            else
                _buyGatlingTurretButton.IsEnabled = true;
            if (_game.Money < TurretBuilder.GetTurret("turret2").Cost)
                _buyRocketTurretButton.IsEnabled = false;
            else
                _buyRocketTurretButton.IsEnabled = true;
            if (_game.Money < TurretBuilder.GetTurret("turret3").Cost)
                _buyMissileTurretButton.IsEnabled = false;
            else
                _buyMissileTurretButton.IsEnabled = true;

            _nextWavePanel.Children.Clear();
            foreach (var item in _game.EnemiesToSpawn)
            {
                _nextWavePanel.Children.Add(new LabelControl()
                {
                    Text = $"{EnemyBuilder.GetEnemy(item,0).Name}",
                    Font = BasicFonts.GetFont(8)
                });
                _nextWavePanel.Refresh();
            }

            if (_buyingPreviewPanel.IsVisible)
            {
                _buyingPreviewPanel.X = mouseState.X - _buyingPreviewPanel.Width / 2;
                _buyingPreviewPanel.Y = mouseState.Y - _buyingPreviewPanel.Height / 2;
                _buyingPreviewRangePanel.X = mouseState.X - _buyingPreviewRangePanel.Width / 2;
                _buyingPreviewRangePanel.Y = mouseState.Y - _buyingPreviewRangePanel.Height / 2;
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (mouseState.X > 0 && mouseState.X < _gameCanvas.Width &&
                        mouseState.Y > 0 && mouseState.Y < _gameCanvas.Height)
                    {
                        if (_game.AddTurret(TurretBuilder.GetTurret(_buyingTurret), new WayPoint(mouseState.X, mouseState.Y)))
                        {
                            if (!keyState.IsKeyDown(Keys.LeftShift))
                            {
                                _buyingPreviewPanel.IsVisible = false;
                                _buyingPreviewRangePanel.IsVisible = false;
                            }
                        }
                    }
                } else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingPreviewPanel.IsVisible = false;
                    _buyingPreviewRangePanel.IsVisible = false;
                }
            }
        }

        private void BuyGatlingTurret_Click(ButtonControl parent)
        {
            _buyingPreviewPanel.IsVisible = true;
            _buyingPreviewRangePanel.IsVisible = true;
            _buyingTurret = "turret1";
            _buyingPreviewPanel.FillColor = BasicTextures.GetBasicRectange(Color.Red);
            _buyingPreviewRangePanel.FillColor = BasicTextures.GetBasicCircle(Color.Gray, TurretBuilder.GetTurret(_buyingTurret).Range * 2);
            _buyingPreviewRangePanel.Width = _buyingPreviewRangePanel.FillColor.Width;
            _buyingPreviewRangePanel.Height = _buyingPreviewRangePanel.FillColor.Height;
        }

        private void BuyRocketTurret_Click(ButtonControl parent)
        {
            _buyingPreviewPanel.IsVisible = true;
            _buyingPreviewRangePanel.IsVisible = true;
            _buyingTurret = "turret2";
            _buyingPreviewPanel.FillColor = BasicTextures.GetBasicRectange(Color.Blue);
            _buyingPreviewRangePanel.FillColor = BasicTextures.GetBasicCircle(Color.Gray, TurretBuilder.GetTurret(_buyingTurret).Range * 2);
            _buyingPreviewRangePanel.Width = _buyingPreviewRangePanel.FillColor.Width;
            _buyingPreviewRangePanel.Height = _buyingPreviewRangePanel.FillColor.Height;
        }

        private void BuyMissileTurret_Click(ButtonControl parent)
        {
            _buyingPreviewPanel.IsVisible = true;
            _buyingPreviewRangePanel.IsVisible = true;
            _buyingTurret = "turret3";
            _buyingPreviewPanel.FillColor = BasicTextures.GetBasicRectange(Color.Yellow);
            _buyingPreviewRangePanel.FillColor = BasicTextures.GetBasicCircle(Color.Gray, TurretBuilder.GetTurret(_buyingTurret).Range * 2);
            _buyingPreviewRangePanel.Width = _buyingPreviewRangePanel.FillColor.Width;
            _buyingPreviewRangePanel.Height = _buyingPreviewRangePanel.FillColor.Height;
        }
    }
}
