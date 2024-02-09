using System;
using System.Collections.Generic;
using TDGame.Core.Enemies;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;
using TDGame.Core.Turrets;

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
            Rockets = new List<RocketDefinition>();
            Missiles = new List<MissileDefinition>();

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
        }

        public void Update(TimeSpan passed)
        {
            if (Running)
            {
                if (AutoSpawn)
                    _enemySpawnTimer.Update(passed);
                _evolutionTimer.Update(passed);
                _mainLoopTimer.Update(passed);
            }
        }

        private void MainLoop()
        {
            UpdateSpawnQueue();
            UpdateEnemyPositions();
            UpdateRockets();
            UpdateMissiles();
            UpdateTurrets(_mainLoopTimer.Target);
        }

        private void UpdateSpawnQueue()
        {
            if (_spawnQueue.Count > 0)
            {
                var first = _spawnQueue[0];
                var minDist = double.MaxValue;
                foreach (var enemy in CurrentEnemies)
                    if (enemy.GroupID == first.GroupID)
                        minDist = Math.Min(MathHelpers.Distance(first, enemy), minDist);
                if (minDist > EnemySpawnDistance)
                {
                    CurrentEnemies.Add(first);
                    _spawnQueue.RemoveAt(0);
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
                var angle = GetAngle(target, enemy);
                var xMod = Math.Cos(angle);
                var yMod = Math.Sin(angle);
                enemy.X += (int)Math.Ceiling(xMod * enemy.Speed * GameStyle.EnemySpeedMultiplier);
                enemy.Y += (int)Math.Ceiling(yMod * enemy.Speed * GameStyle.EnemySpeedMultiplier);
            }
            foreach (var enemy in toRemove)
                CurrentEnemies.Remove(enemy);
        }

        private double GetAngle(WayPoint target, EnemyDefinition enemy) => GetAngle(target.X, target.Y, enemy.X, enemy.Y);
        private double GetAngle(EnemyDefinition enemy, TurretDefinition turret) => GetAngle(enemy.X, enemy.Y, turret.X, turret.Y);
        private double GetAngle(EnemyDefinition enemy, MissileDefinition missile) => GetAngle(enemy.X, enemy.Y, missile.X, missile.Y);

        private double GetAngle(int x1, int y1, int x2, int y2)
        {
            var a = y1 - y2;
            var b = x1 - x2;
            return Math.Atan2(a, b);
        }

        public void QueueEnemies()
        {
            var group = Guid.NewGuid();
            for (int i = 0; i < GameStyle.EnemyQuantity; i++)
            {
                var enemy = EnemyBuilder.GetEnemy(EnemiesToSpawn[0], Evolution);
                enemy.X = Map.WayPoints[0].X;
                enemy.Y = Map.WayPoints[0].Y;
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
                        case TurretType.Bullets: UpdateGatlingTurret(turret); break;
                        case TurretType.Rockets: UpdateRocketTurret(turret); break;
                        case TurretType.Missile: UpdateMissileTurret(turret); break;
                    }
                }
            }
        }

        private EnemyDefinition? GetNearestEnemy(TurretDefinition turret) => GetNearestEnemy(turret.X, turret.Y, turret.Range);
        private EnemyDefinition? GetNearestEnemy(MissileDefinition missile) => GetNearestEnemy(missile.X, missile.Y, missile.Range);
        private EnemyDefinition? GetNearestEnemy(int x, int y, int range)
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
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                turret.Targeting = null;
                if (!DamageEnemy(best, turret.Damage))
                    turret.Targeting = best;
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateRocketTurret(TurretDefinition turret)
        {
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                turret.Targeting = best;
                var angle = GetAngle(best, turret);
                Rockets.Add(new RocketDefinition()
                {
                    X = turret.X,
                    Y = turret.Y,
                    Angle = angle,
                    Speed = 1,
                    Damage = turret.Damage,
                    Range = turret.Range,
                    SplashRange = 50,
                    TriggerRange = 25,
                    Acceleration = 1.2
                });
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateMissileTurret(TurretDefinition turret)
        {
            var best = GetNearestEnemy(turret);
            if (best != null)
            {
                turret.Targeting = best;
                Missiles.Add(new MissileDefinition()
                {
                    X = turret.X,
                    Y = turret.Y,
                    Target = best,
                    Speed = 1,
                    Damage = turret.Damage,
                    Range = turret.Range,
                    SplashRange = 50,
                    TriggerRange = 25,
                    Acceleration = 1.2
                });
                turret.CoolingFor = TimeSpan.FromMilliseconds(turret.Cooldown);
            }
        }

        private void UpdateRockets()
        {
            var toRemove = new List<RocketDefinition>();
            foreach(var rocket in Rockets)
            {
                var xMod = Math.Cos(rocket.Angle);
                var yMod = Math.Sin(rocket.Angle);
                rocket.Speed = (int)Math.Ceiling(rocket.Speed * rocket.Acceleration);
                if (rocket.Speed > GameStyle.ProjectileSpeedCap)
                    rocket.Speed = GameStyle.ProjectileSpeedCap;
                rocket.Traveled += rocket.Speed;
                if (rocket.Traveled > rocket.Range)
                {
                    toRemove.Add(rocket);
                    continue;
                }
                rocket.X += (int)Math.Ceiling(xMod * rocket.Speed);
                rocket.Y += (int)Math.Ceiling(yMod * rocket.Speed);

                var minDist = double.MaxValue;
                foreach (var enemy in CurrentEnemies)
                {
                    var dist = MathHelpers.Distance(rocket.X, rocket.Y, enemy.X, enemy.Y);
                    if (dist < minDist)
                        minDist = dist;
                }
                if (minDist <= rocket.TriggerRange)
                {
                    for(int i = 0; i < CurrentEnemies.Count; i++)
                    {
                        var dist = MathHelpers.Distance(rocket.X, rocket.Y, CurrentEnemies[i].X, CurrentEnemies[i].Y);
                        if (dist < rocket.SplashRange)
                            if (DamageEnemy(CurrentEnemies[i], rocket.Damage))
                                i--;
                    }
                    toRemove.Add(rocket);
                }
            }
            foreach (var rocket in toRemove)
                Rockets.Remove(rocket);
        }

        private void UpdateMissiles()
        {
            var toRemove = new List<MissileDefinition>();
            foreach (var missile in Missiles)
            {
                if (!CurrentEnemies.Contains(missile.Target))
                {
                    var best = GetNearestEnemy(missile);
                    if (best == null)
                    {
                        toRemove.Add(missile);
                        continue;
                    }
                    missile.Target = best;
                }

                var angle = GetAngle(missile.Target, missile);
                var xMod = Math.Cos(angle);
                var yMod = Math.Sin(angle);
                missile.Speed = (int)Math.Ceiling(missile.Speed * missile.Acceleration);
                if (missile.Speed > GameStyle.ProjectileSpeedCap)
                    missile.Speed = GameStyle.ProjectileSpeedCap;
                missile.Traveled += missile.Speed;
                if (missile.Traveled > missile.Range)
                {
                    toRemove.Add(missile);
                    continue;
                }
                missile.X += (int)Math.Ceiling(xMod * missile.Speed);
                missile.Y += (int)Math.Ceiling(yMod * missile.Speed);

                var minDist = double.MaxValue;
                foreach (var enemy in CurrentEnemies)
                {
                    var dist = MathHelpers.Distance(missile.X, missile.Y, enemy.X, enemy.Y);
                    if (dist < minDist)
                        minDist = dist;
                }
                if (minDist <= missile.TriggerRange)
                {
                    for (int i = 0; i < CurrentEnemies.Count; i++)
                    {
                        var dist = MathHelpers.Distance(missile.X, missile.Y, CurrentEnemies[i].X, CurrentEnemies[i].Y);
                        if (dist < missile.SplashRange)
                            if (DamageEnemy(CurrentEnemies[i], missile.Damage))
                                i--;
                    }
                    toRemove.Add(missile);
                }
            }
            foreach (var missile in toRemove)
                Missiles.Remove(missile);
        }

        public bool AddTurret(TurretDefinition turret, WayPoint point)
        {
            if (Money < turret.Cost)
                return false;
            foreach (var block in Map.BlockingTiles)
                if (MathHelpers.Intersects(turret, point, block))
                    return false;

            foreach(var otherTurret in Turrets)
                if (MathHelpers.Intersects(otherTurret, turret, point))
                    return false;

            turret.X = point.X;
            turret.Y = point.Y;
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
                CurrentEnemies.Remove(enemy);
                return true;
            }
            return false;
        }

        public bool LevelUpTurret(TurretDefinition turret, int levelIndex)
        {
            var upgrade = turret.Levels[levelIndex];
            if (Money < upgrade.Cost)
                return false;
            if (!turret.Levels[upgrade.RequiresTurretLevel].HasUpgrade)
                return false;
            
            Money -= upgrade.Cost;
            upgrade.HasUpgrade = true;
            turret.Range = (int)(turret.Range * upgrade.RangeModifier);
            turret.Damage = (int)(turret.Damage * upgrade.DamageModifier);
            turret.Cooldown = (int)(turret.Cooldown * upgrade.CooldownModifier);
            return true;
        }
    }
}
