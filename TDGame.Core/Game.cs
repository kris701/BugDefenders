﻿using System;
using System.Collections.Generic;
using System.Reflection;
using TDGame.Core.Enemies;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;
using TDGame.Core.Turret;
using TDGame.Core.Turrets;
using TDGame.Core.Turrets.Upgrades;

namespace TDGame.Core
{
    public partial class Game
    {
        private List<EnemyDefinition> _spawnQueue = new List<EnemyDefinition>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private GameTimer _mainLoopTimer;
        private Random _rnd = new Random();

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

            UpdateEnemiesToSpawnList();
        }

        private void UpdateEnemiesToSpawnList()
        {
            var options = EnemyBuilder.GetEnemies();
            for (int i = EnemiesToSpawn.Count; i < GameStyle.EnemySpawnQuantity; i++)
                EnemiesToSpawn.Add(options[_rnd.Next(0, options.Count)]);
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
                var toAdd = new List<EnemyDefinition>();
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
                        toAdd.Add(enemy);
                }
                foreach(var add in toAdd)
                {
                    CurrentEnemies.Add(add);
                    _spawnQueue.Remove(add);
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
                var xMod = Math.Cos(enemy.Angle);
                var yMod = Math.Sin(enemy.Angle);
                enemy.X += (int)Math.Ceiling(xMod * enemy.Speed * GameStyle.EnemySpeedMultiplier);
                enemy.Y += (int)Math.Ceiling(yMod * enemy.Speed * GameStyle.EnemySpeedMultiplier);
            }
            foreach (var enemy in toRemove)
                CurrentEnemies.Remove(enemy);
        }

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
            for (int i = 0; i < GameStyle.EnemyQuantity; i++)
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
                        case TurretType.Laser: UpdateGatlingTurret(turret); break;
                        case TurretType.Projectile: UpdateProjectileTurret(turret); break;
                    }
                }
            }
        }

        private EnemyDefinition? GetNearestEnemy(TurretDefinition turret) => GetNearestEnemy(turret.X, turret.Y, turret.Range);
        private EnemyDefinition? GetNearestEnemy(ProjectileDefinition projectile) => GetNearestEnemy(projectile.X, projectile.Y, projectile.Range);
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

        private void UpdateGatlingTurret(TurretDefinition turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                if (!DamageEnemy(best, turret.Damage))
                {
                    turret.Targeting = best;
                    turret.Angle = GetAngle(best, turret);
                }
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateProjectileTurret(TurretDefinition turret)
        {
            turret.Targeting = null;
            var best = GetNearestEnemy(turret);
            if (best != null && turret.ProjectileID != null)
            {
                var projectile = ProjectileBuilder.GetProjectile(turret.ProjectileID);
                projectile.Angle = GetAngle(best, turret);
                turret.Angle = projectile.Angle;
                projectile.X = turret.X + turret.Size / 2 - projectile.Size / 2;
                projectile.Y = turret.Y + turret.Size / 2 - projectile.Size / 2;
                foreach (var level in turret.ProjectileLevels)
                    if (level.HasUpgrade)
                        level.ApplyUpgrade(projectile);
                Projectiles.Add(projectile);
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
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
                projectile.Speed = (int)Math.Ceiling(projectile.Speed * projectile.Acceleration);
                if (projectile.Speed > GameStyle.ProjectileSpeedCap)
                    projectile.Speed = GameStyle.ProjectileSpeedCap;
                projectile.Traveled += projectile.Speed;
                if (projectile.Traveled > projectile.Range)
                {
                    toRemove.Add(projectile);
                    continue;
                }
                projectile.X += (int)Math.Ceiling(xMod * projectile.Speed);
                projectile.Y += (int)Math.Ceiling(yMod * projectile.Speed);

                var minDist = double.MaxValue;
                foreach (var enemy in CurrentEnemies)
                {
                    var dist = MathHelpers.Distance(projectile.X, projectile.Y, enemy.X, enemy.Y);
                    if (dist < minDist)
                        minDist = dist;
                }
                if (minDist <= projectile.TriggerRange)
                {
                    for(int i = 0; i < CurrentEnemies.Count; i++)
                    {
                        var dist = MathHelpers.Distance(projectile.X, projectile.Y, CurrentEnemies[i].X, CurrentEnemies[i].Y);
                        if (dist < projectile.SplashRange)
                            if (DamageEnemy(CurrentEnemies[i], projectile.Damage))
                                i--;
                    }
                    toRemove.Add(projectile);
                }

                if (projectile.X < 0 || projectile.X > Map.Width ||
                    projectile.Y < 0 || projectile.Y > Map.Height)
                    toRemove.Add(projectile);
            }
            foreach (var projectile in toRemove)
                Projectiles.Remove(projectile);
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

            return true;
        }

        private bool DamageEnemy(EnemyDefinition enemy, int damage)
        {
            enemy.Health -= damage;
            if (enemy.Health <= 0)
            {
                Money += enemy.Reward;
                Score += enemy.Reward;
                CurrentEnemies.Remove(enemy);
                return true;
            }
            return false;
        }

        public bool LevelUpTurret(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.TurretLevels[levelIndex];
            if (Money < upgrade.Cost)
                return false;
            if (levelIndex > 0 && !turret.TurretLevels[levelIndex - 1].HasUpgrade)
                return false;
            
            Money -= upgrade.Cost;
            upgrade.ApplyUpgrade(turret);
            return true;
        }

        public bool LevelUpProjectile(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.ProjectileLevels[levelIndex];
            if (Money < upgrade.Cost)
                return false;
            if (levelIndex > 0 && !turret.ProjectileLevels[levelIndex - 1].HasUpgrade)
                return false;

            Money -= upgrade.Cost;
            upgrade.ApplyUpgrade(turret);
            return true;
        }
    }
}
