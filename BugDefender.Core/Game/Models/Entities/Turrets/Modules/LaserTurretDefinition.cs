using BugDefender.Core.Resources;
using System.Text;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Turrets.Modules
{
    [JsonSerializable(typeof(LaserTurretDefinition))]
    public class LaserTurretDefinition : ITurretModule, IRangeAttribute, ISlowingAttribute
    {
        public float Range { get; set; }
        public float Damage { get; set; }
        public int Cooldown { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; } = TimeSpan.Zero;

        public LaserTurretDefinition(float range, float damage, int cooldown, float slowingFactor, int slowingDuration, List<DamageModifier> damageModifiers)
        {
            Range = range;
            Damage = damage;
            Cooldown = cooldown;
            SlowingFactor = slowingFactor;
            SlowingDuration = slowingDuration;
            DamageModifiers = damageModifiers;
        }

        public ITurretModule Copy() => new LaserTurretDefinition(Range, Damage, Cooldown, SlowingFactor, SlowingDuration, DamageModifiers);

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Laser");
            if (Range != 0)
                sb.AppendLine($"Range: {Range}");
            if (Damage != 0)
                sb.AppendLine($"Damage: {Damage}");
            sb.AppendLine($"Cooldown: {Cooldown}");
            if (SlowingFactor != 1)
                sb.AppendLine($"Slowing Factor: {SlowingFactor}");
            if (SlowingDuration != 0)
                sb.AppendLine($"Slowing Duration: {SlowingDuration}");
            sb.AppendLine();
            if (DamageModifiers.Count > 0)
            {
                sb.AppendLine("Damage Modifiers:");
                foreach (var modifier in DamageModifiers)
                    sb.Append($"{ResourceManager.EnemyTypes.GetResource(modifier.EnemyType).Name}: {modifier.Modifier}x, ");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
