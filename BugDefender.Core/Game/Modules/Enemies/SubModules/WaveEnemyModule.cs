using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Game.Modules.Enemies.SubModules
{
    public class WaveEnemyModule : BaseEnemyModule<WaveEnemyDefinition>
    {
        public WaveEnemyModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override List<EnemyInstance> QueueEnemies(Guid id)
        {
            if (!EnemyOptions.Contains(id))
                throw new Exception("Module attempted to queue enemies that does not match its module type!");

            var enemies = new List<EnemyInstance>();
            var template = ResourceManager.Enemies.GetResource(id);
            if (template.ModuleInfo is WaveEnemyDefinition def)
            {
                for (int j = 0; j < def.WaveSize * Context.GameStyle.EnemyWaveMultiplier; j++)
                {
                    var enemy = new EnemyInstance(template, Context.Evolution);
                    if (enemy.ModuleInfo is WaveEnemyDefinition def2)
                    {
                        enemy.PathID = _rnd.Next(0, Context.Map.Paths.Count);
                        enemy.X = Context.Map.Paths[enemy.PathID][0].X - enemy.Size / 2;
                        enemy.Y = Context.Map.Paths[enemy.PathID][0].Y - enemy.Size / 2;
                        def2.Group = new List<EnemyInstance>();
                        enemies.Add(enemy);
                    }
                }
            }
            foreach (var enemy in enemies)
                if (enemy.ModuleInfo is WaveEnemyDefinition def2)
                    def2.Group = enemies;
            return enemies;
        }

        public override List<EnemyInstance> UpdateSpawnQueue(TimeSpan passed, List<EnemyInstance> queue)
        {
            if (queue.Count > 0)
            {
                var enemiesToAdd = new List<EnemyInstance>();
                var vistedQueueItems = new HashSet<Guid>();
                foreach (var enemy in queue)
                {
                    if (EnemyOptions.Contains(enemy.DefinitionID) && enemy.ModuleInfo is WaveEnemyDefinition def)
                    {
                        if (vistedQueueItems.Contains(enemy.ID))
                            continue;
                        foreach (var item in def.Group)
                            vistedQueueItems.Add(item.ID);

                        def.SpawnDelay -= (float)passed.TotalMilliseconds;
                        if (def.SpawnDelay <= 0)
                            enemiesToAdd.Add(enemy);
                    }
                }
                return enemiesToAdd;
            }
            return new List<EnemyInstance>();
        }
    }
}
