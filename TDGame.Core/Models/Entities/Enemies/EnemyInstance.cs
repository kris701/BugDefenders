using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;
using TDGame.Core.Resources;

namespace TDGame.Core.Models.Entities.Enemies
{
    public class EnemyInstance : BasePositionModel, IInstance<EnemyDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }
        public float Health { get; set; }

        public float SlowingFactor { get; set; } = 1;
        public int SlowingDuration { get; set; }
        public int WayPointID { get; set; } = 0;
        public Guid GroupID { get; set; }

        public EnemyInstance(Guid enemyDefinition, float evolution) : this(ResourceManager.Enemies.GetResource(enemyDefinition), evolution)
        {
        }

        public EnemyInstance(EnemyDefinition definition, float evolution)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            Health = definition.Health * evolution;
            Size = definition.Size;
        }

        public EnemyDefinition GetDefinition() => ResourceManager.Enemies.GetResource(DefinitionID);

        public float GetSpeed()
        {
            var def = GetDefinition();
            if (SlowingFactor == 1)
                return def.Speed;
            if (SlowingDuration <= 0)
                SlowingFactor = 1;
            return def.Speed * SlowingFactor;
        }
    }
}
