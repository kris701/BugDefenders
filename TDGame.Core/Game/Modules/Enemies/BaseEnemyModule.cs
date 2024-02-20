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
        public BaseEnemyModule(GameEngine game)
        {
            Game = game;
            var options = ResourceManager.Enemies.GetResources();
            EnemyOptions = new HashSet<Guid>();
            foreach (var option in options)
                if (!Game.GameStyle.EnemyBlackList.Contains(option))
                    if (ResourceManager.Enemies.GetResource(option).ModuleInfo is T)
                        EnemyOptions.Add(option);
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
    }
}
