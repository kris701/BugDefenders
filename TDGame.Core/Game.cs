using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Projectiles;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;

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
                var enemiesToAdd = new List<EnemyInstance>();
                foreach (var enemy in _spawnQueue)
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
                foreach (var enemy in enemiesToAdd)
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
            var toRemove = new List<EnemyInstance>();
            foreach(var enemy in CurrentEnemies)
            {
                var target = Map.WayPoints[enemy.WayPointID];
                if (MathHelpers.Distance(enemy, target) < 5)
                {
                    enemy.WayPointID++;
                    if (enemy.WayPointID >= Map.WayPoints.Count)
                    {
                        DamagePlayer();
                        toRemove.Add(enemy);
                        continue;
                    }
                    target = Map.WayPoints[enemy.WayPointID];
                }
                enemy.Angle = GetAngle(target, enemy);
                var change = GetEnemyLocationChange(enemy.Angle, enemy.GetDefinition().Speed);
                enemy.X += change.X;
                enemy.Y += change.Y;
            }
            foreach (var enemy in toRemove)
                CurrentEnemies.Remove(enemy);
        }

        private void DamagePlayer()
        {
            HP--;
            if (HP <= 0)
                EndGame();
        }

        private FloatPoint GetEnemyLocationChange(float angle, float speed)
        {
            var xMod = Math.Cos(angle);
            var yMod = Math.Sin(angle);
            return new FloatPoint(
                (float)xMod * speed * (float)GameStyle.EnemySpeedMultiplier,
                (float)yMod * speed * (float)GameStyle.EnemySpeedMultiplier);
        }

        private float GetAngle(FloatPoint target, TurretInstance turret) => MathHelpers.GetAngle(target.X, target.Y, turret.X + turret.Size / 2, turret.Y + turret.Size / 2);
        private float GetAngle(FloatPoint target, EnemyInstance enemy) => MathHelpers.GetAngle(target.X, target.Y, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2);
        private float GetAngle(EnemyInstance enemy, TurretInstance turret) => MathHelpers.GetAngle(enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2, turret.X + turret.Size / 2, turret.Y + turret.Size / 2);
        private float GetAngle(EnemyInstance enemy, ProjectileInstance projectile) => MathHelpers.GetAngle(enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2, projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2);

        public void QueueEnemies()
        {
            var group = Guid.NewGuid();
            var template = EnemyBuilder.GetEnemy(EnemiesToSpawn[0]);
            for (int i = 0; i < template.WaveSize * GameStyle.EnemyWaveMultiplier; i++)
            {
                var enemy = new EnemyInstance(EnemiesToSpawn[0], Evolution);
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
                    switch (turret.GetDefinition().Type)
                    {
                        case TurretType.Laser: UpdateLaserTurret(turret); break;
                        case TurretType.Projectile: UpdateProjectileTurret(turret); break;
                        case TurretType.AOE: UpdateAOETurret(turret); break;
                    }
                }
            }
        }

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

        private void UpdateLaserTurret(TurretInstance turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                if (!DamageEnemy(best, turret.Damage, turret.GetDefinition().DamageModifiers))
                {
                    turret.Targeting = best;
                    turret.Angle = GetAngle(best, turret);
                }
                else
                    turret.Kills++;
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateAOETurret(TurretInstance turret)
        {
            turret.Targeting = null;
            var closest = float.MaxValue;
            EnemyInstance? best = null;
            var targeting = new List<EnemyInstance>();
            foreach(var enemy in CurrentEnemies)
            {
                var dist = MathHelpers.Distance(enemy, turret);
                if (dist <= turret.Range)
                {
                    targeting.Add(enemy);
                    if (dist < closest)
                    {
                        closest = dist;
                        best = enemy;
                    }
                }
            }

            if (best != null)
            {
                foreach (var enemy in targeting)
                    if (DamageEnemy(enemy, turret.Damage, turret.GetDefinition().DamageModifiers))
                        turret.Kills++;
                turret.Targeting = best;
                turret.Angle = GetAngle(best, turret);
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateProjectileTurret(TurretInstance turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null && turret.ProjectileDefinition != null)
            {
                var projectile = new ProjectileInstance(turret.ProjectileDefinition);
                projectile.X = turret.X + turret.Size / 2;
                projectile.Y = turret.Y + turret.Size / 2;
                projectile.Source = turret;
                if (turret.GetDefinition().IsTrailing)
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

        private FloatPoint GetTrailingPoint(EnemyInstance enemy, ProjectileInstance projectile)
        {
            float x = enemy.X + enemy.Size / 2;
            float y = enemy.Y + enemy.Size / 2;
            var dist = MathHelpers.Distance(enemy, projectile);
            var steps = dist / projectile.GetDefinition().Speed;
            var change = GetEnemyLocationChange(enemy.Angle, enemy.GetDefinition().Speed);
            return new FloatPoint(x + change.X * (float)steps, y + change.Y * (float)steps);
        }

        private void UpdateProjectiles()
        {
            var toRemove = new List<ProjectileInstance>();
            foreach(var projectile in Projectiles)
            {
                var projectileDef = projectile.GetDefinition();
                if (projectileDef.IsGuided && projectile.Target != null)
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
                if (projectileDef.Acceleration != 1)
                {
                    projectileDef.Speed = (float)Math.Ceiling(projectileDef.Speed * projectileDef.Acceleration);
                    if (projectileDef.Speed > GameStyle.ProjectileSpeedCap)
                        projectileDef.Speed = GameStyle.ProjectileSpeedCap;
                }
                projectile.Traveled += projectileDef.Speed;
                if (projectile.Traveled > projectileDef.Range)
                {
                    toRemove.Add(projectile);
                    continue;
                }

                if (projectile.Size >= 10)
                {
                    projectile.X += (float)xMod * projectileDef.Speed;
                    projectile.Y += (float)yMod * projectileDef.Speed;

                    if (IsWithinTriggerRange(projectile) || 
                        projectile.X < 0 || projectile.X > Map.Width ||
                        projectile.Y < 0 || projectile.Y > Map.Height)
                        toRemove.Add(projectile);
                }
                else
                {
                    for(int i = 0; i < 5; i++)
                    {
                        projectile.X += (float)xMod * ((float)projectileDef.Speed / 5);
                        projectile.Y += (float)yMod * ((float)projectileDef.Speed / 5);

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

        private bool IsWithinTriggerRange(ProjectileInstance projectile)
        {
            bool isWithin = false;
            foreach (var enemy in CurrentEnemies)
            {
                if (MathHelpers.Distance(projectile.X + projectile.Size / 2, projectile.Y + projectile.Size / 2, enemy.X + enemy.Size / 2, enemy.Y + enemy.Size / 2) < projectile.GetDefinition().TriggerRange)
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
                    if (dist < projectile.GetDefinition().SplashRange)
                    {
                        if (DamageEnemy(CurrentEnemies[i], projectile.GetDefinition().Damage, projectile.GetDefinition().DamageModifiers))
                        {
                            if (projectile.Source != null)
                                projectile.Source.Kills++;
                            i--;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool AddTurret(TurretDefinition turretDef, FloatPoint at)
        {
            if (Money < turretDef.Cost)
                return false;
            if (at.X < 0)
                return false;
            if (at.X > Map.Width - turretDef.Size)
                return false;
            if (at.Y < 0)
                return false;
            if (at.Y > Map.Height - turretDef.Size)
                return false;

            foreach (var block in Map.BlockingTiles)
                if (MathHelpers.Intersects(turretDef, at, block))
                    return false;

            foreach(var otherTurret in Turrets)
                if (MathHelpers.Intersects(turretDef, at, otherTurret))
                    return false;

            var newInstance = new TurretInstance(turretDef);
            newInstance.X = at.X;
            newInstance.Y = at.Y;
            Money -= turretDef.Cost;
            Turrets.Add(newInstance);
            if (OnTurretPurchased != null)
                OnTurretPurchased.Invoke(newInstance);

            return true;
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

        public bool CanLevelUpTurret(TurretInstance turret, Guid id)
        {
            var upgrade = turret.GetDefinition().GetAllUpgrades().First(x => x.ID == id);
            if (Money < upgrade.Cost)
                return false;
            if (upgrade.Requires != null && !turret.HasUpgrades.Contains((Guid)upgrade.Requires))
                return false;
            return true;
        }

        public bool LevelUpTurret(TurretInstance turret, Guid id)
        {
            if (!CanLevelUpTurret(turret, id))
                return false;
            var upgrade = turret.GetDefinition().GetAllUpgrades().First(x => x.ID == id);
            turret.ApplyUpgrade(id);
            Money -= upgrade.Cost;

            if (OnTurretUpgraded != null)
                OnTurretUpgraded.Invoke(turret);
            return true;
        }

        public void SellTurret(TurretInstance turret)
        {
            if (!Turrets.Contains(turret))
                throw new Exception("Turret not in game!");
            Money += turret.GetTurretWorth();
            Turrets.Remove(turret);

            if (OnTurretSold != null)
                OnTurretSold.Invoke(turret);
        }
    }
}
