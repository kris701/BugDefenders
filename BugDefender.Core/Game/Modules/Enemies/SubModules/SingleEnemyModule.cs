﻿using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Game.Modules.Enemies.SubModules
{
    public class SingleEnemyModule : BaseEnemyModule<SingleEnemyDefinition>
    {
        public SingleEnemyModule(GameContext context, GameEngine game) : base(context, game)
        {
        }

        public override HashSet<EnemyInstance> QueueEnemies(Guid id)
        {
            if (!EnemyOptions.Contains(id))
                throw new Exception("Module attempted to queue enemies that does not match its module type!");

            var enemies = new HashSet<EnemyInstance>();
            var template = ResourceManager.Enemies.GetResource(id);
            var enemy = new EnemyInstance(template, Context.Evolution)
            {
                PathID = _rnd.Next(0, Context.Map.Paths.Count)
            };
            enemy.X = Context.Map.Paths[enemy.PathID][0].X - enemy.Size / 2;
            enemy.Y = Context.Map.Paths[enemy.PathID][0].Y - enemy.Size / 2;
            enemies.Add(enemy);
            return enemies;
        }

        public override List<EnemyInstance> UpdateSpawnQueue(TimeSpan passed, HashSet<EnemyInstance> queue)
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
