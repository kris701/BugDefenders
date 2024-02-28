using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class LaserTurretUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float RangeModifier { get; set; }
        public float DamageModifier { get; set; }
        public float CooldownModifier { get; set; }
        public float SlowingFactorModifier { get; set; }
        public float SlowingDurationModifier { get; set; }

        public LaserTurretUpgrade(Guid iD, Guid? requires, string name, string description, int cost, float rangeModifier, float damageModifier, float cooldownModifier, float slowingFactorModifier, float slowingDurationModifier)
        {
            ID = iD;
            Requires = requires;
            Name = name;
            Description = description;
            Cost = cost;
            RangeModifier = rangeModifier;
            DamageModifier = damageModifier;
            CooldownModifier = cooldownModifier;
            SlowingFactorModifier = slowingFactorModifier;
            SlowingDurationModifier = slowingDurationModifier;
        }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is LaserTurretDefinition item)
            {
                item.Range *= RangeModifier;
                item.Damage *= DamageModifier;
                item.Cooldown = (int)(item.Cooldown * CooldownModifier);
                item.SlowingFactor *= SlowingFactorModifier;
                item.SlowingDuration = (int)(item.SlowingDuration * SlowingDurationModifier);
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
            if (DamageModifier != 1)
                sb.AppendLine($"Damage {DamageModifier}x");
            if (RangeModifier != 1)
                sb.AppendLine($"Range {RangeModifier}x");
            if (CooldownModifier != 1)
                sb.AppendLine($"Cooldown {CooldownModifier}x");
            if (SlowingFactorModifier != 1)
                sb.AppendLine($"Slowing Factor: {SlowingFactorModifier}x");
            if (SlowingDurationModifier != 1)
                sb.AppendLine($"Slowing Duration: {SlowingDurationModifier}x");

            return sb.ToString();
        }
    }
}
