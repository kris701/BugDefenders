using System.Text;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Game.Models.Entities.Upgrades
{
    public class PassiveTurretUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float RangeModifier { get; set; }
        public float RangeModifierModifier { get; set; }
        public float DamageModifierModifier { get; set; }
        public float CooldownModifierModifier { get; set; }
        public float SlowingFactorModifierModifier { get; set; }
        public float SlowingDurationModifierModifier { get; set; }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is PassiveTurretDefinition item)
            {
                item.Range *= RangeModifier;
                item.RangeModifier *= RangeModifierModifier;
                item.DamageModifier *= DamageModifierModifier;
                item.CooldownModifier = (int)(item.CooldownModifier * CooldownModifierModifier);
                item.SlowingFactorModifier *= SlowingFactorModifierModifier;
                item.SlowingDurationModifier = (int)(item.SlowingDurationModifier * SlowingDurationModifierModifier);
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
            if (DamageModifierModifier != 1)
                sb.AppendLine($"Damage Modifier {DamageModifierModifier}x");
            if (RangeModifierModifier != 1)
                sb.AppendLine($"Range Modifier {RangeModifierModifier}x");
            if (CooldownModifierModifier != 1)
                sb.AppendLine($"Cooldown Modifier {CooldownModifierModifier}x");
            if (SlowingFactorModifierModifier != 1)
                sb.AppendLine($"Slowing Factor Modifier {SlowingFactorModifierModifier}x");
            if (SlowingDurationModifierModifier != 1)
                sb.AppendLine($"Slowing Duration Modifier {SlowingDurationModifierModifier}x");

            return sb.ToString();
        }
    }
}
