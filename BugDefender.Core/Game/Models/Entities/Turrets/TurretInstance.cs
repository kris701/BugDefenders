using BugDefender.Core.Game.Models.Entities.Enemies;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Resources;
using System.Text;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Turrets
{
    public class TurretInstance : BasePositionModel, IInstance<TurretDefinition>
    {
        public enum TargetingTypes { None, Closest, Weakest, Strongest }

        public Guid ID { get; set; }
        public Guid DefinitionID { get; set; }

        public ITurretModule TurretInfo { get; set; }

        [JsonIgnore]
        public EnemyInstance? Targeting { get; set; }
        public int Kills { get; set; } = 0;
        public List<Guid> HasUpgrades { get; set; }
        public TargetingTypes TargetingType { get; set; } = TargetingTypes.Closest;

        [JsonConstructor]
        public TurretInstance(Guid iD, Guid definitionID, ITurretModule turretInfo, int kills, List<Guid> hasUpgrades, TargetingTypes targetingType)
        {
            ID = iD;
            DefinitionID = definitionID;
            TurretInfo = turretInfo;
            Kills = kills;
            HasUpgrades = hasUpgrades;
            TargetingType = targetingType;
        }

        public TurretInstance(TurretDefinition definition)
        {
            ID = Guid.NewGuid();
            DefinitionID = definition.ID;
            TurretInfo = definition.ModuleInfo.Copy();
            Size = definition.Size;
            HasUpgrades = new List<Guid>();
        }

        private TurretDefinition? _definition;
        public TurretDefinition GetDefinition()
        {
            if (_definition != null)
                return _definition;
            _definition = ResourceManager.Turrets.GetResource(DefinitionID);
            return _definition;
        }

        public int GetTurretWorth(GameStyleDefinition style)
        {
            var def = GetDefinition();
            var worth = def.Cost;
            foreach (var upgrade in def.Upgrades)
                if (HasUpgrades.Contains(upgrade.ID))
                    worth += upgrade.Cost;
            return (int)(worth * style.TurretRefundPenalty);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(GetDefinition().ToString());
            sb.AppendLine(TurretInfo.ToString());
            if (Kills != 0)
            {
                sb.AppendLine(" ");
                sb.AppendLine($"Kills: {Kills}");
            }

            return sb.ToString();
        }
    }
}
