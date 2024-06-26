﻿using BugDefender.Core.Game.Helpers;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Game.Modules.Enemies.SubModules
{
    public abstract class BaseEnemyModule<T> : BaseGameModule where T : IEnemyModule, ISlowable
    {
        public HashSet<Guid> EnemyOptions { get; }

        private readonly Dictionary<int, List<Guid>> _enemyLevels = new Dictionary<int, List<Guid>>();
        internal Random _rnd = new Random();
        public BaseEnemyModule(GameContext context, GameEngine game) : base(context, game)
        {
            var options = ResourceManager.Enemies.GetResources();
            if (Context.GameStyle.EnemyWhiteList.Count > 0)
                options = options.Where(x => Context.GameStyle.EnemyWhiteList.Contains(x)).ToList();
            EnemyOptions = new HashSet<Guid>();
            foreach (var option in options)
            {
                if (!Context.GameStyle.EnemyBlackList.Contains(option))
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

        public override void Update(TimeSpan passed)
        {
            var toRemove = new List<EnemyInstance>();
            foreach (var enemy in Context.CurrentEnemies.Enemies)
                if (enemy.ModuleInfo is T def)
                    if (UpdateEnemy(passed, enemy, def))
                        toRemove.Add(enemy);
            foreach (var enemy in toRemove)
                Context.CurrentEnemies.Remove(enemy);
        }
        public abstract HashSet<EnemyInstance> QueueEnemies(Guid id);
        public abstract List<EnemyInstance> UpdateSpawnQueue(TimeSpan passed, HashSet<EnemyInstance> queue);

        internal bool UpdateEnemy(TimeSpan passed, EnemyInstance enemy, T def)
        {
            if (MathHelpers.SqrDistance(enemy, Context.Map.Paths[enemy.PathID][enemy.WayPointID]) < 5 * Context.GameStyle.EnemySpeedMultiplier)
            {
                enemy.WayPointID++;
                if (enemy.WayPointID >= Context.Map.Paths[enemy.PathID].Count)
                {
                    Game.DamagePlayer();
                    return true;
                }
                enemy.Angle = MathHelpers.GetAngle(Context.Map.Paths[enemy.PathID][enemy.WayPointID], enemy);
            }
            if (def.SlowingDuration > 0)
                def.SlowingDuration -= passed.Milliseconds;
            var change = MathHelpers.GetPredictedLocation(enemy.Angle, def.GetSpeed(), Context.GameStyle.EnemySpeedMultiplier);
            enemy.X += change.X;
            enemy.Y += change.Y;
            return false;
        }

        public Guid? GetRandomEnemy(int wave)
        {
            var options = _enemyLevels.Keys.Where(x => x <= wave).ToList();
            if (options.Count == 0)
                options.Add(_enemyLevels.Keys.Min());
            var targetLevel = options[_rnd.Next(0, options.Count)];
            if (_enemyLevels[targetLevel].Count == 0)
                return null;
            var targetEnemy = _enemyLevels[targetLevel][_rnd.Next(0, _enemyLevels[targetLevel].Count)];
            return targetEnemy;
        }
    }
}
