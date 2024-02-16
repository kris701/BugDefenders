using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models.Entities.Enemies;

namespace TDGame.Core.Modules.Enemies
{
    public interface IGameEnemyModule : IGameModule
    {
        public List<Guid> EnemyOptions { get; }
        public List<EnemyInstance> QueueEnemies(Guid id);
        public List<EnemyInstance> UpdateSpawnQueue(List<EnemyInstance> queue);
    }
}
