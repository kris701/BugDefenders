using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Entities.Upgrades;
using TDGame.Core.Models.Maps;
using TDGame.Core.Resources;
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

        private EntityUpdater<TurretInstance, TurretControl> _turretUpdater;
        private EntityUpdater<EnemyInstance, EnemyControl> _enemyUpdater;
        private EntityUpdater<ProjectileInstance, TileControl> _projectile;

        private Guid _currentMap;
        private Guid _currentGameStyle;
        private Core.Game _game;
        private Guid? _buyingTurret;
        private TurretInstance? _selectedTurret;
        private List<AnimatedTileControl> _explosions = new List<AnimatedTileControl>();

        private KeyWatcher _waveKeyWatcher;
        private int tabIndex = 0;
        private KeyWatcher _switchTurretWatcher;
        private KeyWatcher _escapeKeyWatcher;

        public GameScreen(GameEngine parent, Guid mapID, Guid gameStyleID) : base(parent)
        {
            _currentGameStyle = gameStyleID;
            _currentMap = mapID;
            ScaleValue = parent.Settings.Scale;
            _game = new Core.Game(mapID, gameStyleID);
            ResourceManager.CheckGameIntegrity();
            _turretUpdater = new EntityUpdater<TurretInstance, TurretControl>(4, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater = new EntityUpdater<EnemyInstance, EnemyControl>(3, this, _gameArea.X, _gameArea.Y);
            _projectile = new EntityUpdater<ProjectileInstance, TileControl>(5, this,  _gameArea.X, _gameArea.Y);
            _projectile.OnDelete += OnProjectileDeleted;
            _waveKeyWatcher = new KeyWatcher(Keys.Space, () => { 
                _game.QueueEnemies();
                _sendWave.FillColor = BasicTextures.GetBasicRectange(Color.DarkGray);
            }, () => {
                _sendWave.FillColor = BasicTextures.GetBasicRectange(Color.Gray);
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
            if (parent.Tag is ProjectileInstance projDef)
            {
                if (!projDef.GetDefinition().IsExplosive)
                    return;
                var newTile = new AnimatedTileControl(this)
                {
                    X = projDef.X + projDef.Size / 2 - projDef.GetDefinition().SplashRange / 2,
                    Y = projDef.Y + projDef.Size / 2 - projDef.GetDefinition().SplashRange / 2,
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
                    Width = projDef.GetDefinition().SplashRange,
                    Height = projDef.GetDefinition().SplashRange
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
                UnselectTurret();
            else if (parent.Tag is TurretInstance turretDef)
                SelectTurret(turretDef);
        }

        private void SelectTurret(TurretInstance turret)
        {
            _selectedTurret = turret;
            _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, (int)(turret.GetDefinition().Range * 2));
            _turretSelectRangeTile.Width = _turretSelectRangeTile.FillColor.Width;
            _turretSelectRangeTile.Height = _turretSelectRangeTile.FillColor.Height;
            _turretSelectRangeTile.X = turret.X + turret.Size / 2 + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile.Y = turret.Y + turret.Size / 2 + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
            _turretSelectRangeTile.IsVisible = true;

            int index = 0;
            foreach(var upgrade in turret.GetDefinition().GetAllUpgrades())
                if (!turret.HasUpgrades.Contains(upgrade.ID))
                    _turretUpgradePanels[index++].SetUpgrade(upgrade, _game.CanLevelUpTurret(turret, upgrade.ID));

            _turretStatesTextbox.Text = GetTurretDescriptionString(turret);
            _sellTurretButton.Text = $"[{_selectedTurret.GetTurretWorth()}$] Sell Turret";
            _sellTurretButton.IsEnabled = true;
        }

        private string GetTurretDescriptionString(TurretInstance turret)
        {
            var turretDef = turret.GetDefinition();
            var sb = new StringBuilder();
            switch (turretDef.Type)
            {
                case TurretType.Laser: sb.AppendLine("Laser Type Turret"); break;
                case TurretType.Projectile: sb.AppendLine("Projectile Type Turret"); break;
            }
            sb.AppendLine($"Name: {turretDef.Name}");
            sb.AppendLine($"Range: {turret.Range}");
            sb.AppendLine($"Cooldown: {turret.Cooldown}");
            if (turret.ProjectileDefinition == null)
            {
                sb.AppendLine($"Damage: {turret.Damage}");
                if (turretDef.DamageModifiers.Count > 0)
                {
                    foreach (var modifier in turretDef.DamageModifiers) 
                    {
                        var enemyType = ResourceManager.EnemyTypes.GetResource(modifier.EnemyType);
                        sb.AppendLine($"{enemyType.Name}: {modifier.Modifier}x ");
                    }
                }
            }
            else
            {
                var projectile = turret.ProjectileDefinition;
                sb.AppendLine($"Projectile Damage: {projectile.Damage}");
                sb.AppendLine($"Projectile Splash: {projectile.SplashRange}");
                sb.AppendLine($"Projectile Trigger: {projectile.TriggerRange}");
                if (projectile.DamageModifiers.Count > 0)
                {
                    foreach (var modifier in projectile.DamageModifiers)
                    {
                        var enemyType = ResourceManager.EnemyTypes.GetResource(modifier.EnemyType);
                        sb.AppendLine($"{enemyType.Name}: {modifier.Modifier}x ");
                    }
                }
            }
            if (turret.Kills != 0)
            {
                sb.AppendLine(" ");
                sb.AppendLine($"Kills: {turret.Kills}");
            }
            return sb.ToString();
        }

        private void BuyUpgrade_Click(ButtonControl parent)
        {
            if (_selectedTurret != null && parent.Tag is IUpgrade upg)
            {
                if (_game.CanLevelUpTurret(_selectedTurret, upg.ID))
                {
                    _game.LevelUpTurret(_selectedTurret, upg.ID);
                    _turretUpdater.GetItem(_selectedTurret).UpgradeTurretLevels(_selectedTurret);
                    UnselectTurret();
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
                    FillClickedColor = TextureBuilder.GetTexture(e.DefinitionID),
                    FillDisabledColor = TextureBuilder.GetTexture(e.DefinitionID),
                    FillColor = TextureBuilder.GetTexture(e.DefinitionID),
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
                    FillColor = TextureBuilder.GetTexture(e.DefinitionID),
                    X = e.X + _gameArea.X,
                    Y = e.Y + _gameArea.Y,
                    Width = e.Size,
                    Height = e.Size,
                    Rotation = e.Angle + (float)Math.PI / 2,
                    Tag = e
                };
            });
            _projectile.UpdateEntities(_game.Projectiles, (e) =>
            {
                return new TileControl(this)
                {
                    FillColor = TextureBuilder.GetTexture(e.DefinitionID),
                    X = e.X + _gameArea.X,
                    Y = e.Y + _gameArea.Y,
                    Width = e.Size,
                    Height = e.Size,
                    Rotation = e.Angle + (float)Math.PI / 2,
                    Tag = e
                };
            });
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
                if (turret.Tag is Guid turretName)
                {
                    if (_game.Money < ResourceManager.Turrets.GetResource(turretName).Cost)
                        turret.IsEnabled = false;
                    else
                        turret.IsEnabled = true;
                }
            }
        }

        private void UpdateWithinGameField(MouseState mouseState, FloatPoint relativeMousePosition, KeyboardState keyState)
        {
            if (_buyingTurret != null)
            {
                _buyingPreviewTile.IsVisible = true;
                _buyingPreviewTile._x = mouseState.X - _buyingPreviewTile.Width / 2;
                _buyingPreviewTile._y = mouseState.Y - _buyingPreviewTile.Height / 2;
                _buyingPreviewRangeTile.IsVisible = true;
                _buyingPreviewRangeTile._x = mouseState.X - _buyingPreviewRangeTile.Width / 2;
                _buyingPreviewRangeTile._y = mouseState.Y - _buyingPreviewRangeTile.Height / 2;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    var turretDef = ResourceManager.Turrets.GetResource((Guid)_buyingTurret);
                    var at = new FloatPoint(
                        relativeMousePosition.X - turretDef.Size / 2,
                        relativeMousePosition.Y - turretDef.Size / 2);
                    if (_game.AddTurret(turretDef, at))
                    {
                        if (!keyState.IsKeyDown(Keys.LeftShift))
                        {
                            _buyingTurret = null;
                            _turretStatesTextbox.Text = "Select a Turret";
                            _buyingPreviewTile.IsVisible = false;
                            _buyingPreviewRangeTile.IsVisible = false;
                        }
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingTurret = null;
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
                if (turret.GetDefinition().Type == TurretType.Laser)
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
                _nextEnemyPanels[i].UpdateToEnemy(new EnemyInstance(ResourceManager.Enemies.GetResource(_game.EnemiesToSpawn[i]), _game.Evolution));
        }

        private void BuyTurret_Click(ButtonControl parent)
        {
            if (parent.Tag is Guid turretID)
            {
                _buyingTurret = turretID;
                var turret = ResourceManager.Turrets.GetResource(turretID);
                _buyingPreviewTile.FillColor = TextureBuilder.GetTexture(turret.ID);
                _buyingPreviewTile.Width = turret.Size;
                _buyingPreviewTile.Height = turret.Size;
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, (int)(turret.Range * 2));
                _buyingPreviewRangeTile.Width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile.Height = _buyingPreviewRangeTile.FillColor.Height;
                _turretStatesTextbox.Text = GetTurretDescriptionString(new TurretInstance(turret));
            }
        }

        private void GameOver()
        {
            var screen = Parent.TakeScreenCap();
            SwitchView(new GameOverScreen.GameOverScreen(Parent, screen, _game.Score, _game.GameTime));
        }
    }
}
