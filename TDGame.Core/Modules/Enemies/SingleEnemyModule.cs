using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Helpers;
using TDGame.Core.Models.Entities.Enemies;
using TDGame.Core.Models.Entities.Enemies.Modules;
using TDGame.Core.Models.Maps;
using TDGame.Core.Resources;

namespace TDGame.Core.Modules.Enemies
{
    public class SingleEnemyModule : BaseEnemyModule<SingleEnemyDefinition>
    {
        public SingleEnemyModule(Game game) : base(game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            var toRemove = new List<EnemyInstance>();
            foreach (var enemy in Game.CurrentEnemies)
                if (enemy.ModuleInfo is SingleEnemyDefinition def)
                    if (UpdateEnemy(passed, enemy, def))
                        toRemove.Add(enemy);
            foreach (var enemy in toRemove)
                Game.CurrentEnemies.Remove(enemy);
        }
            
        public override List<EnemyInstance> QueueEnemies(Guid id)
        {
            if (!EnemyOptions.Contains(id))
                throw new Exception("Module attempted to queue enemies that does not match its module type!");

            var enemies = new List<EnemyInstance>();
            var template = ResourceManager.Enemies.GetResource(id);
            var enemy = new EnemyInstance(template, Game.Evolution);
            enemy.X = Game.Map.WayPoints[0].X - enemy.Size / 2;
            enemy.Y = Game.Map.WayPoints[0].Y - enemy.Size / 2;
            enemies.Add(enemy);
            return enemies;
        }

        public override List<EnemyInstance> UpdateSpawnQueue(List<EnemyInstance> queue)
        {
            if (queue.Count > 0)
            {
                var enemiesToAdd = new List<EnemyInstance>();
                foreach (var enemy in queue)
                    if (EnemyOptions.Contains(enemy.DefinitionID))
                        enemiesToAdd.Add(enemy);
                return enemiesToAdd;
            }
            return new List<EnemyInstance>();
        }
    }
}
