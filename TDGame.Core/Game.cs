using TDGame.Core.Enemies;
using TDGame.Core.GameStyles;
using TDGame.Core.Helpers;
using TDGame.Core.Maps;

namespace TDGame.Core
{
    public class Game
    {
        public MapDefinition Map { get; }
        public GameStyle GameStyle { get; }
        private int _enemiesToSpawnLimit = 5;
        public List<string> EnemiesToSpawn { get; internal set; }
        public bool AutoSpawn { get; set; } = true;
        public double Evolution { get; internal set; } = 1;
        public int EnemySpawnDistance { get; internal set; } = 10;
        public bool Running { get; set; } = true;
        public List<Enemy> CurrentEnemies { get; internal set; }
        public int HP { get; internal set; }

        private List<Enemy> _spawnQueue = new List<Enemy>();
        private GameTimer _enemySpawnTimer;
        private GameTimer _evolutionTimer;
        private Random _rnd = new Random();

        public Game(string mapName, string style)
        {
            Map = MapBuilder.GetMap(mapName);
            GameStyle = GameStyleBuilder.GetGameStyle(style);
            HP = GameStyle.StartingHP;
            _enemySpawnTimer = new GameTimer(TimeSpan.FromSeconds(1), QueueEnemies);
            _evolutionTimer = new GameTimer(TimeSpan.FromSeconds(10), () => { Evolution *= GameStyle.EvolutionRate; });

            CurrentEnemies = new List<Enemy>();
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
            var toRemove = new List<Enemy>();
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

        private double GetAngle(WayPoint target, Enemy enemy)
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

        private static double Distance(Enemy e1, Enemy e2) => Distance(e1.X, e1.Y, e2.X, e2.Y);
        private static double Distance(Enemy e1, WayPoint w2) => Distance(e1.X, e1.Y, w2.X, w2.Y);
        private static double Distance(int x1, double y1, double x2, double y2) => Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
}
