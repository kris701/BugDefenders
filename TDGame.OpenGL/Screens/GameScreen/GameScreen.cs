using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Entities.Turrets.Modules;
using TDGame.Core.Models.Entities.Upgrades;
using TDGame.Core.Models.Maps;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures.Animations;
using static TDGame.Core.Models.Entities.Turrets.TurretInstance;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseScreen
    {
        private Rectangle _gameArea = new Rectangle(10, 10, 650, 650);

        private EntityUpdater<TurretInstance, TurretControl> _turretUpdater;
        private EntityUpdater<EnemyInstance, EnemyControl> _enemyUpdater;
        private EntityUpdater<ProjectileInstance, AnimatedTileControl> _projectileUpdater;
        private EntityUpdater<EffectEntity, AnimatedTileControl> _effectsUpdater;
        private EntityUpdater<LaserEntity, LineControl> _laserUpdater;

        private Guid _currentMap;
        private Guid _currentGameStyle;
        private Core.Game _game;
        private Guid? _buyingTurret;
        private TurretInstance? _selectedTurret;
        private List<EffectEntity> _effects = new List<EffectEntity>();
        private Dictionary<Guid, LaserEntity> _lasers = new Dictionary<Guid, LaserEntity>();

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
            _projectileUpdater = new EntityUpdater<ProjectileInstance, AnimatedTileControl>(5, this, _gameArea.X, _gameArea.Y);
            _projectileUpdater.OnDelete += OnProjectileDeleted;
            _effectsUpdater = new EntityUpdater<EffectEntity, AnimatedTileControl>(6, this, _gameArea.X, _gameArea.Y);
            _laserUpdater = new EntityUpdater<LaserEntity, LineControl>(7, this, _gameArea.X, _gameArea.Y);

            _waveKeyWatcher = new KeyWatcher(Keys.Space, () =>
            {
                _game.QueueEnemies();
                _sendWave.FillColor = BasicTextures.GetBasicRectange(Color.DarkGray);
            }, () =>
            {
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

        private void OnProjectileDeleted(AnimatedTileControl parent)
        {
            if (parent.Tag is ProjectileInstance projectile)
            {
                if (!projectile.GetDefinition().IsExplosive)
                    return;
                _effects.Add(new EffectEntity(projectile)
                {
                    ID = new Guid("ebca3566-e0bf-4aa1-9a29-74be54f3e96b"),
                    LifeTime = TimeSpan.FromMilliseconds(250)
                });
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
            foreach (var button in _turretTargetingModes)
            {
                button.IsEnabled = false;
                button.FillColor = BasicTextures.GetBasicRectange(Color.Gray);
            }
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
            _turretSelectRangeTile.X = turret.CenterX + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile.Y = turret.CenterY + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
            _turretSelectRangeTile.IsVisible = true;

            int index = 0;
            foreach (var upgrade in turret.GetDefinition().Upgrades)
                if (!turret.HasUpgrades.Contains(upgrade.ID))
                    _turretUpgradePanels[index++].SetUpgrade(upgrade, _game.CanLevelUpTurret(turret, upgrade.ID));

            _turretStatesTextbox.Text = turret.GetDescriptionString();
            _sellTurretButton.Text = $"[{_selectedTurret.GetTurretWorth()}$] Sell Turret";
            _sellTurretButton.IsEnabled = true;

            foreach (var button in _turretTargetingModes)
            {
                button.IsEnabled = true;
                if (Enum.GetName(typeof(TargetingTypes), turret.TargetingType) == button.Text)
                    button.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
            }
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

            _turretUpdater.UpdateEntities(_game.Turrets, gameTime, CreateNewTurretControl);
            _enemyUpdater.UpdateEntities(_game.CurrentEnemies, gameTime, CreateNewEnemyControl, UpdateEnemyControl);
            _projectileUpdater.UpdateEntities(_game.ProjectileTurretsModule.Projectiles, gameTime, CreateNewProjectileControl);
            _effectsUpdater.UpdateEntities(_effects, gameTime, CreateNewEffect);
            _laserUpdater.UpdateEntities(_lasers.Values.ToList(), gameTime, CreateNewLaser, UpdateLaserControl);
            UpdateEffectLifetimes(gameTime);

            UpdateLasers();

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

        private void UpdateEffectLifetimes(GameTime gameTime)
        {
            var toRemove = new List<EffectEntity>();
            foreach (var effect in _effects)
            {
                effect.LifeTime -= gameTime.ElapsedGameTime;
                if (effect.LifeTime <= TimeSpan.Zero)
                    toRemove.Add(effect);
            }
            foreach (var remove in toRemove)
                _effects.Remove(remove);
        }

        private TurretControl CreateNewTurretControl(TurretInstance entity)
        {
            var animation = TextureManager.GetAnimation<TurretAnimationDefinition>(entity.DefinitionID).OnIdle;
            var textureSet = TextureManager.GetTextureSet(animation);
            return new TurretControl(this, clicked: Turret_Click)
            {
                IsEnabled = true,
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Transparent),
                FillDisabledColor = BasicTextures.GetBasicRectange(Color.Transparent),
                TileSet = textureSet.LoadedContents,
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                X = _gameArea.X + entity.X,
                Y = _gameArea.Y + entity.Y,
                Width = entity.Size,
                Height = entity.Size,
                Rotation = entity.Angle,
                Tag = entity
            };
        }

        private EnemyControl CreateNewEnemyControl(EnemyInstance entity)
        {
            var animation = TextureManager.GetAnimation<EnemyAnimationDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = TextureManager.GetTextureSet(animation);
            return new EnemyControl(this, entity)
            {
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                TileSet = textureSet.LoadedContents,
                AutoPlay = true,
                X = entity.X + _gameArea.X,
                Y = entity.Y + _gameArea.Y,
                Width = entity.Size,
                Height = entity.Size,
                Rotation = entity.Angle + (float)Math.PI / 2,
                Tag = entity
            };
        }

        private void UpdateEnemyControl(EnemyInstance entity, EnemyControl control, GameTime passed)
        {
            control.X = entity.X + _gameArea.X + control.VisualOffset.X;
            control.Y = entity.Y + _gameArea.Y + control.VisualOffset.Y;
            control.Rotation = entity.Angle + (float)Math.PI / 2;
        }

        private AnimatedTileControl CreateNewProjectileControl(ProjectileInstance entity)
        {
            var animation = TextureManager.GetAnimation<ProjectileAnimationDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = TextureManager.GetTextureSet(animation);
            return new AnimatedTileControl(this)
            {
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                TileSet = textureSet.LoadedContents,
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
            var animation = TextureManager.GetAnimation<EffectAnimationDefinition>(entity.ID).OnCreate;
            var textureSet = TextureManager.GetTextureSet(animation);
            var newTile = new AnimatedTileControl(this)
            {
                X = _gameArea.X + entity.X,
                Y = _gameArea.Y + entity.Y,
                FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime),
                TileSet = textureSet.LoadedContents,
                AutoPlay = true,
                Width = entity.Size,
                Height = entity.Size,
                Tag = entity
            };
            return newTile;
        }

        private LineControl CreateNewLaser(LaserEntity entity)
        {
            var newTile = new LineControl(this)
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

        private void UpdateLaserControl(LaserEntity entity, LineControl control, GameTime passed)
        {
            control.X = _gameArea.X + entity.From.CenterX;
            control.Y = _gameArea.Y + entity.From.CenterY;
            control.X2 = _gameArea.X + entity.To.CenterX;
            control.Y2 = _gameArea.Y + entity.To.CenterY;
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
            control.SetTurretAnimation(TextureManager.GetAnimation<TurretAnimationDefinition>(turret.DefinitionID).OnShoot);
            control.Initialize();
        }

        public void OnTurretIdling(TurretInstance turret)
        {
            var control = _turretUpdater.GetItem(turret);
            if (control == null)
                return;
            control.SetTurretAnimation(TextureManager.GetAnimation<TurretAnimationDefinition>(turret.DefinitionID).OnIdle);
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
            var found = new List<Guid>();
            foreach (var turret in _game.Turrets)
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
                        _lasers.Add(turret.ID, new LaserEntity()
                        {
                            From = turret,
                            To = turret.Targeting
                        });
                }
            }
            var toRemove = new List<Guid>();
            foreach (var key in _lasers.Keys)
                if (!found.Contains(key))
                    toRemove.Add(key);
            foreach (var remove in toRemove)
                _lasers.Remove(remove);
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
                var animation = TextureManager.GetAnimation<TurretAnimationDefinition>(turretID).OnIdle;
                var textureSet = TextureManager.GetTextureSet(animation);
                _buyingPreviewTile.TileSet = textureSet.LoadedContents;
                _buyingPreviewTile.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
                _buyingPreviewTile.Width = turret.Size;
                _buyingPreviewTile.Height = turret.Size;
                _buyingPreviewTile.Initialize();
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(Color.Gray, (int)(GetRangeOfTurret(turret) * 2));
                _buyingPreviewRangeTile.Width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile.Height = _buyingPreviewRangeTile.FillColor.Height;
                _turretStatesTextbox.Text = new TurretInstance(turret).GetDescriptionString();
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
            switch (turret.ModuleInfo)
            {
                case AOETurretDefinition def: return def.Range;
                case LaserTurretDefinition def: return def.Range;
                case ProjectileTurretDefinition def: return def.Range;
            }
            return 1;
        }
    }
}
