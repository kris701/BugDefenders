using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Projectiles;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Maps;
using TDGame.Core.Game.Modules;
using TDGame.Core.Game.Modules.Enemies;
using TDGame.Core.Game.Modules.Projectiles;
using TDGame.Core.Game.Modules.Turrets;
using TDGame.Core.Resources;
using static TDGame.Core.Game.Models.Entities.Enemies.EnemyDefinition;
using static TDGame.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace TDGame.Core.Game
{
    public delegate void TurretEventHandler(TurretInstance turret);
    public delegate void EnemyEventHandler(EnemyInstance enemy);
    public partial class GameEngine
    {
        public EnemyEventHandler? OnEnemySpawned;
        public EnemyEventHandler? OnEnemyKilled;

        public TurretEventHandler? OnTurretPurchased;
        public TurretEventHandler? OnTurretSold;
        public TurretEventHandler? OnBeforeTurretUpgraded;
        public TurretEventHandler? OnTurretUpgraded;
        public TurretEventHandler? OnTurretShooting;
        public TurretEventHandler? OnTurretIdle;

        private List<EnemyInstance> _spawnQueue = new List<EnemyInstance>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private GameTimer _mainLoopTimer;
        private int _waveQueue = 0;

        public bool GameOver { get; set; }
        private bool _running = true;
        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                if (!GameOver)
                    _running = value;
            }
        }
        public GameContext Context { get; }
        public List<IGameModule> GameModules { get; }

        public AOETurretsModule AOETurretsModule { get; }
        public LaserTurretsModule LaserTurretsModule { get; }
        public ProjectileTurretsModule ProjectileTurretsModule { get; }
        public InvestmentTurretsModule InvestmentTurretsModule { get; }
        public PassiveTurretsModule PassiveTurretsModule { get; }

        public ExplosiveProjectileModule ExplosiveProjectileModule { get; }
        public DirectProjectileModule DirectProjectileModule { get; }

        public WaveEnemyModule WaveEnemiesModule { get; }
        public SingleEnemyModule SingleEnemiesModule { get; }

        public GameEngine(Guid mapID, Guid styleID)
        {
            Context = new GameContext(
                ResourceManager.Maps.GetResource(mapID),
                ResourceManager.GameStyles.GetResource(styleID));
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { if (Context.CurrentEnemies.Count == 0) QueueEnemies(); });
            _evolutionTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { Context.Evolution *= Context.GameStyle.EvolutionRate; });
            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);

            AOETurretsModule = new AOETurretsModule(Context, this);
            LaserTurretsModule = new LaserTurretsModule(Context, this);
            ProjectileTurretsModule = new ProjectileTurretsModule(Context, this);
            InvestmentTurretsModule = new InvestmentTurretsModule(Context, this);
            PassiveTurretsModule = new PassiveTurretsModule(Context, this);

            ExplosiveProjectileModule = new ExplosiveProjectileModule(Context, this);
            DirectProjectileModule = new DirectProjectileModule(Context, this);

            WaveEnemiesModule = new WaveEnemyModule(Context, this);
            SingleEnemiesModule = new SingleEnemyModule(Context, this);

            GameModules = new List<IGameModule>()
            {
                AOETurretsModule,
                LaserTurretsModule,
                ProjectileTurretsModule,
                InvestmentTurretsModule,
                PassiveTurretsModule,

                ExplosiveProjectileModule,
                DirectProjectileModule,

                WaveEnemiesModule,
                SingleEnemiesModule
            };

            UpdateEnemiesToSpawnList();
        }

        public void Update(TimeSpan passed)
        {
            if (Running)
            {
                if (Context.AutoSpawn)
                    _enemySpawnTimer.Update(passed);
                _evolutionTimer.Update(passed);
                _mainLoopTimer.Update(passed);
                Context.GameTime += passed;
            }
        }

        private void MainLoop()
        {
            UpdateSpawnQueue(_mainLoopTimer.Target);
            foreach (var module in GameModules)
                module.Update(_mainLoopTimer.Target);
        }

        internal float GetAngle(FloatPoint target, IPosition item) => MathHelpers.GetAngle(target.X, target.Y, item.CenterX, item.CenterY);
        internal float GetAngle(IPosition item1, IPosition item2) => MathHelpers.GetAngle(item1.CenterX, item1.CenterY, item2.CenterX, item2.CenterY);

        #region Enemies

        internal void DamagePlayer()
        {
            Context.HP--;
            if (Context.HP <= 0)
            {
                Context.HP = 0;
                Running = false;
                GameOver = true;
            }
        }

        internal EnemyInstance? GetBestEnemy(ProjectileInstance projectile) => GetBestEnemy(projectile, float.MaxValue, TargetingTypes.Closest, projectile.GetDefinition().CanDamage);
        internal EnemyInstance? GetBestEnemy(TurretInstance turret, float range) => GetBestEnemy(turret, range, turret.TargetingType, turret.GetDefinition().CanDamage);
        internal EnemyInstance? GetBestEnemy(IPosition item, float range, TargetingTypes targetingType, List<EnemyTerrrainTypes> canDamage)
        {
            EnemyInstance? best = null;
            switch (targetingType)
            {
                case TargetingTypes.Closest:
                    var minDist = float.MaxValue;
                    foreach (var enemy in Context.CurrentEnemies)
                    {
                        if (!canDamage.Contains(enemy.GetDefinition().TerrainType))
                            continue;
                        var dist = MathHelpers.Distance(item.CenterX, item.CenterY, enemy.CenterX, enemy.CenterY);
                        if (dist <= range && dist < minDist)
                        {
                            minDist = dist;
                            best = enemy;
                        }
                    }
                    break;
                case TargetingTypes.Weakest:
                    var lowestHP = float.MaxValue;
                    foreach (var enemy in Context.CurrentEnemies)
                    {
                        if (!canDamage.Contains(enemy.GetDefinition().TerrainType))
                            continue;
                        var dist = MathHelpers.Distance(item.CenterX, item.CenterY, enemy.CenterX, enemy.CenterY);
                        if (dist <= range && enemy.Health < lowestHP)
                        {
                            lowestHP = enemy.Health;
                            best = enemy;
                        }
                    }
                    break;
                case TargetingTypes.Strongest:
                    var highestHP = 0f;
                    foreach (var enemy in Context.CurrentEnemies)
                    {
                        if (!canDamage.Contains(enemy.GetDefinition().TerrainType))
                            continue;
                        var dist = MathHelpers.Distance(item.CenterX, item.CenterY, enemy.CenterX, enemy.CenterY);
                        if (dist <= range && enemy.Health > highestHP)
                        {
                            highestHP = enemy.Health;
                            best = enemy;
                        }
                    }
                    break;

            }
            return best;
        }

        internal bool DamageEnemy(EnemyInstance enemy, float damage)
        {
            enemy.Health -= damage;
            if (enemy.Health <= 0)
            {
                Context.Money += (int)(enemy.GetDefinition().Reward * Context.GameStyle.MoneyMultiplier);
                Context.Score += enemy.GetDefinition().Reward;
                Context.CurrentEnemies.Remove(enemy);
                if (OnEnemyKilled != null)
                    OnEnemyKilled.Invoke(enemy);

                Context.Outcome.TotalKills++;
                if (!Context.Outcome.KillsOfType.ContainsKey(enemy.DefinitionID))
                    Context.Outcome.KillsOfType.Add(enemy.DefinitionID, 1);
                else
                    Context.Outcome.KillsOfType[enemy.DefinitionID] += 1;

                return true;
            }
            return false;
        }

        private void UpdateEnemiesToSpawnList()
        {
            int waveSize = (int)(1 * Context.Evolution);
            for (int i = Context.EnemiesToSpawn.Count; i < 3; i++)
            {
                var wave = new List<Guid>();
                for (int j = 0; j < waveSize; j++)
                {
                    Guid? newEnemy = null;

                    if (_waveQueue != 0 && _waveQueue % Context.GameStyle.BossEveryNWave == 0 && SingleEnemiesModule.EnemyOptions.Count > 0)
                        newEnemy = SingleEnemiesModule.GetRandomEnemy(_waveQueue);
                    if (WaveEnemiesModule.EnemyOptions.Count > 0 && newEnemy == null)
                        newEnemy = WaveEnemiesModule.GetRandomEnemy(_waveQueue);

                    if (newEnemy != null)
                        wave.Add((Guid)newEnemy);
                }
                _waveQueue++;
                Context.EnemiesToSpawn.Add(wave);
            }
        }

        public void QueueEnemies()
        {
            Context.Money += Context.GameStyle.MoneyPrWave;
            Context.Wave++;
            foreach (var item in Context.EnemiesToSpawn[0])
            {
                if (WaveEnemiesModule.EnemyOptions.Contains(item))
                    _spawnQueue.AddRange(WaveEnemiesModule.QueueEnemies(item));
                else if (SingleEnemiesModule.EnemyOptions.Contains(item))
                    _spawnQueue.AddRange(SingleEnemiesModule.QueueEnemies(item));
            }
            Context.EnemiesToSpawn.RemoveAt(0);
            UpdateEnemiesToSpawnList();
        }

        private void UpdateSpawnQueue(TimeSpan passed)
        {
            var newToAdd = WaveEnemiesModule.UpdateSpawnQueue(passed, _spawnQueue);
            newToAdd.AddRange(SingleEnemiesModule.UpdateSpawnQueue(passed, _spawnQueue));
            foreach (var enemy in newToAdd)
            {
                Context.CurrentEnemies.Add(enemy);
                if (OnEnemySpawned != null)
                    OnEnemySpawned.Invoke(enemy);
            }
            foreach (var remove in newToAdd)
                _spawnQueue.Remove(remove);
        }

        #endregion

        #region Turrets

        public bool CanLevelUpTurret(TurretInstance turret, Guid id)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            var upgrade = turret.GetDefinition().Upgrades.FirstOrDefault(x => x.ID == id);
            if (upgrade == null)
                return false;
            if (Context.Money < upgrade.Cost)
                return false;
            if (upgrade.Requires != null && !turret.HasUpgrades.Contains((Guid)upgrade.Requires))
                return false;
            return true;
        }

        public bool LevelUpTurret(TurretInstance turret, Guid id)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            if (!CanLevelUpTurret(turret, id))
                return false;
            var upgrade = turret.GetDefinition().Upgrades.First(x => x.ID == id);
            if (upgrade == null)
                return false;
            if (OnBeforeTurretUpgraded != null)
                OnBeforeTurretUpgraded.Invoke(turret);

            upgrade.ApplyUpgrade(turret);
            Context.Money -= upgrade.Cost;

            if (OnTurretUpgraded != null)
                OnTurretUpgraded.Invoke(turret);
            return true;
        }

        public void SellTurret(TurretInstance turret)
        {
            if (!Context.Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            Context.Money += turret.GetTurretWorth();
            Context.Turrets.Remove(turret);

            if (OnTurretSold != null)
                OnTurretSold.Invoke(turret);
        }

        public bool AddTurret(TurretDefinition turretDef, FloatPoint at)
        {
            if (Context.Money < turretDef.Cost)
                return false;
            if (Context.Wave < turretDef.AvailableAtWave)
                return false;
            if (Context.GameStyle.TurretBlackList.Contains(turretDef.ID))
                return false;
            if (at.X < 0)
                return false;
            if (at.X > Context.Map.Width - turretDef.Size)
                return false;
            if (at.Y < 0)
                return false;
            if (at.Y > Context.Map.Height - turretDef.Size)
                return false;

            foreach (var block in Context.Map.BlockingTiles)
                if (MathHelpers.Intersects(turretDef, at, block))
                    return false;

            foreach (var otherTurret in Context.Turrets)
                if (MathHelpers.Intersects(turretDef, at, otherTurret))
                    return false;

            var newInstance = new TurretInstance(turretDef);
            newInstance.X = at.X;
            newInstance.Y = at.Y;
            Context.Money -= turretDef.Cost;
            Context.Turrets.Add(newInstance);
            if (OnTurretPurchased != null)
                OnTurretPurchased.Invoke(newInstance);

            Context.Outcome.TotalTurretsPlaced++;
            if (!Context.Outcome.TotalTurretsPlacedOfType.ContainsKey(newInstance.DefinitionID))
                Context.Outcome.TotalTurretsPlacedOfType.Add(newInstance.DefinitionID, 1);
            else
                Context.Outcome.TotalTurretsPlacedOfType[newInstance.DefinitionID] += 1;

            return true;
        }

        #endregion
    }
}
