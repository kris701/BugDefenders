using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;
using System.Text;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Projectiles.Modules
{
    public class DirectProjectileDefinition : IProjectileModule, ISpeedAttribute, ISlowingAttribute
    {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        public DirectProjectileDefinition(float speed, float damage, double acceleration, bool isGuided, float slowingFactor, int slowingDuration, List<DamageModifier> damageModifiers)
        {
            Speed = speed;
            Damage = damage;
            Acceleration = acceleration;
            IsGuided = isGuided;
            SlowingFactor = slowingFactor;
            SlowingDuration = slowingDuration;
            DamageModifiers = damageModifiers;
        }

        public IProjectileModule Copy() => new DirectProjectileDefinition(Speed, Damage, Acceleration, IsGuided, SlowingFactor, SlowingDuration, DamageModifiers);

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Speed != 0)
                sb.AppendLine($"Speed: {Speed}");
            if (Damage != 0)
                sb.AppendLine($"Damage: {Damage}");
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
