using System.Text;
using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Resources;

namespace BugDefender.Core.Game.Models.Entities.Turrets
{
    public class TurretInstance : BasePositionModel, IInstance<TurretDefinition>
    {
        public enum TargetingTypes { None, Closest, Weakest, Strongest }

        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }

        public ITurretModule TurretInfo { get; set; }

        public EnemyInstance? Targeting { get; set; }
        public int Kills { get; set; } = 0;
        public List<Guid> HasUpgrades { get; set; }
        public TargetingTypes TargetingType { get; set; } = TargetingTypes.Closest;

        public TurretInstance(Guid definitionID) : this(ResourceManager.Turrets.GetResource(definitionID))
        {
        }

        public TurretInstance(TurretDefinition definition)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            TurretInfo = definition.ModuleInfo.Copy();
            Size = definition.Size;
            HasUpgrades = new List<Guid>();
        }

        public TurretDefinition GetDefinition() => ResourceManager.Turrets.GetResource(DefinitionID);

        public int GetTurretWorth()
        {
            var def = GetDefinition();
            var worth = def.Cost;
            foreach (var upgrade in def.Upgrades)
                if (HasUpgrades.Contains(upgrade.ID))
                    worth += upgrade.Cost;
            return worth;
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(GetDefinition().GetDescriptionString());
            sb.AppendLine(TurretInfo.GetDescriptionString());
            if (Kills != 0)
            {
                sb.AppendLine(" ");
                sb.AppendLine($"Kills: {Kills}");
            }

            return sb.ToString();
        }
    }
}
