using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Helpers;
using TDGame.Core.Models;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Projectiles;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Models.Maps;
using TDGame.Core.Modules;
using TDGame.Core.Resources;
using static TDGame.Core.Models.Entities.Enemies.EnemyDefinition;
using static TDGame.Core.Models.Entities.Turrets.TurretInstance;

namespace TDGame.Core
{
    public delegate void TurretEventHandler(TurretInstance turret);
    public delegate void EnemyEventHandler(EnemyInstance enemy);
    public partial class Game
    {
        public EnemyEventHandler? OnEnemySpawned;
        public EnemyEventHandler? OnEnemyKilled;

        public TurretEventHandler? OnTurretPurchased;
        public TurretEventHandler? OnTurretSold;
        public TurretEventHandler? OnTurretUpgraded;
        public TurretEventHandler? OnTurretShooting;
        public TurretEventHandler? OnTurretIdle;

        private List<EnemyInstance> _spawnQueue = new List<EnemyInstance>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private GameTimer _mainLoopTimer;
        private Random _rnd = new Random();

        public int Spawned { get; internal set; } = 1;
        private List<Guid> _normalEnemies = new List<Guid>();
        private List<Guid> _bossEnemies = new List<Guid>();

        public AOETurretsModule AOETurretsModule { get; }
        public LaserTurretsModule LaserTurretsModule { get; }
        public ProjectileTurretsModule ProjectileTurretsModule { get; }
        public InvestmentTurretsModule InvestmentTurretsModule { get; }

        public Game(Guid mapID, Guid styleID)
        {
            CurrentEnemies = new List<EnemyInstance>();
            Turrets = new List<TurretInstance>();
            EnemiesToSpawn = new List<Guid>();

            AOETurretsModule = new AOETurretsModule(this);
            LaserTurretsModule = new LaserTurretsModule(this);
            ProjectileTurretsModule = new ProjectileTurretsModule(this);
            InvestmentTurretsModule = new InvestmentTurretsModule(this);

            Map = ResourceManager.Maps.GetResource(mapID);
            GameStyle = ResourceManager.GameStyles.GetResource(styleID);
            HP = GameStyle.StartingHP;
            Money = GameStyle.StartingMoney;
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { if (CurrentEnemies.Count == 0) QueueEnemies(); });
            _evolutionTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { Evolution *= GameStyle.EvolutionRate; });
            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);

            var options = ResourceManager.Enemies.GetResources();
            foreach(var enemy in options)
            {
                if (ResourceManager.Enemies.GetResource(enemy).IsBoss)
                    _bossEnemies.Add(enemy);
                else
                    _normalEnemies.Add(enemy);
            }

            UpdateEnemiesToSpawnList();
        }

        private void EndGame()
        {
            Running = false;
            GameOver = true;
        }

        public void Update(TimeSpan passed)
        {
            if (Running)
            {
                if (AutoSpawn)
                    _enemySpawnTimer.Update(passed);
                _evolutionTimer.Update(passed);
                _mainLoopTimer.Update(passed);
                GameTime += passed;
            }
        }

        private void MainLoop()
        {
            UpdateSpawnQueue();
            UpdateEnemySlowDuration(_mainLoopTimer.Target);
            UpdateEnemyPositions();
            AOETurretsModule.Update(_mainLoopTimer.Target);
            LaserTurretsModule.Update(_mainLoopTimer.Target);
            ProjectileTurretsModule.Update(_mainLoopTimer.Target);
            InvestmentTurretsModule.Update(_mainLoopTimer.Target);
        }

        private void DamagePlayer()
        {
            HP--;
            if (HP <= 0)
                EndGame();
        }

        internal float GetAngle(FloatPoint target, IPosition item) => MathHelpers.GetAngle(target.X, target.Y, item.X + item.Size / 2, item.Y + item.Size / 2);
        internal float GetAngle(IPosition item1, IPosition item2) => MathHelpers.GetAngle(item1.X + item1.Size / 2, item1.Y + item1.Size / 2, item2.X + item2.Size / 2, item2.Y + item2.Size / 2);

        internal EnemyInstance? GetBestEnemy(ProjectileInstance projectile) => GetBestEnemy(projectile, float.MaxValue, TargetingTypes.Closest, projectile.GetDefinition().CanDamage);
        internal EnemyInstance? GetBestEnemy(TurretInstance turret, float range) => GetBestEnemy(turret, range, turret.TargetingType, turret.GetDefinition().CanDamage);
        internal EnemyInstance? GetBestEnemy(IPosition item, float range, TargetingTypes targetingType, List<EnemyTerrrainTypes> canDamage)
        {
            EnemyInstance? best = null;
            switch (targetingType)
            {
                case TargetingTypes.Closest:
                    var minDist = float.MaxValue;
                    foreach (var enemy in CurrentEnemies)
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
                    foreach (var enemy in CurrentEnemies)
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
                    foreach (var enemy in CurrentEnemies)
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
                Money += enemy.GetDefinition().Reward;
                Score += enemy.GetDefinition().Reward;
                CurrentEnemies.Remove(enemy);
                if (OnEnemyKilled != null)
                    OnEnemyKilled.Invoke(enemy);
                return true;
            }
            return false;
        }
    }
}
