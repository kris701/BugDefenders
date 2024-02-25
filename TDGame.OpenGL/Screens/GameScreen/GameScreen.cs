using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Game;
using TDGame.Core.Game.Models.Entities;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;
using TDGame.Core.Game.Models.Entities.Upgrades;
using TDGame.Core.Game.Models.Maps;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures.Animations;
using static TDGame.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseScreen
    {
        private static Guid _id = new Guid("2222e50b-cfcd-429b-9a21-3a3b77b4d87b");
        private Rectangle _gameArea = new Rectangle(10, 10, 650, 650);

        private EntityUpdater<TurretInstance, TurretControl> _turretUpdater;
        private EntityUpdater<EnemyInstance, EnemyControl> _enemyUpdater;
        private EntityUpdater<ProjectileInstance, AnimatedTileControl> _projectileUpdater;
        private EntityUpdater<EffectEntity, AnimatedTileControl> _effectsUpdater;
        private EntityUpdater<LaserEntity, LineControl> _laserUpdater;

        private Guid _currentMap;
        private Guid _currentGameStyle;
        private GameEngine _game;
        private Guid? _buyingTurret;
        private TurretInstance? _selectedTurret;
        private HashSet<EffectEntity> _effects = new HashSet<EffectEntity>();
        private Dictionary<Guid, LaserEntity> _lasers = new Dictionary<Guid, LaserEntity>();

        private KeyWatcher _waveKeyWatcher;
        private int tabIndex = 0;
        private KeyWatcher _switchTurretWatcher;
        private KeyWatcher _escapeKeyWatcher;
        private bool _gameOver = false;
        private bool _unselectTurret = false;
        private bool _selectTurret = false;

        public GameScreen(UIEngine parent, Guid mapID, Guid gameStyleID) : base(parent, _id)
        {
            _currentGameStyle = gameStyleID;
            _currentMap = mapID;
            ScaleValue = parent.CurrentUser.UserData.Scale;
            _game = new GameEngine(mapID, gameStyleID);
            _game.TurretsModule.OnTurretShooting += OnTurretFiring;
            _game.TurretsModule.OnTurretIdle += OnTurretIdling;

            _turretUpdater = new EntityUpdater<TurretInstance, TurretControl>(7, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater = new EntityUpdater<EnemyInstance, EnemyControl>(3, this, _gameArea.X, _gameArea.Y);
            _projectileUpdater = new EntityUpdater<ProjectileInstance, AnimatedTileControl>(5, this, _gameArea.X, _gameArea.Y);
            _projectileUpdater.OnDelete += OnProjectileDeleted;
            _effectsUpdater = new EntityUpdater<EffectEntity, AnimatedTileControl>(6, this, _gameArea.X, _gameArea.Y);
            _laserUpdater = new EntityUpdater<LaserEntity, LineControl>(4, this, _gameArea.X, _gameArea.Y);

            _waveKeyWatcher = new KeyWatcher(Keys.Space, () => { _sendWave.DoClick(); });
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
            Initialize();

#if DRAWBLOCKINGTILES
            foreach (var blockingTile in _game.Context.Map.BlockingTiles)
            {
                AddControl(99, new TileControl(Parent)
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
#if DRAWMAPPATHS
            foreach (var path in _game.Context.Map.Paths)
            {
                var from = path[0];
                foreach (var waypoint in path.Skip(1))
                {
                    AddControl(99, new LineControl(Parent)
                    {
                        X = from.X + _gameArea.X,
                        Y = from.Y + _gameArea.Y,
                        X2 = waypoint.X + _gameArea.X,
                        Y2 = waypoint.Y + _gameArea.Y,
                        Alpha = 50
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
                    Parent.UIResources.StopSoundEffect(control.CurrentSoundEffect);
                _game.TurretsModule.SellTurret(_selectedTurret);
                _selectedTurret = null;
                _unselectTurret = true;
            }
        }

        private void OnProjectileDeleted(AnimatedTileControl parent)
        {
            if (parent.Tag is ProjectileInstance projectile)
            {
                if (projectile.ProjectileInfo is ExplosiveProjectileDefinition def)
                {
                    _effects.Add(new EffectEntity()
                    {
                        ID = new Guid("ebca3566-e0bf-4aa1-9a29-74be54f3e96b"),
                        LifeTime = TimeSpan.FromMilliseconds(250),
                        Size = def.SplashRange,
                        X = projectile.CenterX - def.SplashRange / 2,
                        Y = projectile.CenterY - def.SplashRange / 2,
                        Angle = parent.Rotation
                    });
                }
            }
        }

        private void UnselectTurret()
        {
            _turretSelectRangeTile.IsVisible = false;
            _turretStatesTextbox.Text = "Select a Turret";
            _sellTurretButton.IsEnabled = false;
            _sellTurretButton.Text = $"Sell Turret";
            foreach (var button in _turretTargetingModes)
            {
                button.IsEnabled = false;
                button.FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            }
            foreach (var buttons in _turretUpgradePages)
                foreach (var control in buttons)
                    RemoveControl(10, control);
            _upgradesLeftButton.IsVisible = false;
            _upgradesRightButton.IsVisible = false;
        }

        private void Turret_Click(AnimatedButtonControl parent)
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

            _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)Scale(GetRangeOfTurret(_selectedTurret.TurretInfo) * 2));
            _turretSelectRangeTile._width = _turretSelectRangeTile.FillColor.Width;
            _turretSelectRangeTile._height = _turretSelectRangeTile.FillColor.Height;
            _turretSelectRangeTile._x = Scale(_selectedTurret.CenterX) + Scale(_gameArea.X) - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile._y = Scale(_selectedTurret.CenterY) + Scale(_gameArea.Y) - _turretSelectRangeTile.FillColor.Height / 2;
            _turretSelectRangeTile.CalculateViewPort();
            _turretSelectRangeTile.IsVisible = true;

            SetTurretUpgradeField(_selectedTurret);

            _turretStatesTextbox.Text = _selectedTurret.GetDescriptionString();
            _sellTurretButton.Text = $"[{_selectedTurret.GetTurretWorth()}$] Sell Turret";
            _sellTurretButton.IsEnabled = true;

            foreach (var button in _turretTargetingModes)
            {
                button.IsEnabled = true;
                if (Enum.GetName(typeof(TargetingTypes), _selectedTurret.TargetingType) == button.Text)
                    button.FillColor = Parent.UIResources.GetTexture(new Guid("5b3e5e64-9c3d-4ba5-a113-b6a41a501c20"));
            }
        }

        private void BuyUpgrade_Click(ButtonControl parent)
        {
            if (_selectedTurret != null && !_unselectTurret && parent.Tag is IUpgrade upg)
            {
                if (_game.TurretsModule.CanLevelUpTurret(_selectedTurret, upg.ID))
                {
                    _game.TurretsModule.LevelUpTurret(_selectedTurret, upg.ID);
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
                Parent.UIResources.ResumeSounds();
                _startButton.Text = "Pause";
            }
            else
            {
                Parent.UIResources.PauseSounds();
                _startButton.Text = "Start";
            }
        }

        private void AutoRunButton_Click(ButtonControl parent)
        {
            _game.Context.AutoSpawn = !_game.Context.AutoSpawn;
            if (_game.Context.AutoSpawn)
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

            _game.Update(gameTime.ElapsedGameTime);

            _moneyLabel.Text = $"Money: {_game.Context.Money}$";
            _hpLabel.Text = $"HP: {_game.Context.HP}";
            _scoreLabel.Text = $"Wave {_game.Context.Wave},Score {_game.Context.Score}";

            UpdateTurretPurchaseButtons();
            UpdateNextEnemies();

            _turretUpdater.UpdateEntities(_game.Context.Turrets, gameTime, CreateNewTurretControl);
            _enemyUpdater.UpdateEntities(_game.Context.CurrentEnemies, gameTime, CreateNewEnemyControl, UpdateEnemyControl);
            _projectileUpdater.UpdateEntities(_game.Context.Projectiles, gameTime, CreateNewProjectileControl);
            _effectsUpdater.UpdateEntities(_effects, gameTime, CreateNewEffect);
            _laserUpdater.UpdateEntities(_lasers.Values.ToHashSet(), gameTime, CreateNewLaser, UpdateLaserControl);
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
            var animation = Parent.UIResources.GetAnimation<TurretEntityDefinition>(entity.DefinitionID).OnIdle;
            var textureSet = Parent.UIResources.GetTextureSet(animation);
            return new TurretControl(Parent, entity, clicked: Turret_Click)
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
                Rotation = entity.Angle + (float)Math.PI / 2,
                Tag = entity
            };
        }

        private EnemyControl CreateNewEnemyControl(EnemyInstance entity)
        {
            var animation = Parent.UIResources.GetAnimation<EnemyEntityDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = Parent.UIResources.GetTextureSet(animation);
            return new EnemyControl(Parent, entity)
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
            var animation = Parent.UIResources.GetAnimation<ProjectileEntityDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = Parent.UIResources.GetTextureSet(animation);
            return new AnimatedTileControl(Parent)
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
            var animation = Parent.UIResources.GetAnimation<EffectEntityDefinition>(entity.ID).OnCreate;
            var textureSet = Parent.UIResources.GetTextureSet(animation);
            var newTile = new AnimatedTileControl(Parent)
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
            newTile.ViewPort = _gameArea;
            return newTile;
        }

        private LineControl CreateNewLaser(LaserEntity entity)
        {
            var newTile = new LineControl(Parent)
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
            foreach (var turret in _turretPages[_currentTurretPage])
            {
                if (turret.Tag is TurretDefinition def)
                {
                    if (_game.Context.Money < def.Cost || _game.Context.Wave < def.AvailableAtWave)
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
            control.SetTurretAnimation(Parent.UIResources.GetAnimation<TurretEntityDefinition>(turret.DefinitionID).OnShoot);
            control.Initialize();
            Parent.UIResources.StopSoundEffect(control.CurrentSoundEffect);
            control.CurrentSoundEffect = Parent.UIResources.PlaySoundEffect(Parent.UIResources.GetSoundEffects<TurretEntityDefinition>(turret.DefinitionID).OnShoot);
            if (turret.TurretInfo is AOETurretDefinition def)
            {
                _effects.Add(new EffectEntity()
                {
                    ID = turret.DefinitionID,
                    X = turret.CenterX - def.Range,
                    Y = turret.CenterY - def.Range,
                    Size = def.Range * 2
                });
            }
        }

        public void OnTurretIdling(TurretInstance turret)
        {
            var control = _turretUpdater.GetItem(turret);
            if (control == null)
                return;
            control.SetTurretAnimation(Parent.UIResources.GetAnimation<TurretEntityDefinition>(turret.DefinitionID).OnIdle);
            control.Initialize();
            Parent.UIResources.StopSoundEffect(control.CurrentSoundEffect);
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
                _buyingPreviewRangeTile.CalculateViewPort();

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    var turretDef = ResourceManager.Turrets.GetResource((Guid)_buyingTurret);
                    var at = new FloatPoint(
                        relativeMousePosition.X - turretDef.Size / 2,
                        relativeMousePosition.Y - turretDef.Size / 2);
                    if (_game.TurretsModule.AddTurret(turretDef, at))
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
                _buyingTurret = def.ID;
                var animation = Parent.UIResources.GetAnimation<TurretEntityDefinition>(def.ID).OnIdle;
                var textureSet = Parent.UIResources.GetTextureSet(animation);
                _buyingPreviewTile.TileSet = textureSet.LoadedContents;
                _buyingPreviewTile.FrameTime = TimeSpan.FromMilliseconds(textureSet.FrameTime);
                _buyingPreviewTile.Width = def.Size;
                _buyingPreviewTile.Height = def.Size;
                _buyingPreviewTile.Initialize();
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)Scale(GetRangeOfTurret(def.ModuleInfo) * 2));
                _buyingPreviewRangeTile._width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile._height = _buyingPreviewRangeTile.FillColor.Height;
                _turretStatesTextbox.Text = new TurretInstance(def).GetDescriptionString();
            }
        }

        private void GameOver()
        {
            if (!_gameOver)
            {
                _gameOver = true;

                Parent.CurrentUser.Stats.Combine(_game.Context.Outcome);
                var achivements = ResourceManager.Achivements.GetResources();
                foreach (var id in achivements)
                {
                    if (!Parent.CurrentUser.Achivements.Contains(id))
                    {
                        var achivement = ResourceManager.Achivements.GetResource(id);
                        if (achivement.Criteria.IsValid(Parent.CurrentUser.Stats))
                            Parent.CurrentUser.Achivements.Add(id);
                    }
                }

                Parent.UIResources.StopSounds();
                var screen = Parent.TakeScreenCap();
                SwitchView(new GameOverScreen.GameOverScreen(Parent, screen, _game.Context.Score, _game.Context.GameTime));
            }
        }

        private float GetRangeOfTurret(ITurretModule mod)
        {
            if (mod is IRangeAttribute range)
                return range.Range;
            return 1;
        }

        private void UpdateTurretSelectionPages()
        {
            foreach (var buttons in _turretPages)
                foreach (var control in buttons)
                    control.IsVisible = false;

            foreach (var control in _turretPages[_currentTurretPage])
                control.IsVisible = true;
        }

        private void SetTurretUpgradeField(TurretInstance turret)
        {
            _upgradesLeftButton.IsVisible = false;
            _upgradesRightButton.IsVisible = false;
            int count = 1;
            int page = 0;
            int offset = 0;
            _turretUpgradePages.Clear();
            _currentTurretUpgradePage = 0;
            _turretUpgradePages.Add(new List<UpgradePanel>());
            foreach (var upgrade in turret.GetDefinition().Upgrades)
            {
                if (turret.HasUpgrades.Contains(upgrade.ID))
                    continue;
                if (count++ % (_upgradeSelectionsPrPage + 1) == 0)
                {
                    page++;
                    _turretUpgradePages.Add(new List<UpgradePanel>());
                    offset = 0;
                    _upgradesLeftButton.IsVisible = true;
                    _upgradesRightButton.IsVisible = true;
                }
                var upgradePanel = new UpgradePanel(Parent, BuyUpgrade_Click, upgrade, _game.TurretsModule.CanLevelUpTurret(turret, upgrade.ID))
                {
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    X = _gameArea.X + 10 + (offset++ * 210),
                    Y = _gameArea.Y + _gameArea.Height + 45,
                    Height = 30,
                    Width = 210,
                    Tag = upgrade,
                    IsVisible = false
                };
                _turretUpgradePages[page].Add(upgradePanel);
                upgradePanel.Initialize();
                AddControl(10, upgradePanel);
            }

            UpdateTurretUpgradeSelectionPages();
        }

        private void UpdateTurretUpgradeSelectionPages()
        {
            foreach (var buttons in _turretUpgradePages)
                foreach (var control in buttons)
                    control.IsVisible = false;

            foreach (var control in _turretUpgradePages[_currentTurretUpgradePage])
                control.IsVisible = true;
        }
    }
}
