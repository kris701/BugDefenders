using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Modules;

namespace TDGame.Core.Game.Modules.Enemies
{
    public interface IGameEnemyModule : IGameModule
    {
        public List<Guid> EnemyOptions { get; }
        public List<EnemyInstance> QueueEnemies(Guid id);
        public List<EnemyInstance> UpdateSpawnQueue(List<EnemyInstance> queue);
    }
}
