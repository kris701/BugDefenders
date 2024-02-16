using TDGame.Core.Models.Entities.Enemies.Modules;
using TDGame.Core.Resources;

namespace TDGame.Core.Models.Entities.Enemies
{
    public class EnemyInstance : BasePositionModel, IInstance<EnemyDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }
        public float Health { get; set; }
        public IEnemyModule ModuleInfo { get; set; }
        public int WayPointID { get; set; } = 0;

        public EnemyInstance(Guid enemyDefinition, float evolution) : this(ResourceManager.Enemies.GetResource(enemyDefinition), evolution)
        {
        }

        public EnemyInstance(EnemyDefinition definition, float evolution)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            ModuleInfo = definition.ModuleInfo.Copy();
            Health = definition.Health * evolution;
            Size = definition.Size;
        }

        public EnemyDefinition GetDefinition() => ResourceManager.Enemies.GetResource(DefinitionID);
    }
}
