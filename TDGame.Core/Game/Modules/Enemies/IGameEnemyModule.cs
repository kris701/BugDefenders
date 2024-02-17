using TDGame.Core.Game.Models.Entities.Enemies;

namespace TDGame.Core.Game.Modules.Enemies
{
    public interface IGameEnemyModule : IGameModule
    {
        public List<Guid> EnemyOptions { get; }
        public List<EnemyInstance> QueueEnemies(Guid id);
        public List<EnemyInstance> UpdateSpawnQueue(List<EnemyInstance> queue);
    }
}
