using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class ProjectileTurretUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float RangeModifier { get; set; }
        public float CooldownModifier { get; set; }

        public ProjectileTurretUpgrade(Guid iD, Guid? requires, string name, string description, int cost, float rangeModifier, float cooldownModifier)
        {
            ID = iD;
            Requires = requires;
            Name = name;
            Description = description;
            Cost = cost;
            RangeModifier = rangeModifier;
            CooldownModifier = cooldownModifier;
        }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is ProjectileTurretDefinition item)
            {
                item.Range *= RangeModifier;
                item.Cooldown = (int)(item.Cooldown * CooldownModifier);
                on.HasUpgrades.Add(ID);
            }
            else
                throw new Exception("Invalid upgrade type for turret!");
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Description);
            sb.AppendLine("Turret get:");
            if (RangeModifier != 1)
                sb.AppendLine($"Range {RangeModifier}x");
            if (CooldownModifier != 1)
                sb.AppendLine($"Cooldown {CooldownModifier}x");

            return sb.ToString();
        }
    }
}
