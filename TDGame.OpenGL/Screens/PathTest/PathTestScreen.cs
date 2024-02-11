using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TDGame.Core;
using TDGame.Core.Enemies;
using TDGame.Core.Maps;
using TDGame.Core.Turret;
using TDGame.Core.Turrets;
using TDGame.Core.Turrets.Upgrades;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.PathTest
{
    public partial class PathTestScreen : BaseScreen
    {
        private Rectangle _gameArea = new Rectangle(10, 10, 650, 650);

        private EntityUpdater<TurretDefinition> _turretUpdater;
        private EntityUpdater<EnemyDefinition> _enemyUpdater;
        private EntityUpdater<ProjectileDefinition> _projectile;

        private string _currentMap;
        private string _currentGameStyle;
        private Core.Game _game;
        private string _buyingTurret = "";
        private TurretDefinition? _selectedTurret;

        public PathTestScreen(GameEngine parent, string map, string gamestyle) : base(parent)
        {
            _currentGameStyle = gamestyle;
            _currentMap = map;
            ScaleValue = parent.Scale;
            _game = new Core.Game(map, gamestyle);
            _turretUpdater = new EntityUpdater<TurretDefinition>(4, this, 50, _gameArea.X, _gameArea.Y, Turret_Click);
            _enemyUpdater = new EntityUpdater<EnemyDefinition>(3, this, 20, _gameArea.X, _gameArea.Y);
            _projectile = new EntityUpdater<ProjectileDefinition>(5, this, 20, _gameArea.X, _gameArea.Y);
            Initialize();

#if DRAWBLOCKINGTILES
            foreach (var blockingTile in _game.Map.BlockingTiles)
            {
                AddControl(99, new TileControl(this)
                {
                    X = blockingTile.X + _gameArea.X,
                    Y = blockingTile.Y + _gameArea.Y,
                    Width = blockingTile.Width,
                    Height = blockingTile.Height,
                    FillColor = BasicTextures.GetBasicRectange(Color.Red),
                    Alpha = 50
                });
            }
#endif
        }

        private void UnselectTurret()
        {
            _selectedTurret = null;
            _turretSelectRangeTile.IsVisible = false;
            _turretUpgrade1.TurnInvisible();
            _turretUpgrade2.TurnInvisible();
            _turretUpgrade3.TurnInvisible();
            _projectileUpgrade1.TurnInvisible();
            _projectileUpgrade2.TurnInvisible();
            _projectileUpgrade3.TurnInvisible();
        }

        private void Turret_Click(ButtonControl parent)
        {
            if (_buyingPreviewTile.IsVisible)
                return;
            if (_turretSelectRangeTile.IsVisible)
            {
                UnselectTurret();
            }
            else if (parent.Tag is TurretDefinition turretDef)
            {
                _selectedTurret = turretDef;
                _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, turretDef.Range * 2);
                _turretSelectRangeTile.Width = _turretSelectRangeTile.FillColor.Width;
                _turretSelectRangeTile.Height = _turretSelectRangeTile.FillColor.Height;
                _turretSelectRangeTile.X = turretDef.X + turretDef.Size / 2 + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
                _turretSelectRangeTile.Y = turretDef.Y + turretDef.Size / 2 + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
                _turretSelectRangeTile.IsVisible = true;

                for(int i = 0; i < turretDef.TurretLevels.Count; i++)
                {
                    if (!turretDef.TurretLevels[i].HasUpgrade)
                    {
                        if (!_turretUpgrade1.IsVisible)
                        {
                            _turretUpgrade1.SetUpgrade(turretDef.TurretLevels[i]);
                        } else if (!_turretUpgrade2.IsVisible)
                        {
                            _turretUpgrade2.SetUpgrade(turretDef.TurretLevels[i]);
                        } else if (!_turretUpgrade3.IsVisible)
                        {
                            _turretUpgrade3.SetUpgrade(turretDef.TurretLevels[i]);
                        }
                    }
                }

                for (int i = 0; i < turretDef.ProjectileLevels.Count; i++)
                {
                    if (!turretDef.ProjectileLevels[i].HasUpgrade)
                    {
                        if (!_projectileUpgrade1.IsVisible)
                        {
                            _projectileUpgrade1.SetUpgrade(turretDef.ProjectileLevels[i]);
                        }
                        else if (!_projectileUpgrade2.IsVisible)
                        {
                            _projectileUpgrade2.SetUpgrade(turretDef.ProjectileLevels[i]);
                        }
                        else if (!_projectileUpgrade3.IsVisible)
                        {
                            _projectileUpgrade3.SetUpgrade(turretDef.ProjectileLevels[i]);
                        }
                    }
                }
            }
        }

        private void BuyUpgrade_Click(ButtonControl parent)
        {
            if (_selectedTurret != null && parent.Tag is IUpgrade upg)
            {
                if (upg is TurretLevel turretLevel)
                {
                    var index = _selectedTurret.TurretLevels.IndexOf(turretLevel);
                    _game.LevelUpTurret(_selectedTurret, index);
                }
                else if (upg is ProjectileLevel projLevel)
                {
                    var index = _selectedTurret.ProjectileLevels.IndexOf(projLevel);
                    _game.LevelUpProjectile(_selectedTurret, index);
                }
                UnselectTurret();
            }
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

        public override void OnUpdate(GameTime gameTime)
        {
            if (_game.GameOver)
                Parent.SwitchView(new MainMenu.MainMenu(Parent));

            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            _game.Update(gameTime.ElapsedGameTime);
            
            _moneyLabel.Text = $"Money: {_game.Money}$";
            _hpLabel.Text = $"HP: {_game.HP}";

            UpdateTurretPurchaseButtons();

            _turretUpdater.UpdateEntities(_game.Turrets,
                (e) =>
                    {
                        return new ButtonControl(this, clicked: Turret_Click)
                        {
                            IsEnabled = true,
                            FillClickedColor = TextureBuilder.GetTexture(e.ID),
                            FillDisabledColor = TextureBuilder.GetTexture(e.ID),
                            FillColor = TextureBuilder.GetTexture(e.ID),
                            X = _gameArea.X + e.X,
                            Y = _gameArea.Y + e.Y,
                            Width = _turretUpdater.Size,
                            Height = _turretUpdater.Size,
                            Tag = e
                        };
                    });
            _enemyUpdater.UpdateEntities(_game.CurrentEnemies);
            _projectile.UpdateEntities(_game.Projectiles, 
                (e) =>
                    {
                        return new ButtonControl(this)
                        {
                            IsEnabled = false,
                            FillClickedColor = TextureBuilder.GetTexture(e.ID),
                            FillDisabledColor = TextureBuilder.GetTexture(e.ID),
                            FillColor = TextureBuilder.GetTexture(e.ID),
                            X = _gameArea.X + e.X,
                            Y = _gameArea.Y + e.Y,
                            Width = _projectile.Size,
                            Height = _projectile.Size,
                            Rotation = e.Angle + (float)Math.PI / 2,
                            Tag = e
                        };
                    }, 
                (e, c) =>
                    {
                        c.X = e.X + _gameArea.X;
                        c.Y = e.Y + _gameArea.Y;
                        c.Rotation = e.Angle + (float)Math.PI / 2;
                    });
            UpdateLasers();

            if (mouseState.X >= Scale(_gameArea.X) && mouseState.X <= Scale(_gameArea.X) + Scale(_gameArea.Width) &&
                mouseState.Y >= Scale(_gameArea.Y) && mouseState.Y <= Scale(_gameArea.Y) + Scale(_gameArea.Height))
            {
                var relative = new WayPoint(InvScale(mouseState.X - Scale(_gameArea.X)), InvScale(mouseState.Y - Scale(_gameArea.Y)));
                UpdateWithinGameField(mouseState, relative, keyState);
            }
            else
            {
                _buyingPreviewTile.IsVisible = false;
                _buyingPreviewRangeTile.IsVisible = false;
            }
        }

        private void UpdateTurretPurchaseButtons()
        {
            foreach (var turret in _turretButtons)
            {
                if (turret.Tag is string turretName)
                {
                    if (_game.Money < TurretBuilder.GetTurret(turretName).Cost)
                        turret.IsEnabled = false;
                    else
                        turret.IsEnabled = true;
                }
            }
        }

        private void UpdateWithinGameField(MouseState mouseState, WayPoint relativeMousePosition, KeyboardState keyState)
        {
            if (_buyingTurret != "")
            {
                _buyingPreviewTile.IsVisible = true;
                _buyingPreviewTile.Width = _buyingPreviewTile.FillColor.Width;
                _buyingPreviewTile.Height = _buyingPreviewTile.FillColor.Height;
                _buyingPreviewTile._x = mouseState.X - _buyingPreviewTile.Width / 2;
                _buyingPreviewTile._y = mouseState.Y - _buyingPreviewTile.Height / 2;
                _buyingPreviewRangeTile.IsVisible = true;
                _buyingPreviewRangeTile.Width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile.Height = _buyingPreviewRangeTile.FillColor.Height;
                _buyingPreviewRangeTile._x = mouseState.X - _buyingPreviewRangeTile.Width / 2;
                _buyingPreviewRangeTile._y = mouseState.Y - _buyingPreviewRangeTile.Height / 2;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    var newTurret = TurretBuilder.GetTurret(_buyingTurret);
                    newTurret.X = relativeMousePosition.X - Scale(newTurret.Size / 2);
                    newTurret.Y = relativeMousePosition.Y - Scale(newTurret.Size / 2);
                    if (_game.AddTurret(newTurret))
                    {
                        if (!keyState.IsKeyDown(Keys.LeftShift))
                        {
                            _buyingTurret = "";
                            _buyingPreviewTile.IsVisible = false;
                            _buyingPreviewRangeTile.IsVisible = false;
                        }
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingTurret = "";
                    _buyingPreviewTile.IsVisible = false;
                    _buyingPreviewRangeTile.IsVisible = false;
                }
            }
            if (_selectedTurret != null && mouseState.RightButton == ButtonState.Pressed)
                UnselectTurret();
        }

        private void UpdateLasers()
        {
            ClearLayer(6);
            foreach(var turret in _game.Turrets)
            {
                if (turret.Type == TurretType.Laser)
                {
                    if (turret.Targeting != null)
                    {
                        AddControl(6, new LineControl(this)
                        {
                            Thickness = 3,
                            Stroke = BasicTextures.GetBasicRectange(Color.Red),
                            X = _gameArea.X + turret.X + turret.Size / 2,
                            Y = _gameArea.Y + turret.Y + turret.Size / 2,
                            X2 = _gameArea.X + turret.Targeting.X + 10,
                            Y2 = _gameArea.Y + turret.Targeting.Y + 10,
                        });
                    }
                }
            }
        }

        private void BuyTurret_Click(ButtonControl parent)
        {
            if (parent.Tag is string turretName)
            {
                _buyingTurret = turretName;
                var turret = TurretBuilder.GetTurret(turretName);
                _buyingPreviewTile.FillColor = TextureBuilder.GetTexture(turret.ID);
                _buyingPreviewTile.Width = _buyingPreviewTile.FillColor.Width;
                _buyingPreviewTile.Height = _buyingPreviewTile.FillColor.Height;
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, turret.Range * 2);
            }
        }
    }
}
