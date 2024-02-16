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
            _game.OnTurretShooting += OnTurretFiring;
            _game.OnTurretIdle += OnTurretIdling;
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
                    TileSet = TextureManager.GetTextureSet(new Guid("ebca3566-e0bf-4aa1-9a29-74be54f3e96b")),
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

        private void Turret_Click(AnimatedButtonControl parent)
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
            _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, (int)(GetRangeOfTurret(turret) * 2));
            _turretSelectRangeTile.Width = _turretSelectRangeTile.FillColor.Width;
            _turretSelectRangeTile.Height = _turretSelectRangeTile.FillColor.Height;
            _turretSelectRangeTile.X = turret.X + turret.Size / 2 + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile.Y = turret.Y + turret.Size / 2 + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
            _turretSelectRangeTile.IsVisible = true;

            int index = 0;
            foreach(var upgrade in turret.GetDefinition().Upgrades)
                if (!turret.HasUpgrades.Contains(upgrade.ID))
                    _turretUpgradePanels[index++].SetUpgrade(upgrade, _game.CanLevelUpTurret(turret, upgrade.ID));

            _turretStatesTextbox.Text = GetTurretDescriptionString(turret);
            _sellTurretButton.Text = $"[{_selectedTurret.GetTurretWorth()}$] Sell Turret";
            _sellTurretButton.IsEnabled = true;
        }

        private string GetTurretDescriptionString(TurretInstance turret)
        {
            var sb = new StringBuilder();
            sb.AppendLine(turret.TurretInfo.GetDescriptionString());
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
                    FillClickedColor = BasicTextures.GetBasicRectange(Color.Transparent),
                    FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                    TileSet = TextureManager.GetTextureSet(e.DefinitionID),
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
                    FillColor = TextureManager.GetTexture(e.DefinitionID),
                    X = e.X + _gameArea.X,
                    Y = e.Y + _gameArea.Y,
                    Width = e.Size,
                    Height = e.Size,
                    Rotation = e.Angle + (float)Math.PI / 2,
                    Tag = e
                };
            }, (e, i) =>
            {
                i.X = e.X + _gameArea.X + i.VisualOffset.X;
                i.Y = e.Y + _gameArea.Y + i.VisualOffset.Y;
                i.Rotation = e.Angle + (float)Math.PI / 2;
            });
            _projectile.UpdateEntities(_game.ProjectileTurretsModule.Projectiles, (e) =>
            {
                return new TileControl(this)
                {
                    FillColor = TextureManager.GetTexture(e.DefinitionID),
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

        public void OnTurretFiring(TurretInstance turret)
        {
            var control = _turretUpdater.GetItem(turret);
            if (control == null)
                return;
            if (turret.DefinitionID == new Guid("468b3259-bd65-4aa1-9e40-5fb5e455b847"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("9448e307-2401-4060-9bc2-12698f86aa73"));
            if (turret.DefinitionID == new Guid("e1e5d65f-b09e-4f71-a743-67ead199cdfc"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("37de2d39-b883-491d-a70b-536965aac408"));
            if (turret.DefinitionID == new Guid("9f22ceed-4d97-4ac6-846a-405ece02d64a"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("624e9637-9a13-4d0a-8b6f-35c4e9198618"));
            if (turret.DefinitionID == new Guid("1a252e16-8c8c-4179-8b1c-75b2b8f47704"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("9dca5c04-36c5-4eb5-bd36-a7f8ba25562e"));
            if (turret.DefinitionID == new Guid("c402592b-1af3-4bb5-a7e6-9e285dfc31a2"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("63afea5b-f247-4a86-8648-2c878a21a7ca"));
            control.Initialize();
        }

        public void OnTurretIdling(TurretInstance turret)
        {
            var control = _turretUpdater.GetItem(turret);
            if (control == null)
                return;
            if (turret.DefinitionID == new Guid("468b3259-bd65-4aa1-9e40-5fb5e455b847"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("468b3259-bd65-4aa1-9e40-5fb5e455b847"));
            if (turret.DefinitionID == new Guid("e1e5d65f-b09e-4f71-a743-67ead199cdfc"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("e1e5d65f-b09e-4f71-a743-67ead199cdfc"));
            if (turret.DefinitionID == new Guid("9f22ceed-4d97-4ac6-846a-405ece02d64a"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("9f22ceed-4d97-4ac6-846a-405ece02d64a"));
            if (turret.DefinitionID == new Guid("1a252e16-8c8c-4179-8b1c-75b2b8f47704"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("1a252e16-8c8c-4179-8b1c-75b2b8f47704"));
            if (turret.DefinitionID == new Guid("c402592b-1af3-4bb5-a7e6-9e285dfc31a2"))
                control.TileSet = TextureManager.GetTextureSet(new Guid("c402592b-1af3-4bb5-a7e6-9e285dfc31a2"));
            control.Initialize();
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
                if (turret.TurretInfo is LaserTurretDefinition)
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
                _buyingPreviewTile.TileSet = TextureManager.GetTextureSet(turret.ID);
                _buyingPreviewTile.Width = turret.Size;
                _buyingPreviewTile.Height = turret.Size;
                _buyingPreviewTile.Initialize();
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, (int)(GetRangeOfTurret(turret) * 2));
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

        private float GetRangeOfTurret(TurretInstance turret)
        {
            switch (turret.TurretInfo)
            {
                case AOETurretDefinition def: return def.Range;
                case LaserTurretDefinition def: return def.Range;
                case ProjectileTurretDefinition def: return def.Range;
            }
            return 1;
        }

        private float GetRangeOfTurret(TurretDefinition turret)
        {
            switch (turret.TurretType)
            {
                case AOETurretDefinition def: return def.Range;
                case LaserTurretDefinition def: return def.Range;
                case ProjectileTurretDefinition def: return def.Range;
            }
            return 1;
        }
    }
}
