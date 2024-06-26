﻿using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using static BugDefender.Core.Game.Models.Entities.Enemies.EnemyDefinition;

namespace BugDefender.Core.Game.Models.Entities.Projectiles
{
    public class ProjectileDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public IProjectileModule ModuleInfo { get; set; }
        public HashSet<EnemyTerrrainTypes> CanTarget { get; set; }

        public ProjectileDefinition(Guid iD, string name, string description, float size, IProjectileModule moduleInfo, HashSet<EnemyTerrrainTypes> canTarget)
        {
            ID = iD;
            Name = name;
            Description = description;
            Size = size;
            ModuleInfo = moduleInfo;
            CanTarget = canTarget;
        }
    }
}
