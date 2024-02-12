using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseScreen
    {
        private Rectangle _gameArea = new Rectangle(10, 10, 650, 650);

        private EntityUpdater<TurretDefinition, TurretControl> _turretUpdater;
        private EntityUpdater<EnemyDefinition, EnemyControl> _enemyUpdater;
        private EntityUpdater<ProjectileDefinition, TileControl> _projectile;

        private string _currentMap;
        private string _currentGameStyle;
        private Core.Game _game;
        private string _buyingTurret = "";
        private TurretDefinition? _selectedTurret;
        private List<AnimatedTileControl> _explosions = new List<AnimatedTileControl>();

        private KeyWatcher _waveKeyWatcher;
        private int tabIndex = 0;
        private KeyWatcher _switchTurretWatcher;
        private KeyWatcher _escapeKeyWatcher;

        public GameScreen(GameEngine parent, string map, string gamestyle) : base(parent)
        {
            _currentGameStyle = gamestyle;
            _currentMap = map;
            ScaleValue = parent.Settings.Scale;
            _game = new Core.Game(map, gamestyle);
            _turretUpdater = new EntityUpdater<TurretDefinition, TurretControl>(4, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater = new EntityUpdater<EnemyDefinition, EnemyControl>(3, this, _gameArea.X, _gameArea.Y);
            _projectile = new EntityUpdater<ProjectileDefinition, TileControl>(5, this,  _gameArea.X, _gameArea.Y);
            _projectile.OnDelete += OnProjectileDeleted;
            _waveKeyWatcher = new KeyWatcher(Keys.Space, () => { 
                _game.QueueEnemies();
                _sendWave.FillColor = BasicTextures.GetBasicRectange(Color.Gray);
            }, () => {
                _sendWave.FillColor = BasicTextures.GetBasicRectange(Color.White);
            });
            _switchTurretWatcher = new KeyWatcher(Keys.Tab, () =>
            {
                if (_game.Turrets.Count == 0)
                    return;
                tabIndex++;
                if (tabIndex >= _game.Turrets.Count)
                    tabIndex = 0;
                UnselectTurret();
                SelectTurret(_game.Turrets[tabIndex]);
            });
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, UnselectTurret);
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

        private void SellTurret_Click(ButtonControl parent)
        {
            if (_selectedTurret != null)
            {
                _game.SellTurret(_selectedTurret);
                UnselectTurret();
            }
        }

        private void OnProjectileDeleted(TileControl parent)
        {
            if (parent.Tag is ProjectileDefinition projDef)
            {
                if (!projDef.IsExplosive)
                    return;
                var newTile = new AnimatedTileControl(this)
                {
                    X = projDef.X + projDef.Size / 2 - projDef.SplashRange / 2,
                    Y = projDef.Y + projDef.Size / 2 - projDef.SplashRange / 2,
                    FrameTime = TimeSpan.FromMilliseconds(100),
                    TileSet = TextureBuilder.GetTextureSet(new List<Guid>()
                    {
                        new Guid("ebca3566-e0bf-4aa1-9a29-74be54f3e96b"),
                        new Guid("d07d1ff5-1e67-454a-8e1b-3264705d2704"),
                        new Guid("ebca3566-e0bf-4aa1-9a29-74be54f3e96b"),
                        new Guid("d07d1ff5-1e67-454a-8e1b-3264705d2704"),
                        new Guid("ebca3566-e0bf-4aa1-9a29-74be54f3e96b"),
                        new Guid("d07d1ff5-1e67-454a-8e1b-3264705d2704"),
                    }),
                    AutoPlay = false,
                    Width = projDef.SplashRange,
                    Height = projDef.SplashRange
                };
                newTile.OnAnimationDone += (p) => {
                    p.IsVisible = false;
                };
                newTile.Initialize();
                _explosions.Add(newTile);
                AddControl(7, newTile);
            }
        }

        private void UnselectTurret()
        {
            _selectedTurret = null;
            _turretSelectRangeTile.IsVisible = false;
            foreach (var item in _turretUpgradePanels)
                item.TurnInvisible();
            _turretStatesTextbox.Text = "Select a Turret";
            _sellTurretButton.IsEnabled = false;
            _sellTurretButton.Text = $"Sell Turret";
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
                SelectTurret(turretDef);
            }
        }

        private void SelectTurret(TurretDefinition turretDef)
        {
            _selectedTurret = turretDef;
            _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, turretDef.Range * 2);
            _turretSelectRangeTile.Width = _turretSelectRangeTile.FillColor.Width;
            _turretSelectRangeTile.Height = _turretSelectRangeTile.FillColor.Height;
            _turretSelectRangeTile.X = turretDef.X + turretDef.Size / 2 + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile.Y = turretDef.Y + turretDef.Size / 2 + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
            _turretSelectRangeTile.IsVisible = true;

            var index = 0;
            for (int i = 0; i < turretDef.TurretLevels.Count; i++)
            {
                if (index >= _turretUpgradePanels.Count)
                    break;
                if (!turretDef.TurretLevels[i].HasUpgrade)
                    _turretUpgradePanels[index++].SetUpgrade(turretDef.TurretLevels[i], _game.CanLevelUpTurret(turretDef, i));
            }
            for (int i = 0; i < turretDef.ProjectileLevels.Count; i++)
            {
                if (index >= _turretUpgradePanels.Count)
                    break;
                if (!turretDef.ProjectileLevels[i].HasUpgrade)
                    _turretUpgradePanels[index++].SetUpgrade(turretDef.ProjectileLevels[i], _game.CanLevelUpProjectile(turretDef, i));
            }

            _turretStatesTextbox.Text = GetTurretDescriptionString(turretDef);
            _sellTurretButton.Text = $"[{_game.GetTurretWorth(_selectedTurret)}$] Sell Turret";
            _sellTurretButton.IsEnabled = true;
        }

        private string GetTurretDescriptionString(TurretDefinition turretDef)
        {
            var sb = new StringBuilder();
            switch (turretDef.Type)
            {
                case TurretType.Laser: sb.AppendLine("Laser Type Turret"); break;
                case TurretType.Projectile: sb.AppendLine("Projectile Type Turret"); break;
            }
            sb.AppendLine($"Name: {turretDef.Name}");
            sb.AppendLine($"Range: {turretDef.Range}");
            sb.AppendLine($"Cooldown: {turretDef.Cooldown}");
            if (turretDef.ProjectileID == null)
            {
                sb.AppendLine($"Damage: {turretDef.Damage}");
                if (turretDef.StrongAgainst.Count > 0 && _game.GameStyle.StrengthModifier != 1)
                {
                    string strengths = "";
                    foreach (var strength in turretDef.StrongAgainst)
                        strengths += $" {EnemyDefinition.GetEnemyTypeName(strength)}";
                    sb.AppendLine($"Strong Against:{strengths}");
                }
                if (turretDef.WeakAgainst.Count > 0 && _game.GameStyle.WeaknessModifier != 1)
                {
                    string weaknesses = "";
                    foreach (var weakness in turretDef.WeakAgainst)
                        weaknesses += $" {EnemyDefinition.GetEnemyTypeName(weakness)}";
                    sb.AppendLine($"Weak Against:{weaknesses}");
                }
            }
            else
            {
                var projectile = _game.GetProjectileForTurret(turretDef);
                sb.AppendLine($"Projectile Damage: {projectile.Damage}");
                sb.AppendLine($"Projectile Splash: {projectile.SplashRange}");
                sb.AppendLine($"Projectile Trigger: {projectile.TriggerRange}");
                if (projectile.StrongAgainst.Count > 0 && _game.GameStyle.StrengthModifier != 1)
                {
                    string strengths = "";
                    foreach (var strength in projectile.StrongAgainst)
                        strengths += $" {EnemyDefinition.GetEnemyTypeName(strength)}";
                    sb.AppendLine($"Strong Against:{strengths}");
                }
                if (projectile.WeakAgainst.Count > 0 && _game.GameStyle.WeaknessModifier != 1)
                {
                    string weaknesses = "";
                    foreach (var weakness in projectile.WeakAgainst)
                        weaknesses += $" {EnemyDefinition.GetEnemyTypeName(weakness)}";
                    sb.AppendLine($"Weak Against:{weaknesses}");
                }
            }
            sb.AppendLine($"Range: {turretDef.Range}");
            sb.AppendLine($"Cooldown: {turretDef.Cooldown}");
            if (turretDef.Kills != 0)
            {
                sb.AppendLine(" ");
                sb.AppendLine($"Kills: {turretDef.Kills}");
            }
            return sb.ToString();
        }

        private void BuyUpgrade_Click(ButtonControl parent)
        {
            if (_selectedTurret != null && parent.Tag is IUpgrade upg)
            {
                if (upg is TurretLevel turretLevel)
                {
                    var index = _selectedTurret.TurretLevels.IndexOf(turretLevel);
                    if (_game.CanLevelUpTurret(_selectedTurret, index))
                    {
                        _game.LevelUpTurret(_selectedTurret, index);
                        _turretUpdater.GetItem(_selectedTurret).UpgradeTurretLevels(_selectedTurret);
                        UnselectTurret();
                    }
                }
                else if (upg is ProjectileLevel projLevel)
                {
                    var index = _selectedTurret.ProjectileLevels.IndexOf(projLevel);
                    if (_game.CanLevelUpProjectile(_selectedTurret, index))
                    {
                        _game.LevelUpProjectile(_selectedTurret, index);
                        _turretUpdater.GetItem(_selectedTurret).UpgradeTurretLevels(_selectedTurret);
                        UnselectTurret();
                    }
                }
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
                GameOver();

            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            _waveKeyWatcher.Update(keyState);
            _switchTurretWatcher.Update(keyState);
            _escapeKeyWatcher.Update(keyState);

            _game.Update(gameTime.ElapsedGameTime);
            
            _moneyLabel.Text = $"Money: {_game.Money}$";
            _hpLabel.Text = $"HP: {_game.HP}";
            _scoreLabel.Text = $"Score: {_game.Score}";

            UpdateTurretPurchaseButtons();
            UpdateNextEnemies();

            _turretUpdater.UpdateEntities(_game.Turrets, (e) =>
            {
                return new TurretControl(this, clicked: Turret_Click)
                {
                    IsEnabled = true,
                    FillClickedColor = TextureBuilder.GetTexture(e.ID),
                    FillDisabledColor = TextureBuilder.GetTexture(e.ID),
                    FillColor = TextureBuilder.GetTexture(e.ID),
                    X = _gameArea.X + e.X,
                    Y = _gameArea.Y + e.Y,
                    Width = e.Size,
                    Height = e.Size,
                    Rotation = e.Angle,
                    Tag = e
                };
            });
            _enemyUpdater.UpdateEntities(_game.CurrentEnemies, (e) => {
                return new EnemyControl(this, e)
                {
                    FillColor = TextureBuilder.GetTexture(e.ID),
                    X = e.X + _gameArea.X,
                    Y = e.Y + _gameArea.Y,
                    Width = e.Size,
                    Height = e.Size,
                    Rotation = e.Angle + (float)Math.PI / 2,
                    Tag = e
                };
            });
            _projectile.UpdateEntities(_game.Projectiles);
            UpdateLasers();
            UpdateExplosions(gameTime);

            if (mouseState.X >= Scale(_gameArea.X) && mouseState.X <= Scale(_gameArea.X) + Scale(_gameArea.Width) &&
                mouseState.Y >= Scale(_gameArea.Y) && mouseState.Y <= Scale(_gameArea.Y) + Scale(_gameArea.Height))
            {
                var relative = new FloatPoint(InvScale(mouseState.X - Scale(_gameArea.X)), InvScale(mouseState.Y - Scale(_gameArea.Y)));
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

        private void UpdateWithinGameField(MouseState mouseState, FloatPoint relativeMousePosition, KeyboardState keyState)
        {
            if (_buyingTurret != "")
            {
                _buyingPreviewTile.IsVisible = true;
                _buyingPreviewTile._x = mouseState.X - _buyingPreviewTile.Width / 2;
                _buyingPreviewTile._y = mouseState.Y - _buyingPreviewTile.Height / 2;
                _buyingPreviewRangeTile.IsVisible = true;
                _buyingPreviewRangeTile._x = mouseState.X - _buyingPreviewRangeTile.Width / 2;
                _buyingPreviewRangeTile._y = mouseState.Y - _buyingPreviewRangeTile.Height / 2;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    var newTurret = TurretBuilder.GetTurret(_buyingTurret);
                    newTurret.X = relativeMousePosition.X - newTurret.Size / 2;
                    newTurret.Y = relativeMousePosition.Y - newTurret.Size / 2;
                    if (_game.AddTurret(newTurret))
                    {
                        if (!keyState.IsKeyDown(Keys.LeftShift))
                        {
                            _buyingTurret = "";
                            _turretStatesTextbox.Text = "Select a Turret";
                            _buyingPreviewTile.IsVisible = false;
                            _buyingPreviewRangeTile.IsVisible = false;
                        }
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingTurret = "";
                    _turretStatesTextbox.Text = "Select a Turret";
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
                            X2 = _gameArea.X + turret.Targeting.X + turret.Targeting.Size / 2,
                            Y2 = _gameArea.Y + turret.Targeting.Y + turret.Targeting.Size / 2,
                        });
                    }
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            foreach (var explosion in _explosions)
                explosion.Update(gameTime);
            _explosions.RemoveAll(x => x.IsVisible == false);
        }

        private void UpdateNextEnemies()
        {
            for (int i = 0; i < _game.EnemiesToSpawn.Count && i < _nextEnemyPanels.Count; i++)
                _nextEnemyPanels[i].UpdateToEnemy(EnemyBuilder.GetEnemy(_game.EnemiesToSpawn[i], _game.Evolution));
        }

        private void BuyTurret_Click(ButtonControl parent)
        {
            if (parent.Tag is string turretName)
            {
                _buyingTurret = turretName;
                var turret = TurretBuilder.GetTurret(turretName);
                _buyingPreviewTile.FillColor = TextureBuilder.GetTexture(turret.ID);
                _buyingPreviewTile.Width = turret.Size;
                _buyingPreviewTile.Height = turret.Size;
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, turret.Range * 2);
                _buyingPreviewRangeTile.Width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile.Height = _buyingPreviewRangeTile.FillColor.Height;
                _turretStatesTextbox.Text = GetTurretDescriptionString(turret);
            }
        }

        private void GameOver()
        {
            var screen = Parent.TakeScreenCap();
            SwitchView(new GameOverScreen.GameOverScreen(Parent, screen, _game.Score, _game.GameTime));
        }
    }
}
