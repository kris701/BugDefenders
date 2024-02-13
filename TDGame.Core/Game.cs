using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;
using TDGame.Core.Models;

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

        private List<EnemyInstance> _spawnQueue = new List<EnemyInstance>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private GameTimer _mainLoopTimer;
        private Random _rnd = new Random();

        private int _spawned = 1;
        private List<Guid> _normalEnemies = new List<Guid>();
        private List<Guid> _bossEnemies = new List<Guid>();

        public Game(Guid mapID, Guid styleID)
        {
            CurrentEnemies = new List<EnemyInstance>();
            Turrets = new List<TurretInstance>();
            EnemiesToSpawn = new List<Guid>();
            Projectiles = new List<ProjectileInstance>();

            Map = MapBuilder.GetMap(mapID);
            GameStyle = GameStyleBuilder.GetGameStyle(styleID);
            HP = GameStyle.StartingHP;
            Money = GameStyle.StartingMoney;
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { if (CurrentEnemies.Count == 0) QueueEnemies(); });
            _evolutionTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { Evolution *= GameStyle.EvolutionRate; });
            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);

            var options = EnemyBuilder.GetEnemies();
            foreach(var enemy in options)
            {
                if (EnemyBuilder.GetEnemy(enemy).IsBoss)
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
            UpdateEnemyPositions();
            UpdateProjectiles();
            UpdateTurrets(_mainLoopTimer.Target);
        }

        private void DamagePlayer()
        {
            HP--;
            if (HP <= 0)
                EndGame();
        }

        private float GetAngle(FloatPoint target, IPosition item) => MathHelpers.GetAngle(target.X, target.Y, item.X + item.Size / 2, item.Y + item.Size / 2);
        private float GetAngle(IPosition item1, IPosition item2) => MathHelpers.GetAngle(item1.X, item1.Y, item2.X + item2.Size / 2, item2.Y + item2.Size / 2);

        private EnemyInstance? GetNearestEnemy(TurretInstance turret) => GetNearestEnemy(turret.X + turret.Size / 2, turret.Y + turret.Size / 2, turret.GetDefinition().Range);
        private EnemyInstance? GetNearestEnemy(ProjectileInstance projectile) => GetNearestEnemy(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, projectile.GetDefinition().Range);
        private EnemyInstance? GetNearestEnemy(double x, double y, float range)
        {
            var minDist = float.MaxValue;
            EnemyInstance? best = null;
            foreach (var enemy in CurrentEnemies)
            {
                var dist = MathHelpers.Distance(x, y, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2);
                if (dist <= range && dist < minDist)
                {
                    minDist = dist;
                    best = enemy;
                }
            }
            return best;
        }

        private bool DamageEnemy(EnemyInstance enemy, float damage, List<DamageModifier> modifiers)
        {
            foreach (var modifier in modifiers)
                if (modifier.EnemyType == enemy.GetDefinition().EnemyType)
                    damage = damage * modifier.Modifier;
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
