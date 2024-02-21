using System.Text.Json.Serialization;
using TDGame.Core.Game.Models.Entities.Enemies;
using TDGame.Core.Game.Models.Entities.Projectiles.Modules;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Resources;

namespace TDGame.Core.Game.Models.Entities.Projectiles
{
    public class ProjectileInstance : BasePositionModel, IInstance<ProjectileDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }

        public IProjectileModule ProjectileInfo { get; set; }

        public EnemyInstance? Target { get; set; }
        public TurretInstance? Source { get; set; }

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

        public ProjectileDefinition GetDefinition() => ResourceManager.Projectiles.GetResource(DefinitionID);
    }
}
