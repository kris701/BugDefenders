using BugDefender.Core.Game.Models.Entities.Enemies.Modules;
using BugDefender.Core.Resources;
using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Enemies
{
    public class EnemyInstance : BasePositionModel, IInstance<EnemyDefinition>
    {
        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }
        public float Health { get; set; }
        public IEnemyModule ModuleInfo { get; set; }
        public int PathID { get; set; } = 0;
        public int WayPointID { get; set; } = 0;

        public EnemyInstance(Guid iD, Guid definitionID, float health, IEnemyModule moduleInfo, int pathID, int wayPointID)
        {
            ID = iD;
            DefinitionID = definitionID;
            Health = health;
            ModuleInfo = moduleInfo;
            PathID = pathID;
            WayPointID = wayPointID;
        }

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

        private EnemyDefinition? _definition;
        public EnemyDefinition GetDefinition()
        {
            if (_definition != null)
                return _definition;
            _definition = ResourceManager.Enemies.GetResource(DefinitionID);
            return _definition;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(GetDefinition().ToString());
            sb.AppendLine(ModuleInfo.ToString());
            sb.AppendLine();
            sb.AppendLine($"Current Health: {Health}");

            return sb.ToString();
        }
    }
}
