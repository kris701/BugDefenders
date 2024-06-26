﻿using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Modules.Enemies.SubModules;
using static BugDefender.Core.Game.Models.Entities.Enemies.EnemyDefinition;
using static BugDefender.Core.Game.Models.Entities.Turrets.TurretInstance;

namespace BugDefender.Core.Game.Modules.Enemies
{
    public delegate void EnemyEventHandler(EnemyInstance enemy);
    public class EnemiesModule : BaseSuperGameModule
    {
        public EnemyEventHandler? OnEnemySpawned;
        public EnemyEventHandler? OnEnemyKilled;

        public WaveEnemyModule WaveEnemiesModule { get; private set; }
        public SingleEnemyModule SingleEnemiesModule { get; private set; }

        private readonly GameTimer _enemySpawnTimer;
        private readonly HashSet<EnemyInstance> _spawnQueue = new HashSet<EnemyInstance>();
        private int _waveQueue = 0;

        public EnemiesModule(GameContext context, GameEngine game) : base(context, game)
        {
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), (t) => { if (Context.CurrentEnemies.Count == 0) QueueEnemies(); });
            WaveEnemiesModule = new WaveEnemyModule(Context, Game);
            SingleEnemiesModule = new SingleEnemyModule(Context, Game);
            Modules = new List<IGameModule>()
            {
                WaveEnemiesModule,
                SingleEnemiesModule
            };
            UpdateEnemiesToSpawnList();
        }

        public override void Update(TimeSpan passed)
        {
            if (Context.AutoSpawn)
                _enemySpawnTimer.Update(passed);
            UpdateSpawnQueue(passed);
            foreach (var module in Modules)
                module.Update(passed);
        }

        public EnemyInstance? GetBestEnemy(ProjectileInstance projectile) => GetBestEnemy(projectile, float.MaxValue, TargetingTypes.Closest, projectile.GetDefinition().CanTarget);
        public EnemyInstance? GetBestEnemy(TurretInstance turret, float range) => GetBestEnemy(turret, range, turret.TargetingType, turret.GetDefinition().CanTarget);
        public EnemyInstance? GetBestEnemy(IPosition item, float range, TargetingTypes targetingType, HashSet<EnemyTerrrainTypes> canTarget)
        {
            range = (float)Math.Pow(range, 2);
            float dist = 0;
            EnemyInstance? best = null;
            switch (targetingType)
            {
                case TargetingTypes.Closest:
                    var minDist = float.MaxValue;
                    foreach (var target in canTarget)
                    {
                        foreach (var enemy in Context.CurrentEnemies.EnemiesByTerrain[target])
                        {
                            dist = MathHelpers.SqrDistance(item, enemy);
                            if (dist <= range && dist < minDist)
                            {
                                minDist = dist;
                                best = enemy;
                            }
                        }
                    }
                    break;
                case TargetingTypes.Weakest:
                    var lowestHP = float.MaxValue;
                    foreach (var target in canTarget)
                    {
                        foreach (var enemy in Context.CurrentEnemies.EnemiesByTerrain[target])
                        {
                            dist = MathHelpers.SqrDistance(item, enemy);
                            if (dist <= range && enemy.Health < lowestHP)
                            {
                                lowestHP = enemy.Health;
                                best = enemy;
                            }
                        }
                    }
                    break;
                case TargetingTypes.Strongest:
                    var highestHP = 0f;
                    foreach (var target in canTarget)
                    {
                        foreach (var enemy in Context.CurrentEnemies.EnemiesByTerrain[target])
                        {
                            dist = MathHelpers.SqrDistance(item, enemy);
                            if (dist <= range && enemy.Health > highestHP)
                            {
                                highestHP = enemy.Health;
                                best = enemy;
                            }
                        }
                    }
                    break;

            }
            return best;
        }

        public bool DamageEnemy(EnemyInstance enemy, float damage, Guid turretDefinitionID)
        {
            if (CheatsHelper.Cheats.Contains(CheatTypes.DamageX10))
                damage *= 10;
            enemy.Health -= damage;
            if (enemy.Health <= 0)
            {
                var enemyDef = enemy.GetDefinition();
                var amount = (int)(enemyDef.Reward * Context.GameStyle.RewardMultiplier);
                Context.Money += amount;
                Context.Stats.MoneyEarned(amount);

                Context.Stats.Score += enemyDef.Reward;
                Context.CurrentEnemies.Remove(enemy);
                OnEnemyKilled?.Invoke(enemy);

                Context.Stats.EnemyKilled(enemy.DefinitionID, turretDefinitionID);
                return true;
            }
            return false;
        }

        public void UpdateEnemiesToSpawnList()
        {
            int waveSize = (int)Context.Evolution;
            if (CheatsHelper.Cheats.Contains(CheatTypes.EnemiesX100))
                waveSize *= 100;
            if (waveSize > 50)
                waveSize = 50;
            for (int i = Context.EnemiesToSpawn.Count; i < 4; i++)
            {
                var wave = new List<Guid>();
                for (int j = 0; j < waveSize; j++)
                {
                    Guid? newEnemy = null;

                    if ((_waveQueue + 1) % Context.GameStyle.BossEveryNWave == 0 && SingleEnemiesModule.EnemyOptions.Count > 0)
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
            Context.Stats.MoneyEarned(Context.GameStyle.MoneyPrWave);
            Context.Wave++;
            Context.Stats.WaveStarted();
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
                OnEnemySpawned?.Invoke(enemy);
            }
            foreach (var remove in newToAdd)
                _spawnQueue.Remove(remove);
        }
    }
}
