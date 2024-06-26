﻿using BugDefender.Core.Game.Models.Entities.Enemies;

namespace BugDefender.Core.Game.Modules.Enemies.SubModules
{
    public interface IGameEnemyModule : IGameModule
    {
        public HashSet<Guid> EnemyOptions { get; }
        public List<EnemyInstance> QueueEnemies(Guid id);
        public List<EnemyInstance> UpdateSpawnQueue(TimeSpan passed, List<EnemyInstance> queue);
        public Guid? GetRandomEnemy(int wave);
    }
}
