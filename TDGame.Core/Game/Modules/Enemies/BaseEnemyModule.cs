using TDGame.Core.Game.Helpers;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Enemies.Modules;
using TDGame.Core.Resources;

namespace TDGame.Core.Game.Modules.Enemies
{
    public abstract class BaseEnemyModule<T> : IGameEnemyModule where T : IEnemyModule
    {
        public GameEngine Game { get; }
        public HashSet<Guid> EnemyOptions { get; }
        private Dictionary<int, List<Guid>> _enemyLevels = new Dictionary<int, List<Guid>>();
        private Random _rnd = new Random();
        public BaseEnemyModule(GameEngine game)
        {
            Game = game;
            var options = ResourceManager.Enemies.GetResources();
            EnemyOptions = new HashSet<Guid>();
            foreach (var option in options)
            {
                if (!Game.GameStyle.EnemyBlackList.Contains(option))
                {
                    var enemy = ResourceManager.Enemies.GetResource(option);
                    if (enemy.ModuleInfo is T)
                    {
                        EnemyOptions.Add(option);
                        if (_enemyLevels.ContainsKey(enemy.AvailableAtWave))
                            _enemyLevels[enemy.AvailableAtWave].Add(option);
                        else
                            _enemyLevels.Add(enemy.AvailableAtWave, new List<Guid>() { option });
                    }
                }
            }
        }

        public abstract void Update(TimeSpan passed);
        public abstract List<EnemyInstance> QueueEnemies(Guid id);
        public abstract List<EnemyInstance> UpdateSpawnQueue(TimeSpan passed, List<EnemyInstance> queue);

        internal bool UpdateEnemy(TimeSpan passed, EnemyInstance enemy, ISlowable def)
        {
            if (def.SlowingDuration > 0)
                def.SlowingDuration -= passed.Milliseconds;
            var target = Game.Map.WayPoints[enemy.WayPointID];
            if (MathHelpers.Distance(enemy, target) < 5)
            {
                enemy.WayPointID++;
                if (enemy.WayPointID >= Game.Map.WayPoints.Count)
                {
                    Game.DamagePlayer();
                    return true;
                }
                target = Game.Map.WayPoints[enemy.WayPointID];
            }
            enemy.Angle = Game.GetAngle(target, enemy);
            var change = MathHelpers.GetPredictedLocation(enemy.Angle, def.GetSpeed(), Game.GameStyle.EnemySpeedMultiplier);
            enemy.X += change.X;
            enemy.Y += change.Y;
            return false;
        }

        public Guid? GetRandomEnemy(int wave)
        {
            var options = _enemyLevels.Keys.Where(x => x <= wave).ToList();
            if (options.Count == 0)
                return null;
            var targetLevel = options[_rnd.Next(0, options.Count)];
            if (_enemyLevels[targetLevel].Count == 0)
                return null;
            var targetEnemy = _enemyLevels[targetLevel][_rnd.Next(0, _enemyLevels[targetLevel].Count)];
            return targetEnemy;
        }
    }
}
