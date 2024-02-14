﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Entities.Enemies;
using TDGame.Core.Entities.Turrets;
using TDGame.Core.Models;
using TDGame.Core.Resources;

namespace TDGame.Core.Entities.Projectiles
{
    public class ProjectileInstance : BasePositionModel, IInstance<ProjectileDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }
        public float Traveled { get; set; }
        public EnemyInstance? Target { get; set; }
        public TurretInstance? Source { get; set; }
        public List<Guid> HasUpgrades { get; set; }

        public ProjectileInstance(Guid definitionID) : this(ResourceManager.Projectiles.GetResource(definitionID))
        {
        }

        public ProjectileInstance(ProjectileDefinition definition)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            Size = definition.Size;
            HasUpgrades = new List<Guid>();
        }

        public ProjectileDefinition GetDefinition() => ResourceManager.Projectiles.GetResource(DefinitionID);
    }
}
