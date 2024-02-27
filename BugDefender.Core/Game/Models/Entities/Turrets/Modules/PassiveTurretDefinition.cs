using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Turrets.Modules
{
    public class PassiveTurretDefinition : ITurretModule, IRangeAttribute
    {
        public float Range { get; set; }
        public float RangeModifier { get; set; } = 1;
        public float DamageModifier { get; set; } = 1;
        public float CooldownModifier { get; set; } = 1;
        public float SlowingFactorModifier { get; set; } = 1;
        public int SlowingDurationModifier { get; set; } = 1;

        public ITurretModule Copy()
        {
            return new PassiveTurretDefinition()
            {
                Range = Range,
                RangeModifier = RangeModifier,
                DamageModifier = DamageModifier,
                CooldownModifier = CooldownModifier,
                SlowingFactorModifier = SlowingFactorModifier,
                SlowingDurationModifier = SlowingDurationModifier
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Passive");
            sb.AppendLine($"Range: {Range}");
            sb.AppendLine("Gives:");
            if (RangeModifier != 1)
                sb.AppendLine($"Range: {RangeModifier}x");
            if (DamageModifier != 1)
                sb.AppendLine($"Damage: {DamageModifier}x");
            if (CooldownModifier != 1)
                sb.AppendLine($"Cooldown: {CooldownModifier}x");
            if (SlowingFactorModifier != 1)
                sb.AppendLine($"Slowing Factor: {SlowingFactorModifier}x");
            if (SlowingDurationModifier != 1)
                sb.AppendLine($"Slowing Duration: {SlowingDurationModifier}x");

            return sb.ToString();
        }
    }
}
