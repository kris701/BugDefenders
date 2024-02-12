using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Enemies;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;
using TDGame.Core.Turret;
using TDGame.Core.Turrets;
using TDGame.Core.Turrets.Upgrades;
using static TDGame.Core.Enemies.EnemyDefinition;

namespace TDGame.Core
{
    public delegate void TurretEventHandler(TurretDefinition turret);
    public delegate void EnemyEventHandler(EnemyDefinition enemy);
    public partial class Game
    {
        public EnemyEventHandler? OnEnemySpawned;
        public EnemyEventHandler? OnEnemyKilled;

        public TurretEventHandler? OnTurretPurchased;
        public TurretEventHandler? OnTurretSold;
        public TurretEventHandler? OnTurretUpgraded;

        private List<EnemyDefinition> _spawnQueue = new List<EnemyDefinition>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private GameTimer _mainLoopTimer;
        private Random _rnd = new Random();

        private int _spawned = 1;
        private List<string> _normalEnemies = new List<string>();
        private List<string> _bossEnemies = new List<string>();

        public Game(string mapName, string style)
        {
            CurrentEnemies = new List<EnemyDefinition>();
            Turrets = new List<TurretDefinition>();
            EnemiesToSpawn = new List<string>();
            Projectiles = new List<ProjectileDefinition>();

            Map = MapBuilder.GetMap(mapName);
            GameStyle = GameStyleBuilder.GetGameStyle(style);
            HP = GameStyle.StartingHP;
            Money = GameStyle.StartingMoney;
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { if (CurrentEnemies.Count == 0) QueueEnemies(); });
            _evolutionTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { Evolution *= GameStyle.EvolutionRate; });
            _mainLoopTimer = new GameTimer(TimeSpan.FromMilliseconds(30), MainLoop);

            var options = EnemyBuilder.GetEnemies();
            foreach(var enemy in options)
            {
                if (EnemyBuilder.GetEnemy(enemy, 0).IsBoss)
                    _bossEnemies.Add(enemy);
                else
                    _normalEnemies.Add(enemy);
            }

