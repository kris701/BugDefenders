﻿using BugDefender.Core.Game;
using BugDefender.Core.Game.Helpers;
#if RELEASE
using BugDefender.Core.Game.Helpers;
#endif
using BugDefender.Core.Game.Models.Entities;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.Core.Game.Modules.Turrets;
using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using MonoGame.OpenGL.Formatter.Input;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.GameView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BugDefender.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("2222e50b-cfcd-429b-9a21-3a3b77b4d87b");
        public static Rectangle _gameArea = new Rectangle(155, 10, 950, 950);

        private readonly EntityUpdater<TurretInstance, TurretControl> _turretUpdater;
        private readonly EntityUpdater<EnemyInstance, EnemyControl> _enemyUpdater;
        private readonly EntityUpdater<ProjectileInstance, AnimatedTileControl> _projectileUpdater;
        private readonly EntityUpdater<EffectEntity, AnimatedTileControl> _effectsUpdater;
        private readonly EntityUpdater<LaserEntity, LineControl> _laserUpdater;
        private readonly EntityUpdater<EnemyInstance, TileControl> _deadEnemyUpdater;

        private readonly GameEngine _game;
        private TurretDefinition? _buyingTurret;
        private TurretInstance? _selectedTurret;
        private readonly HashSet<EffectEntity> _effects = new HashSet<EffectEntity>();
        private readonly Dictionary<Guid, LaserEntity> _lasers = new Dictionary<Guid, LaserEntity>();
        private readonly HashSet<EnemyInstance> _deadEnemyInstances = new HashSet<EnemyInstance>();

        private readonly GameTimer _backgroundTasksTimer;
        private readonly GameTimer _gameTasksTimer;

        private readonly KeyWatcher _waveKeyWatcher;
        private int tabIndex = 0;
        private readonly KeyWatcher _switchTurretWatcher;
        private readonly KeyWatcher _escapeKeyWatcher;
        private bool _gameOverCheck = false;
        private bool _unselectTurret = false;
        private bool _selectTurret = false;
        private TimeSpan _hurtGameAreaTileShowTime = TimeSpan.Zero;

        public GameScreen(BugDefenderGameWindow parent, GameEngine game) : base(parent, _id)
        {
            _game = game;
            _game.TurretsModule.OnTurretShooting += OnTurretFiring;
            _game.TurretsModule.OnTurretIdle += OnTurretIdling;
            _game.OnPlayerDamaged += () =>
            {
                Parent.Audio.PlaySoundEffectOnce(new Guid("130c17d8-7cab-4fc0-8256-18092609f8d5"));
                if (_hurtGameAreaTile != null)
                {
                    _hurtGameAreaTile.IsVisible = true;
                    _hurtGameAreaTile.Alpha = 256;
                    _hurtGameAreaTileShowTime = TimeSpan.FromSeconds(1);
                }
            };

            _turretUpdater = new EntityUpdater<TurretInstance, TurretControl>(95, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater = new EntityUpdater<EnemyInstance, EnemyControl>(91, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater.OnDelete += OnEnemyDeath;
            _projectileUpdater = new EntityUpdater<ProjectileInstance, AnimatedTileControl>(92, this, _gameArea.X, _gameArea.Y);
            _projectileUpdater.OnDelete += OnProjectileDeleted;
            _effectsUpdater = new EntityUpdater<EffectEntity, AnimatedTileControl>(93, this, _gameArea.X, _gameArea.Y);
            _laserUpdater = new EntityUpdater<LaserEntity, LineControl>(92, this, _gameArea.X, _gameArea.Y);
            _deadEnemyUpdater = new EntityUpdater<EnemyInstance, TileControl>(120, this, _gameArea.X, _gameArea.Y);

            _waveKeyWatcher = new KeyWatcher(Keys.Space, () =>
            {
                if (_game.Context.CanSave())
                    Parent.UserManager.SaveGame(_game.GameSave);
                _sendWave?.DoClick();
            });
            _switchTurretWatcher = new KeyWatcher(Keys.Tab, () =>
            {
                if (_game.Context.Turrets.Count == 0)
                    return;
                tabIndex++;
                if (tabIndex >= _game.Context.Turrets.Count)
                    tabIndex = 0;
                _unselectTurret = true;
                _selectTurret = true;
                _selectedTurret = _game.Context.Turrets.ToList()[tabIndex];
            });
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, UnselectTurret);
            _backgroundTasksTimer = new GameTimer(TimeSpan.FromMilliseconds(100), OnUpdateBackground);
            _gameTasksTimer = new GameTimer(TimeSpan.FromMilliseconds(33), OnUpdateGame);
            Initialize();
            Parent.Audio.PlaySong(ID);

#if DEBUG && DRAWBLOCKINGTILES
            foreach (var blockingTile in _game.Context.Map.BlockingTiles)
            {
                AddControl(200, new TileControl()
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
#if DEBUG && DRAWMAPPATHS
            foreach (var path in _game.Context.Map.Paths)
            {
                var from = path[0];
                foreach (var waypoint in path.Skip(1))
                {
                    AddControl(200, new LineControl()
                    {
                        X = from.X + _gameArea.X,
                        Y = from.Y + _gameArea.Y,
                        X2 = waypoint.X + _gameArea.X,
                        Y2 = waypoint.Y + _gameArea.Y,
                        Alpha = 50,
                        Thickness = 5
                    });
                    from = waypoint;
                }
            }
#endif
        }

        private void SellTurret_Click(ButtonControl parent)
        {
            if (_selectedTurret != null)
            {
                var control = _turretUpdater.GetItem(_selectedTurret);
                if (control != null)
                    Parent.Audio.StopSoundEffect(control.CurrentSoundEffect);
                _game.TurretsModule.SellTurret(_selectedTurret);
                _selectedTurret = null;
                _unselectTurret = true;
            }
        }

        private void OnProjectileDeleted(AnimatedTileControl parent)
        {
            if (parent.Tag is ProjectileInstance projectile)
            {
                var entityDef = Parent.ResourcePackController.GetAnimation<ProjectileEntityDefinition>(projectile.DefinitionID);
                if (entityDef.OnDestroyed != Guid.Empty)
                {
                    var soundDef = Parent.ResourcePackController.GetSoundEffects<ProjectileEntityDefinition>(projectile.DefinitionID);
                    if (soundDef.OnDestroyed != Guid.Empty)
                        Parent.Audio.PlaySoundEffectOnce(soundDef.OnDestroyed);
                    var effect = Parent.Textures.GetTextureSet(entityDef.OnDestroyed);
                    _effects.Add(new EffectEntity(entityDef.OnDestroyed, TimeSpan.FromMilliseconds(250), effect)
                    {
                        X = projectile.CenterX - effect.GetLoadedContent()[0].Width / 2,
                        Y = projectile.CenterY - effect.GetLoadedContent()[0].Height / 2
                    });
                }
            }
        }

        private void SelectEnemy(ButtonControl button)
        {
            if (button.Tag is EnemyInstance instance)
            {
                _turretInfoPanel.Unselect();
                _turretInfoPanel.IsVisible = false;
                _enemyInfoPanel.Select(instance);
                _enemyInfoPanel.IsVisible = true;
            }
        }

        private void OnEnemyDeath(EnemyControl parent)
        {
            if (parent.Enemy.Health > 0)
                return;
            var entityDef = Parent.ResourcePackController.GetAnimation<EnemyEntityDefinition>(parent.Enemy.DefinitionID);
            if (entityDef.OnDeath != Guid.Empty)
            {
                var effect = Parent.Textures.GetTextureSet(entityDef.OnDeath);
                _effects.Add(new EffectEntity(entityDef.OnDeath, TimeSpan.FromMilliseconds(250), effect)
                {
                    X = parent.Enemy.CenterX - effect.GetLoadedContent()[0].Width / 2,
                    Y = parent.Enemy.CenterY - effect.GetLoadedContent()[0].Height / 2
                });
            }

            var deadEnemy = new EnemyInstance(parent.Enemy.GetDefinition(), 1);
            var rnd = new Random();
            if (rnd.Next(0, 2) == 1)
                deadEnemy.X = -75 + rnd.Next(-10, 11);
            else
                deadEnemy.X = 1675 + rnd.Next(-10, 11);
            deadEnemy.Y = -100;
            _deadEnemyInstances.Add(deadEnemy);
        }

        private void UnselectTurret()
        {
            _turretSelectRangeTile.IsVisible = false;
            _turretInfoPanel.Unselect();
            _upgradePageHandler.MaxPage = 0;
            _upgradePageHandler.MinPage = 0;
            _upgradePageHandler.MaxItem = 0;
            _upgradePageHandler.UpdatePages();
        }

        private void Turret_Click(ButtonControl parent)
        {
            if (_buyingPreviewTile.IsVisible)
                return;
            if (_turretSelectRangeTile.IsVisible)
                _unselectTurret = true;
            else if (parent.Tag is TurretInstance turret)
            {
                _selectedTurret = turret;
                _selectTurret = true;
            }
        }

        private void SelectTurret()
        {
            if (_selectedTurret == null)
                return;

            _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)GetRangeOfTurret(_selectedTurret.TurretInfo) * 2);
            _turretSelectRangeTile.Width = _turretSelectRangeTile.FillColor.Width;
            _turretSelectRangeTile.Height = _turretSelectRangeTile.FillColor.Height;
            _turretSelectRangeTile.X = _selectedTurret.CenterX + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile.Y = _selectedTurret.CenterY + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
            _turretSelectRangeTile.CalculateViewPort();
            _turretSelectRangeTile.IsVisible = true;

            SetTurretUpgradeField(_selectedTurret);
            _turretInfoPanel.SelectInstance(_selectedTurret);
            _turretInfoPanel.IsVisible = true;
            _enemyInfoPanel.Unselect();
            _enemyInfoPanel.IsVisible = false;
        }

        private void BuyUpgrade_Click(ButtonControl parent)
        {
            if (_selectedTurret != null && !_unselectTurret && parent.Tag is UpgradeDefinition upg)
            {
                if (_game.TurretsModule.CanUpgradeTurret(_selectedTurret, upg.ID) == TurretsModule.CanUpgradeResult.Success)
                {
                    Parent.Audio.PlaySoundEffectOnce(new Guid("aebfa031-8a3c-46c1-82dd-13a39d3caf36"));
                    _game.TurretsModule.UpgradeTurret(_selectedTurret, upg.ID);
                    var control = _turretUpdater.GetItem(_selectedTurret);
                    if (control != null)
                        control.UpgradeTurretLevels(_selectedTurret);
                    _selectTurret = true;
                    _unselectTurret = true;
                }
            }
        }

        private void StartButton_Click(ButtonControl parent)
        {
            _game.Running = !_game.Running;
            if (_game.Running)
            {
                Parent.Audio.ResumeSounds();
                parent.Text = "Pause";
            }
            else
            {
                Parent.Audio.PauseSounds();
                parent.Text = "Start";
            }
        }

        private void AutoRunButton_Click(ButtonControl parent)
        {
            _game.Context.AutoSpawn = !_game.Context.AutoSpawn;
            if (_game.Context.AutoSpawn)
                parent.Text = "[X] Auto-Wave";
            else
                parent.Text = "[ ] Auto-Wave";
        }

        public void OnUpdateBackground(TimeSpan passed)
        {
            _moneyLabel.Text = $"Money: {_game.Context.Money}$";
            _scoreLabel.Text = $"Wave {_game.Context.Wave}, Score {_game.Context.Stats.Score}, HP: {_game.Context.HP}";
            UpdateTurretPurchaseButtons();
            UpdateNextEnemies();
            _saveAndExitButton.IsEnabled = _game.Context.CanSave();
            UpdateGameInfoPanel();
            UpdateTurretUpgradePanel();
        }

        private void UpdateGameInfoPanel()
        {
            if (_game.Criterias.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var req in _game.Criterias)
                    sb.AppendLine(req.Progress(_game.Context.Stats));
                _gameInfoTextbox.Text = sb.ToString();
            }
            _playtimeLabel.Text = $"Game time: {_game.Context.Stats.GameTime.ToString("hh\\:mm\\:ss")}";
        }

        public void OnUpdateGame(TimeSpan passed)
        {
            _game.Update(passed);
            UpdateEffectLifetimes(passed);
            UpdateLasers();
            UpdateDeadEnemyList();
            _turretUpdater.UpdateEntities(_game.Context.Turrets, passed, CreateNewTurretControl);
            _enemyUpdater.UpdateEntities(_game.Context.CurrentEnemies.Enemies, passed, CreateNewEnemyControl, UpdateEnemyControl);
            _projectileUpdater.UpdateEntities(_game.Context.Projectiles, passed, CreateNewProjectileControl);
            _effectsUpdater.UpdateEntities(_effects, passed, CreateNewEffect);
            _deadEnemyUpdater.UpdateEntities(_deadEnemyInstances, passed, CreateNewDeadEnemyControl);
            _laserUpdater.UpdateEntities(_lasers.Values.ToHashSet(), passed, CreateNewLaser, UpdateLaserControl);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            if (_game.GameOver)
                GameOver();

            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            if (_unselectTurret)
            {
                UnselectTurret();
                _unselectTurret = false;
            }
            if (_selectTurret)
            {
                SelectTurret();
                _selectTurret = false;
            }

            _waveKeyWatcher.Update(keyState);
            _switchTurretWatcher.Update(keyState);
            _escapeKeyWatcher.Update(keyState);

            _backgroundTasksTimer.Update(gameTime.ElapsedGameTime);
            _gameTasksTimer.Update(gameTime.ElapsedGameTime);

            var translatedPos = InputHelper.GetRelativePosition(Parent.XScale, Parent.YScale);
            if (translatedPos.X >= _gameArea.X && translatedPos.X <= _gameArea.X + _gameArea.Width &&
                translatedPos.Y >= _gameArea.Y && translatedPos.Y <= _gameArea.Y + _gameArea.Height)
            {
                UpdateWithinGameField(mouseState, translatedPos, keyState);
            }
            else
            {
                _buyingPreviewTile.IsVisible = false;
                _buyingPreviewRangeTile.IsVisible = false;
            }

            if (_hurtGameAreaTileShowTime > TimeSpan.Zero)
            {
                _hurtGameAreaTileShowTime -= gameTime.ElapsedGameTime;
                var diff = (_hurtGameAreaTileShowTime / TimeSpan.FromSeconds(1)) * 256;
                _hurtGameAreaTile.Alpha = (int)diff;
                if (_hurtGameAreaTileShowTime <= TimeSpan.Zero)
                    _hurtGameAreaTile.IsVisible = false;
            }
        }

        private void UpdateTurretUpgradePanel()
        {
            if (_selectedTurret != null)
            {
                foreach (var item in _upgradePageHandler.Pages[_upgradePageHandler.PageIndex])
                {
                    if (item.Upgrade != null)
                    {
                        bool isLocked = false;
                        if (item.Upgrade.Requires != null)
                            isLocked = !_selectedTurret.HasUpgrades.Contains((Guid)item.Upgrade.Requires);
                        item.SetPurchasability(
                            _game.TurretsModule.CanUpgradeTurret(_selectedTurret, item.Upgrade.ID) == TurretsModule.CanUpgradeResult.Success,
                            isLocked);
                    }
                }
            }
        }

        private void UpdateEffectLifetimes(TimeSpan passed)
        {
            var toRemove = new List<EffectEntity>();
            foreach (var effect in _effects)
            {
                effect.LifeTime -= passed;
                if (effect.LifeTime <= TimeSpan.Zero)
                    toRemove.Add(effect);
            }
            foreach (var remove in toRemove)
                _effects.Remove(remove);
        }

        private TurretControl CreateNewTurretControl(TurretInstance entity)
        {
            return new TurretControl(Parent, entity, Turret_Click)
            {
                X = _gameArea.X + entity.X,
                Y = _gameArea.Y + entity.Y
            };
        }

        private EnemyControl CreateNewEnemyControl(EnemyInstance entity)
        {
            return new EnemyControl(Parent, entity)
            {
                X = entity.X + _gameArea.X,
                Y = entity.Y + _gameArea.Y
            };
        }

        private void UpdateEnemyControl(EnemyInstance entity, EnemyControl control, TimeSpan passed)
        {
            control.X = entity.X + _gameArea.X + control.VisualOffset.X;
            control.Y = entity.Y + _gameArea.Y + control.VisualOffset.Y;
            control.Rotation = entity.Angle + (float)Math.PI / 2;
        }

        private TileControl CreateNewDeadEnemyControl(EnemyInstance entity)
        {
            var animation = Parent.ResourcePackController.GetAnimation<EnemyEntityDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = Parent.Textures.GetTextureSet(animation);
            return new TileControl()
            {
                FillColor = textureSet.GetLoadedContent()[0],
                X = entity.X + _gameArea.X,
                Y = entity.Y + _gameArea.Y,
                Width = entity.Size,
                Height = entity.Size,
                Rotation = entity.Angle + (float)Math.PI / 2,
                Tag = entity
            };
        }

        private AnimatedTileControl CreateNewProjectileControl(ProjectileInstance entity)
        {
            if (Parent.ResourcePackController.HasSoundEffects<EffectEntityDefinition>(entity.DefinitionID))
            {
                var entityDef = Parent.ResourcePackController.GetSoundEffects<ProjectileEntityDefinition>(entity.DefinitionID);
                if (entityDef.OnCreate != Guid.Empty)
                    Parent.Audio.PlaySoundEffectOnce(entityDef.OnCreate);
            }
            var animation = Parent.ResourcePackController.GetAnimation<ProjectileEntityDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = Parent.Textures.GetTextureSet(animation);
            return new AnimatedTileControl()
            {
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                TileSet = textureSet.GetLoadedContent(),
                X = entity.X + _gameArea.X,
                Y = entity.Y + _gameArea.Y,
                Width = entity.Size,
                Height = entity.Size,
                Rotation = entity.Angle + (float)Math.PI / 2,
                Tag = entity
            };
        }

        private AnimatedTileControl CreateNewEffect(EffectEntity entity)
        {
            if (Parent.ResourcePackController.HasSoundEffects<EffectEntityDefinition>(entity.ID))
            {
                var entityDef = Parent.ResourcePackController.GetSoundEffects<EffectEntityDefinition>(entity.ID);
                if (entityDef.OnCreate != Guid.Empty)
                    Parent.Audio.PlaySoundEffectOnce(entityDef.OnCreate);
            }
            var newTile = new AnimatedTileControl()
            {
                X = _gameArea.X + entity.X,
                Y = _gameArea.Y + entity.Y,
                FrameTime = TimeSpan.FromMilliseconds(entity.TextureSetDefinition.FrameTime),
                TileSet = entity.TextureSetDefinition.GetLoadedContent(),
                AutoPlay = true,
                Width = entity.Size,
                Height = entity.Size,
                Tag = entity
            };
            newTile.ViewPort = _gameArea;
            return newTile;
        }

        private LineControl CreateNewLaser(LaserEntity entity)
        {
            var newTile = new LineControl()
            {
                Thickness = 3,
                Stroke = BasicTextures.GetBasicRectange(Color.Red),
                X = _gameArea.X + entity.From.CenterX,
                Y = _gameArea.Y + entity.From.CenterY,
                X2 = _gameArea.X + entity.To.CenterX,
                Y2 = _gameArea.Y + entity.To.CenterY,
                Tag = entity
            };
            return newTile;
        }

        private void UpdateLaserControl(LaserEntity entity, LineControl control, TimeSpan passed)
        {
            control.X = _gameArea.X + entity.From.CenterX;
            control.Y = _gameArea.Y + entity.From.CenterY;
            control.X2 = _gameArea.X + entity.To.CenterX;
            control.Y2 = _gameArea.Y + entity.To.CenterY;
        }

        private void UpdateTurretPurchaseButtons()
        {
            foreach (var turret in _turretPageHandler.Pages[_turretPageHandler.PageIndex])
                turret.SetPurchasability(_game.Context.Money >= turret.Turret.Cost, _game.Context.Wave < turret.Turret.AvailableAtWave);
        }

        public void OnTurretFiring(TurretInstance turret)
        {
            var control = _turretUpdater.GetItem(turret);
            if (control == null)
                return;
            control.SetTurretAnimation(Parent.ResourcePackController.GetAnimation<TurretEntityDefinition>(turret.DefinitionID).OnShoot);
            Parent.Audio.StopSoundEffect(control.CurrentSoundEffect);
            control.CurrentSoundEffect = Parent.Audio.PlaySoundEffect(Parent.ResourcePackController.GetSoundEffects<TurretEntityDefinition>(turret.DefinitionID).OnShoot);
            if (turret.TurretInfo is AOETurretDefinition def)
            {
                var entityDef = Parent.ResourcePackController.GetAnimation<EffectEntityDefinition>(turret.DefinitionID);
                if (entityDef.OnCreate != Guid.Empty)
                {
                    var effect = Parent.Textures.GetTextureSet(entityDef.OnCreate);
                    _effects.Add(new EffectEntity(entityDef.OnCreate, TimeSpan.FromSeconds(1), effect)
                    {
                        X = turret.CenterX - def.Range,
                        Y = turret.CenterY - def.Range,
                        Size = def.Range * 2
                    });
                }
            }
        }

        public void OnTurretIdling(TurretInstance turret)
        {
            var control = _turretUpdater.GetItem(turret);
            if (control == null)
                return;
            control.SetTurretAnimation(Parent.ResourcePackController.GetAnimation<TurretEntityDefinition>(turret.DefinitionID).OnIdle);
            Parent.Audio.StopSoundEffect(control.CurrentSoundEffect);
        }

        private void UpdateWithinGameField(MouseState mouseState, FloatPoint relativeMousePosition, KeyboardState keyState)
        {
            if (_buyingTurret != null)
            {
                _buyingPreviewTile.IsVisible = true;
                _buyingPreviewTile.X = relativeMousePosition.X - _buyingPreviewTile.Width / 2;
                _buyingPreviewTile.Y = relativeMousePosition.Y - _buyingPreviewTile.Height / 2;
                _buyingPreviewTile.CalculateViewPort();
                _buyingPreviewRangeTile.IsVisible = true;
                _buyingPreviewRangeTile.X = relativeMousePosition.X - _buyingPreviewRangeTile.Width / 2;
                _buyingPreviewRangeTile.Y = relativeMousePosition.Y - _buyingPreviewRangeTile.Height / 2;
                _buyingPreviewRangeTile.CalculateViewPort();
                _buyingPreviewSizeTile.IsVisible = true;
                _buyingPreviewSizeTile.X = relativeMousePosition.X - _buyingPreviewSizeTile.Width / 2;
                _buyingPreviewSizeTile.Y = relativeMousePosition.Y - _buyingPreviewSizeTile.Height / 2;
                _buyingPreviewSizeTile.CalculateViewPort();
                var at = new Core.Models.FloatPoint(
                    relativeMousePosition.X - _buyingTurret.Size / 2 - _gameArea.X,
                    relativeMousePosition.Y - _buyingTurret.Size / 2 - _gameArea.Y);
                if (!_game.TurretsModule.IsTurretPlacementOk(_buyingTurret, at))
                {
                    _buyingPreviewRangeTile.IsVisible = false;
                    _buyingPreviewTile.Alpha = 100;
                }
                else
                {
                    _buyingPreviewRangeTile.IsVisible = true;
                    _buyingPreviewTile.Alpha = 255;
                }

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_game.TurretsModule.AddTurret(_buyingTurret, at) == TurretsModule.AddTurretResult.Success)
                    {
                        if (!keyState.IsKeyDown(Keys.LeftShift))
                        {
                            _buyingTurret = null;
                            _turretInfoPanel.Unselect();
                            _buyingPreviewTile.IsVisible = false;
                            _buyingPreviewRangeTile.IsVisible = false;
                            _buyingPreviewSizeTile.IsVisible = false;
                        }
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    _buyingTurret = null;
                    _turretInfoPanel.Unselect();
                    _buyingPreviewTile.IsVisible = false;
                    _buyingPreviewRangeTile.IsVisible = false;
                    _buyingPreviewSizeTile.IsVisible = false;
                }
            }
            if (_selectedTurret != null && mouseState.RightButton == ButtonState.Pressed)
            {
                _selectedTurret = null;
                _unselectTurret = true;
            }
        }

        private void UpdateLasers()
        {
            var found = new List<Guid>();
            foreach (var turret in _game.Context.Turrets)
            {
                if (turret.TurretInfo is LaserTurretDefinition)
                {
                    found.Add(turret.ID);
                    if (_lasers.ContainsKey(turret.ID))
                    {
                        if (turret.Targeting == null)
                            _lasers.Remove(turret.ID);
                        else if (_lasers[turret.ID].To != turret.Targeting)
                            _lasers.Remove(turret.ID);
                    }
                    else if (!_lasers.ContainsKey(turret.ID) && turret.Targeting != null)
                        _lasers.Add(turret.ID, new LaserEntity(turret, turret.Targeting));
                }
            }
            var toRemove = new List<Guid>();
            foreach (var key in _lasers.Keys)
                if (!found.Contains(key))
                    toRemove.Add(key);
            foreach (var remove in toRemove)
                _lasers.Remove(remove);
        }

        private void UpdateDeadEnemyList()
        {
            var toRemove = new List<EnemyInstance>();
            foreach (var item in _deadEnemyInstances)
            {
                item.Y += 2;
                item.Angle += 0.15f;
                if (item.Y > IWindow.BaseScreenSize.Y)
                    toRemove.Add(item);
            }
            foreach (var remove in toRemove)
                _deadEnemyInstances.Remove(remove);
        }

        private void UpdateNextEnemies()
        {
            var rollingEvolution = _game.Context.Evolution;
            for (int i = 0; i < _game.Context.EnemiesToSpawn.Count && i < _nextEnemyPanels.Count; i++)
            {
                _nextEnemyPanels[i].UpdateToEnemy(_game.Context.EnemiesToSpawn[i], rollingEvolution);
                rollingEvolution *= _game.Context.GameStyle.EvolutionRate;
            }
        }

        private void BuyTurret_Click(ButtonControl parent)
        {
            if (parent.Tag is TurretDefinition def)
            {
                _buyingTurret = def;
                var animation = Parent.ResourcePackController.GetAnimation<TurretEntityDefinition>(def.ID).OnIdle;
                var textureSet = Parent.Textures.GetTextureSet(animation);
                _buyingPreviewTile.TileSet = textureSet.GetLoadedContent();
                _buyingPreviewTile.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
                _buyingPreviewTile.Width = def.Size;
                _buyingPreviewTile.Height = def.Size;
                _buyingPreviewTile.Initialize();
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)GetRangeOfTurret(def.ModuleInfo) * 2);
                _buyingPreviewRangeTile.Width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile.Height = _buyingPreviewRangeTile.FillColor.Height;
                _buyingPreviewSizeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)(def.Size));
                _buyingPreviewSizeTile.Width = _buyingPreviewSizeTile.FillColor.Width;
                _buyingPreviewSizeTile.Height = _buyingPreviewSizeTile.FillColor.Height;

                _turretInfoPanel.SelectDefinition(def);
                _turretInfoPanel.IsVisible = true;
                _enemyInfoPanel.Unselect();
                _enemyInfoPanel.IsVisible = false;
            }
        }

        private void SaveAndGoToMainMenu()
        {
            if (_game.Context.CanSave())
            {
                Parent.UserManager.SaveGame(_game.GameSave);
                Parent.Audio.StopSounds();
                SwitchView(new MainMenu.MainMenuView(Parent));
            }
        }

        private void GameOver()
        {
            if (!_gameOverCheck)
            {
                _gameOverCheck = true;
                Parent.Audio.StopSounds();
            }
        }

        private float GetRangeOfTurret(ITurretModule mod)
        {
            if (mod is IRangeAttribute range)
                return range.Range;
            return 1;
        }

        private void SetTurretUpgradeField(TurretInstance turret)
        {
            int pageIndex = 0;
            int offset = 0;
            int count = 0;
            foreach (var upgrade in turret.GetDefinition().Upgrades)
            {
                if (turret.HasUpgrades.Contains(upgrade.ID))
                    continue;
                count++;
                bool isLocked = false;
                if (upgrade.Requires != null)
                    isLocked = !turret.HasUpgrades.Contains((Guid)upgrade.Requires);
                bool canUpgrade = _game.TurretsModule.CanUpgradeTurret(turret, upgrade.ID) == TurretsModule.CanUpgradeResult.Success;
                _upgradePageHandler.Pages[pageIndex][offset].SetUpgrade(
                    upgrade,
                    canUpgrade,
                    isLocked);

                offset++;
                if (offset % _upgradePageHandler.ItemsPrPage == 0)
                {
                    pageIndex++;
                    offset = 0;
                }
            }
            _upgradePageHandler.PageIndex = 0;
            _upgradePageHandler.MinPage = 0;
            _upgradePageHandler.MaxPage = (int)Math.Ceiling((double)count / _upgradePageHandler.ItemsPrPage);
            _upgradePageHandler.MaxItem = count;
            _upgradePageHandler.UpdatePages();
        }
    }
}
