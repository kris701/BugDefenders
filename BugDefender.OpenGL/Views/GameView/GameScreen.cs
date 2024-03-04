using BugDefender.Core.Game;
#if RELEASE
using BugDefender.Core.Game.Helpers;
#endif
using BugDefender.Core.Game.Models.Entities;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.Core.Game.Models.Maps;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.ResourcePacks.EntityResources;
using BugDefender.OpenGL.Views.GameView;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static BugDefender.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace BugDefender.OpenGL.Screens.GameScreen
{
    public partial class GameScreen : BaseAnimatedView
    {
        private static readonly Guid _id = new Guid("2222e50b-cfcd-429b-9a21-3a3b77b4d87b");
        private Rectangle _gameArea = new Rectangle(155, 10, 950, 950);
        private static readonly string _saveDir = "Saves";

        private readonly EntityUpdater<TurretInstance, TurretControl> _turretUpdater;
        private readonly EntityUpdater<EnemyInstance, EnemyControl> _enemyUpdater;
        private readonly EntityUpdater<ProjectileInstance, AnimatedTileControl> _projectileUpdater;
        private readonly EntityUpdater<EffectEntity, AnimatedTileControl> _effectsUpdater;
        private readonly EntityUpdater<LaserEntity, LineControl> _laserUpdater;

        private readonly GameEngine _game;
        private Guid? _buyingTurret;
        private TurretInstance? _selectedTurret;
        private readonly HashSet<EffectEntity> _effects = new HashSet<EffectEntity>();
        private readonly Dictionary<Guid, LaserEntity> _lasers = new Dictionary<Guid, LaserEntity>();

        private readonly KeyWatcher _waveKeyWatcher;
        private int tabIndex = 0;
        private readonly KeyWatcher _switchTurretWatcher;
        private readonly KeyWatcher _escapeKeyWatcher;
        private bool _gameOverCheck = false;
        private bool _unselectTurret = false;
        private bool _selectTurret = false;

        public GameScreen(GameWindow parent, ChallengeDefinition challenge) : this(parent, new GameEngine(challenge))
        {
        }

        public GameScreen(GameWindow parent, Guid mapID, Guid gameStyleID) : this(parent, new GameEngine(mapID, gameStyleID))
        {
        }

        public GameScreen(GameWindow parent, GameContext context) : this(parent, new GameEngine(context))
        {
        }

        private GameScreen(GameWindow parent, GameEngine game) : base(
            parent,
            _id,
            parent.UIResources.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.UIResources.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            _game = game;
            _game.TurretsModule.OnTurretShooting += OnTurretFiring;
            _game.TurretsModule.OnTurretIdle += OnTurretIdling;
            _game.OnPlayerDamaged += () =>
            {
                Parent.UIResources.PlaySoundEffectOnce(new Guid("130c17d8-7cab-4fc0-8256-18092609f8d5"));
            };

            _turretUpdater = new EntityUpdater<TurretInstance, TurretControl>(7, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater = new EntityUpdater<EnemyInstance, EnemyControl>(3, this, _gameArea.X, _gameArea.Y);
            _enemyUpdater.OnDelete += OnEnemyDeath;
            _projectileUpdater = new EntityUpdater<ProjectileInstance, AnimatedTileControl>(5, this, _gameArea.X, _gameArea.Y);
            _projectileUpdater.OnDelete += OnProjectileDeleted;
            _effectsUpdater = new EntityUpdater<EffectEntity, AnimatedTileControl>(6, this, _gameArea.X, _gameArea.Y);
            _laserUpdater = new EntityUpdater<LaserEntity, LineControl>(4, this, _gameArea.X, _gameArea.Y);

            _waveKeyWatcher = new KeyWatcher(Keys.Space, () => { _sendWave?.DoClick(); });
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
            Parent.UIResources.PlaySong(ID);

#if DEBUG && DRAWBLOCKINGTILES
            foreach (var blockingTile in _game.Context.Map.BlockingTiles)
            {
                AddControl(99, new TileControl()
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
                    AddControl(99, new LineControl()
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
                var entityDef = Parent.UIResources.GetAnimation<ProjectileEntityDefinition>(projectile.DefinitionID);
                if (entityDef.OnDestroyed != Guid.Empty)
                {
                    var soundDef = Parent.UIResources.GetSoundEffects<ProjectileEntityDefinition>(projectile.DefinitionID);
                    if (soundDef.OnDestroyed != Guid.Empty)
                        Parent.UIResources.PlaySoundEffectOnce(soundDef.OnDestroyed);
                    var effect = Parent.UIResources.GetTextureSet(entityDef.OnDestroyed);
                    _effects.Add(new EffectEntity(entityDef.OnDestroyed, TimeSpan.FromMilliseconds(250), effect)
                    {
                        X = projectile.CenterX - effect.LoadedContents[0].Width / 2,
                        Y = projectile.CenterY - effect.LoadedContents[0].Height / 2
                    });
                }
            }
        }

        private void OnEnemyDeath(EnemyControl parent)
        {
            var entityDef = Parent.UIResources.GetAnimation<EnemyEntityDefinition>(parent.Enemy.DefinitionID);
            if (entityDef.OnDeath != Guid.Empty)
            {
                var effect = Parent.UIResources.GetTextureSet(entityDef.OnDeath);
                _effects.Add(new EffectEntity(entityDef.OnDeath, TimeSpan.FromMilliseconds(250), effect)
                {
                    X = parent.Enemy.CenterX - effect.LoadedContents[0].Width / 2,
                    Y = parent.Enemy.CenterY - effect.LoadedContents[0].Height / 2
                });
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

            _turretSelectRangeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)GetRangeOfTurret(_selectedTurret.TurretInfo) * 2);
            _turretSelectRangeTile.Width = _turretSelectRangeTile.FillColor.Width;
            _turretSelectRangeTile.Height = _turretSelectRangeTile.FillColor.Height;
            _turretSelectRangeTile.X = _selectedTurret.CenterX + _gameArea.X - _turretSelectRangeTile.FillColor.Width / 2;
            _turretSelectRangeTile.Y = _selectedTurret.CenterY + _gameArea.Y - _turretSelectRangeTile.FillColor.Height / 2;
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
            if (_selectedTurret != null && !_unselectTurret && parent.Tag is UpgradeDefinition upg)
            {
                if (_game.TurretsModule.CanUpgradeTurret(_selectedTurret, upg.ID))
                {
                    Parent.UIResources.PlaySoundEffectOnce(new Guid("aebfa031-8a3c-46c1-82dd-13a39d3caf36"));
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
            _scoreLabel.Text = $"Wave {_game.Context.Wave}, Score {_game.Context.Score}, HP: {_game.Context.HP}";

            UpdateTurretPurchaseButtons();
            UpdateNextEnemies();

            _turretUpdater.UpdateEntities(_game.Context.Turrets, gameTime, CreateNewTurretControl);
            _enemyUpdater.UpdateEntities(_game.Context.CurrentEnemies, gameTime, CreateNewEnemyControl, UpdateEnemyControl);
            _projectileUpdater.UpdateEntities(_game.Context.Projectiles, gameTime, CreateNewProjectileControl);
            _effectsUpdater.UpdateEntities(_effects, gameTime, CreateNewEffect);
            _laserUpdater.UpdateEntities(_lasers.Values.ToHashSet(), gameTime, CreateNewLaser, UpdateLaserControl);
            UpdateEffectLifetimes(gameTime);

            UpdateLasers();

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

            _saveAndExitButton.IsEnabled = _game.Context.CanSave();
            if (_playtimeLabel != null)
                _playtimeLabel.Text = $"Game time: {_game.Context.GameTime.ToString("hh\\:mm\\:ss")}";
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
            if (Parent.UIResources.HasSoundEffects<EffectEntityDefinition>(entity.DefinitionID))
            {
                var entityDef = Parent.UIResources.GetSoundEffects<ProjectileEntityDefinition>(entity.DefinitionID);
                if (entityDef.OnCreate != Guid.Empty)
                    Parent.UIResources.PlaySoundEffectOnce(entityDef.OnCreate);
            }
            var animation = Parent.UIResources.GetAnimation<ProjectileEntityDefinition>(entity.DefinitionID).OnCreate;
            var textureSet = Parent.UIResources.GetTextureSet(animation);
            return new AnimatedTileControl()
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
            if (Parent.UIResources.HasSoundEffects<EffectEntityDefinition>(entity.ID))
            {
                var entityDef = Parent.UIResources.GetSoundEffects<EffectEntityDefinition>(entity.ID);
                if (entityDef.OnCreate != Guid.Empty)
                    Parent.UIResources.PlaySoundEffectOnce(entityDef.OnCreate);
            }
            var newTile = new AnimatedTileControl()
            {
                X = _gameArea.X + entity.X,
                Y = _gameArea.Y + entity.Y,
                FrameTime = TimeSpan.FromMilliseconds(entity.TextureSetDefinition.FrameTime),
                TileSet = entity.TextureSetDefinition.LoadedContents,
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
                var entityDef = Parent.UIResources.GetAnimation<EffectEntityDefinition>(turret.DefinitionID);
                if (entityDef.OnCreate != Guid.Empty)
                {
                    var effect = Parent.UIResources.GetTextureSet(entityDef.OnCreate);
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
            control.SetTurretAnimation(Parent.UIResources.GetAnimation<TurretEntityDefinition>(turret.DefinitionID).OnIdle);
            control.Initialize();
            Parent.UIResources.StopSoundEffect(control.CurrentSoundEffect);
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

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    var turretDef = ResourceManager.Turrets.GetResource((Guid)_buyingTurret);
                    var at = new FloatPoint(
                        relativeMousePosition.X - turretDef.Size / 2 - _gameArea.X,
                        relativeMousePosition.Y - turretDef.Size / 2 - _gameArea.Y);
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
                _buyingPreviewRangeTile.FillColor = BasicTextures.GetBasicCircle(new Color(50, 50, 50), (int)GetRangeOfTurret(def.ModuleInfo) * 2);
                _buyingPreviewRangeTile.Width = _buyingPreviewRangeTile.FillColor.Width;
                _buyingPreviewRangeTile.Height = _buyingPreviewRangeTile.FillColor.Height;
                _turretStatesTextbox.Text = new TurretInstance(def).GetDescriptionString();
            }
        }

        private void SaveGame()
        {
            if (_game.Context.CanSave())
            {
                if (!Directory.Exists(_saveDir))
                    Directory.CreateDirectory(_saveDir);
                var saveFile = Path.Combine(_saveDir, $"{Parent.CurrentUser.ID}_save.json");
                _game.Context.Save(new FileInfo(saveFile));
            }
        }

        private void SaveAndGoToMainMenu()
        {
            if (_game.Context.CanSave())
            {
                SaveGame();
                GoToMainMenu();
            }
        }

        private void GoToMainMenu()
        {
            Parent.UIResources.StopSounds();
            SwitchView(new MainMenu.MainMenuView(Parent));
        }

        private void GameOver()
        {
            if (!_gameOverCheck)
            {
                _gameOverCheck = true;
                var credits = 0;
                var result = _game.Result;

                if (_game.Context.Challenge != null)
                {
                    if (result == GameResult.ChallengeSuccess)
                    {
                        credits += _game.Context.Challenge.Reward;
                        Parent.CurrentUser.CompletedChallenges.Add(_game.Context.Challenge.ID);
                    }
                }

#if RELEASE
                if (CheatsHelper.Cheats.Count == 0)
                {
#endif
                if (result != GameResult.ChallengeLost)
                {
                    Parent.CurrentUser.Stats.Combine(_game.Context.Stats);
                    credits += (_game.Context.Score / 100);
                    Parent.CurrentUser.Credits += credits;
                    Parent.UserManager.CheckAndApplyAchivements(Parent.CurrentUser);
                    Parent.UserManager.SaveUser(Parent.CurrentUser);
                }
#if RELEASE
                }
#endif
                Parent.UIResources.StopSounds();
                var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
                SwitchView(new GameOverScreen.GameOverView(Parent, screen, _game.Context.Score, credits, _game.Context.GameTime, _game.Result));
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
                var upgradePanel = new UpgradePanel(Parent, BuyUpgrade_Click, upgrade, _game.TurretsModule.CanUpgradeTurret(turret, upgrade.ID))
                {
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    X = _upgradesLeftButton.X,
                    Y = _upgradesLeftButton.Y + (offset++ * 170) + 30,
                    Height = 30,
                    Width = 300,
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
