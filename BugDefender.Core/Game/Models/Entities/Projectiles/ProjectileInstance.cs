using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Projectiles.Modules;
using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Projectiles
{
    [JsonSerializable(typeof(ProjectileInstance))]
    public class ProjectileInstance : BasePositionModel, IInstance<ProjectileDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }

        public IProjectileModule ProjectileInfo { get; set; }

        [JsonIgnore]
        public EnemyInstance? Target { get; set; }
        [JsonIgnore]
        public TurretInstance? Source { get; set; }

        [JsonConstructor]
        public ProjectileInstance(Guid iD, Guid definitionID, IProjectileModule projectileInfo)
        {
            ID = iD;
            DefinitionID = definitionID;
            ProjectileInfo = projectileInfo;
        }

        public ProjectileInstance(Guid definitionID, IProjectileModule module) : this(ResourceManager.Projectiles.GetResource(definitionID), module)
        {
        }

        public ProjectileInstance(ProjectileDefinition definition, IProjectileModule module)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            Size = definition.Size;
            ProjectileInfo = module.Copy();
        }

        private ProjectileDefinition? _definition;
        public ProjectileDefinition GetDefinition()
        {
            if (_definition != null)
                return _definition;
            _definition = ResourceManager.Projectiles.GetResource(DefinitionID);
            return _definition;
        }
    }
}
