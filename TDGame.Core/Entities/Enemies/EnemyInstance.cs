using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;

namespace TDGame.Core.Entities.Enemies
{
    public class EnemyInstance : BasePositionModel, IInstance<EnemyDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }
        public float Health { get; set; }

        public int WayPointID { get; set; } = 0;
        public Guid GroupID { get; set; }

        public EnemyInstance(Guid enemyDefinition, float evolution) : this(EnemyBuilder.GetEnemy(enemyDefinition), evolution)
        {
        }

        public EnemyInstance(EnemyDefinition definition, float evolution)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            Health = definition.Health * evolution;
            Size = definition.Size;
        }

        public EnemyDefinition GetDefinition() => EnemyBuilder.GetEnemy(DefinitionID);
    }
}