            UpdateEnemiesToSpawnList();
        }

        private void UpdateEnemiesToSpawnList()
        {
            for (int i = EnemiesToSpawn.Count; i < 5; i++)
            {
                if (_spawned++ % GameStyle.BossEveryNWave == 0)
                    EnemiesToSpawn.Add(_bossEnemies[_rnd.Next(0, _bossEnemies.Count)]);
                else
                    EnemiesToSpawn.Add(_normalEnemies[_rnd.Next(0, _normalEnemies.Count)]);
            }
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

        private void UpdateSpawnQueue()
        {
            if (_spawnQueue.Count > 0)
            {
                var vistedGroups = new List<Guid>();
                var enemiesToAdd = new List<EnemyDefinition>();
                foreach(var enemy in _spawnQueue)
                {
                    if (vistedGroups.Contains(enemy.GroupID))
                        continue;
                    vistedGroups.Add(enemy.GroupID);
                    var minDist = double.MaxValue;
                    foreach (var CurrentEnemy in CurrentEnemies)
                        if (CurrentEnemy.GroupID == enemy.GroupID)
                            minDist = Math.Min(MathHelpers.Distance(enemy, CurrentEnemy), minDist);
                    if (minDist > enemy.Size)
                        enemiesToAdd.Add(enemy);
                }
                foreach(var enemy in enemiesToAdd)
                {
                    CurrentEnemies.Add(enemy);
                    _spawnQueue.Remove(enemy);
                    if (OnEnemySpawned != null)
                        OnEnemySpawned.Invoke(enemy);
                }
            }
        }

        private void UpdateEnemyPositions()
        {
            var toRemove = new List<EnemyDefinition>();
            foreach(var enemy in CurrentEnemies)
            {
                var target = Map.WayPoints[enemy.WayPointID];
                if (MathHelpers.Distance(enemy, target) < 5)
                {
                    enemy.WayPointID++;
                    if (enemy.WayPointID >= Map.WayPoints.Count)
                    {
                        HP--;
                        if (HP <= 0)
                            EndGame();
                        toRemove.Add(enemy);
                        continue;
                    }
                    target = Map.WayPoints[enemy.WayPointID];
                }
                enemy.Angle = GetAngle(target, enemy);
                var change = GetEnemyLocationChange(enemy.Angle, (float)enemy.Speed);
                enemy.X += change.X;
                enemy.Y += change.Y;
            }
            foreach (var enemy in toRemove)
                CurrentEnemies.Remove(enemy);
        }

        private WayPoint GetEnemyLocationChange(float angle, float speed)
        {
            var xMod = Math.Cos(angle);
            var yMod = Math.Sin(angle);
            return new WayPoint(
                (float)xMod * speed * (float)GameStyle.EnemySpeedMultiplier,
                (float)yMod * speed * (float)GameStyle.EnemySpeedMultiplier);
        }

        private float GetAngle(WayPoint target, TurretDefinition turret) => GetAngle(target.X, target.Y, turret.X + turret.Size / 2, turret.Y + turret.Size / 2);
        private float GetAngle(WayPoint target, EnemyDefinition enemy) => GetAngle(target.X, target.Y, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2);
        private float GetAngle(EnemyDefinition enemy, TurretDefinition turret) => GetAngle(enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2, turret.X + turret.Size / 2, turret.Y + turret.Size / 2);
        private float GetAngle(EnemyDefinition enemy, ProjectileDefinition projectile) => GetAngle(enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2, projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2);

        private float GetAngle(float x1, float y1, float x2, float y2)
        {
            var a = y1 - y2;
            var b = x1 - x2;
            return (float)Math.Atan2(a, b);
        }

        public void QueueEnemies()
        {
            var group = Guid.NewGuid();
            var template = EnemyBuilder.GetEnemy(EnemiesToSpawn[0], Evolution);
            for (int i = 0; i < template.WaveSize * GameStyle.EnemyWaveMultiplier; i++)
            {
                var enemy = EnemyBuilder.GetEnemy(EnemiesToSpawn[0], Evolution);
                enemy.X = Map.WayPoints[0].X - enemy.Size / 2;
                enemy.Y = Map.WayPoints[0].Y - enemy.Size / 2;
                enemy.GroupID = group;
                _spawnQueue.Add(enemy);
            }
            EnemiesToSpawn.RemoveAt(0);
            UpdateEnemiesToSpawnList();
        }

        private void UpdateTurrets(TimeSpan passed)
        {
            foreach (var turret in Turrets)
            {
                turret.CoolingFor -= passed;

                if (turret.CoolingFor <= TimeSpan.Zero)
                {
                    switch (turret.Type)
                    {
                        case TurretType.Laser: UpdateLaserTurret(turret); break;
                        case TurretType.Projectile: UpdateProjectileTurret(turret); break;
                    }
                }
            }
        }

        private EnemyDefinition? GetNearestEnemy(TurretDefinition turret) => GetNearestEnemy(turret.X + turret.Size / 2, turret.Y + turret.Size / 2, turret.Range);
        private EnemyDefinition? GetNearestEnemy(ProjectileDefinition projectile) => GetNearestEnemy(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, projectile.Range);
        private EnemyDefinition? GetNearestEnemy(double x, double y, int range)
        {
            var minDist = double.MaxValue;
            EnemyDefinition? best = null;
            foreach (var enemy in CurrentEnemies)
            {
                var dist = MathHelpers.Distance(x, y, enemy.X, enemy.Y);
                if (dist <= range && dist < minDist)
                {
                    minDist = dist;
                    best = enemy;
                }
            }
            return best;
        }

        private void UpdateLaserTurret(TurretDefinition turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                if (!DamageEnemy(best, turret.Damage, turret.StrongAgainst, turret.WeakAgainst))
                {
                    turret.Targeting = best;
                    turret.Angle = GetAngle(best, turret);
                }
                else
                    turret.Kills++;
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateProjectileTurret(TurretDefinition turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null && turret.ProjectileID != null)
            {
                var projectile = GetProjectileForTurret(turret);
                if (turret.IsTrailing)
                    projectile.Angle = GetAngle(
                        GetTrailingPoint(best, projectile), 
                        turret);
                else
                    projectile.Angle = GetAngle(best, turret);
                turret.Angle = projectile.Angle;
                Projectiles.Add(projectile);
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private WayPoint GetTrailingPoint(EnemyDefinition enemy, ProjectileDefinition projectile)
        {
            float x = enemy.X + enemy.Size / 2;
            float y = enemy.Y + enemy.Size / 2;
            var dist = MathHelpers.Distance(enemy, projectile);
            var steps = dist / projectile.Speed;
            var change = GetEnemyLocationChange(enemy.Angle, (float)enemy.Speed);
            return new WayPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
        }

        public ProjectileDefinition GetProjectileForTurret(TurretDefinition turret)
        {
            if (turret.ProjectileID == null)
                throw new ArgumentNullException("Projectile ID was null!");
            var projectile = ProjectileBuilder.GetProjectile(turret.ProjectileID);
            projectile.Source = turret;
            projectile.X = turret.X + turret.Size / 2 - projectile.Size / 2;
            projectile.Y = turret.Y + turret.Size / 2 - projectile.Size / 2;
            foreach (var level in turret.ProjectileLevels)
                if (level.HasUpgrade)
                    level.ApplyUpgrade(projectile);
            return projectile;
        }

        private void UpdateProjectiles()
        {
            var toRemove = new List<ProjectileDefinition>();
            foreach(var projectile in Projectiles)
            {
                if (projectile.IsGuided)
                {
                    if (!CurrentEnemies.Contains(projectile.Target))
                    {
                        var best = GetNearestEnemy(projectile);
                        if (best == null)
                        {
                            toRemove.Add(projectile);
                            continue;
                        }
                        projectile.Target = best;
                    }
                    projectile.Angle = GetAngle(projectile.Target, projectile);
                }

                var xMod = Math.Cos(projectile.Angle);
                var yMod = Math.Sin(projectile.Angle);
                if (projectile.Acceleration != 1)
                {
                    projectile.Speed = (int)Math.Ceiling(projectile.Speed * projectile.Acceleration);
                    if (projectile.Speed > GameStyle.ProjectileSpeedCap)
                        projectile.Speed = GameStyle.ProjectileSpeedCap;
                }
                projectile.Traveled += projectile.Speed;
                if (projectile.Traveled > projectile.Range)
                {
                    toRemove.Add(projectile);
                    continue;
                }

                if (projectile.Size >= 10)
                {
                    projectile.X += (float)xMod * projectile.Speed;
                    projectile.Y += (float)yMod * projectile.Speed;

                    if (IsWithinTriggerRange(projectile) || 
                        projectile.X < 0 || projectile.X > Map.Width ||
                        projectile.Y < 0 || projectile.Y > Map.Height)
                        toRemove.Add(projectile);
                }
                else
                {
                    for(int i = 0; i < 5; i++)
                    {
                        projectile.X += (float)xMod * ((float)projectile.Speed / 5);
                        projectile.Y += (float)yMod * ((float)projectile.Speed / 5);

                        if (IsWithinTriggerRange(projectile) ||
                            projectile.X < 0 || projectile.X > Map.Width ||
                            projectile.Y < 0 || projectile.Y > Map.Height)
                        {
                            toRemove.Add(projectile);
                            break;
                        }
                    }
                }
            }
            foreach (var projectile in toRemove)
                Projectiles.Remove(projectile);
        }

        private bool IsWithinTriggerRange(ProjectileDefinition projectile)
        {
            bool isWithin = false;
            foreach (var enemy in CurrentEnemies)
            {
                if (MathHelpers.Distance(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2) < projectile.TriggerRange)
                {
                    isWithin = true;
                    break;
                }
            }
            if (isWithin)
            {
                for (int i = 0; i < CurrentEnemies.Count; i++)
                {
                    var dist = MathHelpers.Distance(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, CurrentEnemies[i].X + CurrentEnemies[i].Size / 2, CurrentEnemies[i].Y + CurrentEnemies[i].Size / 2);
                    if (dist < projectile.SplashRange)
                    {
                        if (DamageEnemy(CurrentEnemies[i], projectile.Damage, projectile.StrongAgainst, projectile.WeakAgainst))
                        {
                            projectile.Source.Kills++;
                            i--;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool AddTurret(TurretDefinition turret)
        {
            if (Money < turret.Cost)
                return false;
            if (turret.X < 0)
                return false;
            if (turret.X > Map.Width - turret.Size)
                return false;
            if (turret.Y < 0)
                return false;
            if (turret.Y > Map.Height - turret.Size)
                return false;

            foreach (var block in Map.BlockingTiles)
                if (MathHelpers.Intersects(turret, block))
                    return false;

            foreach(var otherTurret in Turrets)
                if (MathHelpers.Intersects(otherTurret, turret))
                    return false;

            Money -= turret.Cost;
            Turrets.Add(turret);
            if (OnTurretPurchased != null)
                OnTurretPurchased.Invoke(turret);

            return true;
        }

        private bool DamageEnemy(EnemyDefinition enemy, int damage, List<EnemyTypes> strengths, List<EnemyTypes> weaknesses)
        {
            if (strengths.Contains(enemy.Type))
                damage = (int)(damage * GameStyle.StrengthModifier);
            if (weaknesses.Contains(enemy.Type))
                damage = (int)(damage * GameStyle.WeaknessModifier);
            enemy.Health -= damage;
            if (enemy.Health <= 0)
            {
                Money += enemy.Reward;
                Score += enemy.Reward;
                CurrentEnemies.Remove(enemy);
                if (OnEnemyKilled != null)
                    OnEnemyKilled.Invoke(enemy);
                return true;
            }
            return false;
        }

        public bool CanLevelUpTurret(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.TurretLevels[levelIndex];
            if (Money < upgrade.Cost)
                return false;
            if (levelIndex > 0 && !turret.TurretLevels[levelIndex - 1].HasUpgrade)
                return false;
            return true;
        }
        public bool LevelUpTurret(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.TurretLevels[levelIndex];
            if (!CanLevelUpTurret(turret, levelIndex))
                return false;

            Money -= upgrade.Cost;
            upgrade.ApplyUpgrade(turret);
            if (OnTurretUpgraded != null)
                OnTurretUpgraded.Invoke(turret);
            return true;
        }

        public bool CanLevelUpProjectile(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.TurretLevels[levelIndex];
            if (Money < upgrade.Cost)
                return false;
            if (levelIndex > 0 && !turret.ProjectileLevels[levelIndex - 1].HasUpgrade)
                return false;

            return true;
        }
        public bool LevelUpProjectile(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.ProjectileLevels[levelIndex];
            if (!CanLevelUpProjectile(turret, levelIndex))
                return false;

            Money -= upgrade.Cost;
            upgrade.ApplyUpgrade(turret);
            if (OnTurretUpgraded != null)
                OnTurretUpgraded.Invoke(turret);
            return true;
        }

        public int GetTurretWorth(TurretDefinition turret)
        {
            var worth = turret.Cost;
            foreach (var upgrade in turret.GetAllUpgrades())
                if (upgrade.HasUpgrade)
                    worth += upgrade.Cost;
            return worth;
        }

        public void SellTurret(TurretDefinition turret)
        {
            if (!Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            Money += GetTurretWorth(turret);
            Turrets.Remove(turret);

            if (OnTurretSold != null)
                OnTurretSold.Invoke(turret);
        }
    }
}
