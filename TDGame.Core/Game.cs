using System.Collections.Generic;
using TDGame.Core.Enemies;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;
using TDGame.Core.Maps.Tiles;
using TDGame.Core.Turrets;

namespace TDGame.Core
{
    public class Game
    {
        public MapDefinition Map { get; }
        public GameStyleDefinition GameStyle { get; }
        private int _enemiesToSpawnLimit = 5;
        public List<string> EnemiesToSpawn { get; internal set; }
        public bool AutoSpawn { get; set; } = true;
        public double Evolution { get; internal set; } = 1;
        public int EnemySpawnDistance { get; internal set; } = 10;
        public bool Running { get; set; } = true;
        public List<EnemyDefinition> CurrentEnemies { get; internal set; }
        public int HP { get; internal set; }
        public int Money { get; internal set; }
        public List<TurretDefinition> Turrets { get; internal set; }

        private List<EnemyDefinition> _spawnQueue = new List<EnemyDefinition>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private Random _rnd = new Random();

        public Game(string mapName, string style)
        {
            Map = MapBuilder.GetMap(mapName);
            GameStyle = GameStyleBuilder.GetGameStyle(style);
            HP = GameStyle.StartingHP;
            Money = GameStyle.StartingMoney;
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), QueueEnemies);
            _evolutionTimer = new GameTimer(TimeSpan.FromSeconds(1), () => { Evolution *= GameStyle.EvolutionRate; });

            CurrentEnemies = new List<EnemyDefinition>();
            Turrets = new List<TurretDefinition>();
            EnemiesToSpawn = new List<string>();
            UpdateEnemiesToSpawnList();
        }

        private void UpdateEnemiesToSpawnList()
        {
            var options = EnemyBuilder.GetEnemies();
            for (int i = EnemiesToSpawn.Count; i < _enemiesToSpawnLimit; i++)
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

                UpdateSpawnQueue();
                UpdateEnemyPositions();
                UpdateTurrets(passed);
            }
        }

        private void UpdateSpawnQueue()
        {
            if (_spawnQueue.Count > 0)
            {
                var first = _spawnQueue[0];
                var minDist = double.MaxValue;
                foreach (var enemy in CurrentEnemies)
                    if (enemy.GroupID == first.GroupID)
                        minDist = Math.Min(Distance(first, enemy), minDist);
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
                if (Distance(enemy, target) < 5)
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

        private double GetAngle(WayPoint target, EnemyDefinition enemy)
        {
            var a = target.Y - enemy.Y;
            var b = target.X - enemy.X;
            return Math.Atan2(a, b);
        }

        private void QueueEnemies()
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
                    }
                }
            }
        }

        private EnemyDefinition? GetNearestEnemy(TurretDefinition turret)
        {
            var minDist = double.MaxValue;
            EnemyDefinition? best = null;
            foreach (var enemy in CurrentEnemies)
            {
                var dist = Distance(turret, enemy);
                if (dist < minDist)
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

        public bool AddTurret(TurretDefinition turret, WayPoint point)
        {
            foreach (var block in Map.BlockingTiles)
                if (Intersects(turret, point, block))
                    return false;

            foreach(var otherTurret in Turrets)
                if (Intersects(otherTurret, turret))
                    return false;

            turret.X = point.X;
            turret.Y = point.Y;
            Turrets.Add(turret);

            return true;
        }

        private bool DamageEnemy(EnemyDefinition enemy, int damage)
        {
            enemy.Health -= damage;
            if (enemy.Health <= 0)
            {
                CurrentEnemies.Remove(enemy);
                return true;
            }
            return false;
        }

        private static bool Intersects(TurretDefinition turret, WayPoint turretLocation, BlockedTile tile)
        {
            float closestX = (turretLocation.X < tile.X ? tile.X : (turretLocation.X > tile.Width ? tile.Width : turretLocation.X));
            float closestY = (turretLocation.Y < tile.Y ? tile.Y : (turretLocation.Y > tile.Height ? tile.Height : turretLocation.Y));
            float dx = closestX - turretLocation.X;
            float dy = closestY - turretLocation.Y;

            return (dx * dx + dy * dy) <= turret.Size * turret.Size;
        }

        private static bool Intersects(TurretDefinition e1, TurretDefinition e2) => Distance(e1, e2) < e1.Size + e2.Size;
        private static double Distance(TurretDefinition e1, TurretDefinition e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        private static double Distance(TurretDefinition e1, EnemyDefinition e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        private static double Distance(EnemyDefinition e1, EnemyDefinition e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        private static double Distance(EnemyDefinition e1, WayPoint w2) => Distance(e1.X, e1.Y, w2.X, w2.Y);
        private static double Distance(int x1, double y1, double x2, double y2) => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
}
