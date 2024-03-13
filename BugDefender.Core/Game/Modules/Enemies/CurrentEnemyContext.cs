using BugDefender.Core.Game.Models.Entities.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static BugDefender.Core.Game.Models.Entities.Enemies.EnemyDefinition;

namespace BugDefender.Core.Game.Modules.Enemies
{
    public class CurrentEnemyContext
    {
        public Dictionary<EnemyTerrrainTypes, HashSet<EnemyInstance>> EnemiesByTerrain { get; } = new Dictionary<EnemyTerrrainTypes, HashSet<EnemyInstance>>();
        public HashSet<EnemyInstance> Enemies { get; } = new HashSet<EnemyInstance>();

        public CurrentEnemyContext()
        {
            foreach (var option in Enum.GetValues(typeof(EnemyTerrrainTypes)))
                if (option is EnemyTerrrainTypes op && !EnemiesByTerrain.ContainsKey(op))
                    EnemiesByTerrain.Add(op, new HashSet<EnemyInstance>());
        }

        [JsonConstructor]
        public CurrentEnemyContext(Dictionary<EnemyTerrrainTypes, HashSet<EnemyInstance>> enemiesByTerrain, HashSet<EnemyInstance> enemies)
        {
            EnemiesByTerrain = enemiesByTerrain;
            Enemies = enemies;
            foreach (var option in Enum.GetValues(typeof(EnemyTerrrainTypes)))
                if (option is EnemyTerrrainTypes op && !EnemiesByTerrain.ContainsKey(op))
                    EnemiesByTerrain.Add(op, new HashSet<EnemyInstance>());
        }

        public void AddEnemy(EnemyInstance enemy)
        {
            var enemyDef = enemy.GetDefinition();
            EnemiesByTerrain[enemyDef.TerrainType].Add(enemy);
            Enemies.Add(enemy);
        }

        public void RemoveEnemy(EnemyInstance enemy)
        {
            var enemyDef = enemy.GetDefinition();
            EnemiesByTerrain[enemyDef.TerrainType].Remove(enemy);
            Enemies.Remove(enemy);
        }

        public int Count => Enemies.Count;
    }
}
