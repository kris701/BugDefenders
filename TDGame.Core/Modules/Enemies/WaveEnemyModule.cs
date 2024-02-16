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
    public class WaveEnemyModule : BaseEnemyModule<WaveEnemyDefinition>
    {
        public WaveEnemyModule(Game game) : base(game)
        {
        }

        public override void Update(TimeSpan passed)
        {
            var toRemove = new List<EnemyInstance>();
            foreach (var enemy in Game.CurrentEnemies)
                if (enemy.ModuleInfo is WaveEnemyDefinition def)
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
            var group = Guid.NewGuid();
            var template = ResourceManager.Enemies.GetResource(id);
            if (template.ModuleInfo is WaveEnemyDefinition def)
            {
                for (int j = 0; j < def.WaveSize * Game.GameStyle.EnemyWaveMultiplier; j++)
                {
                    var enemy = new EnemyInstance(template, Game.Evolution);
                    if (enemy.ModuleInfo is WaveEnemyDefinition def2)
                    {
                        enemy.X = Game.Map.WayPoints[0].X - enemy.Size / 2;
                        enemy.Y = Game.Map.WayPoints[0].Y - enemy.Size / 2;
                        def2.GroupID = group;
                        enemies.Add(enemy);
                    }
                }
            }
            return enemies;
        }

        public override List<EnemyInstance> UpdateSpawnQueue(List<EnemyInstance> queue)
        {
            if (queue.Count > 0)
            {
                var vistedGroups = new List<Guid>();
                var enemiesToAdd = new List<EnemyInstance>();
                foreach (var enemy in queue)
                {
                    if (EnemyOptions.Contains(enemy.DefinitionID) && enemy.ModuleInfo is WaveEnemyDefinition def)
                    {
                        if (vistedGroups.Contains(def.GroupID))
                            continue;
                        vistedGroups.Add(def.GroupID);
                        var minDist = double.MaxValue;
                        foreach (var current in Game.CurrentEnemies)
                            if (EnemyOptions.Contains(current.DefinitionID) && current.ModuleInfo is WaveEnemyDefinition def2)
                                if (def2.GroupID == def.GroupID)
                                    minDist = Math.Min(MathHelpers.Distance(enemy, current), minDist);
                        if (minDist > enemy.Size)
                            enemiesToAdd.Add(enemy);
                    }
                }
                return enemiesToAdd;
            }
            return new List<EnemyInstance>();
        }
    }
}
