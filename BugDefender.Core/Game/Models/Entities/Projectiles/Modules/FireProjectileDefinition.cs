using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;
using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Projectiles.Modules
{
    public class FireProjectileDefinition : IProjectileModule, ISpeedAttribute, ISlowingAttribute
    {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float DamageRange { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public int LifeTimeMs { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        public FireProjectileDefinition(float speed, float damage, float damageRange, double acceleration, bool isGuided, float slowingFactor, int slowingDuration, int lifetimeMs, List<DamageModifier> damageModifiers)
        {
            Speed = speed;
            Damage = damage;
            DamageRange = damageRange;
            Acceleration = acceleration;
            IsGuided = isGuided;
            SlowingFactor = slowingFactor;
            SlowingDuration = slowingDuration;
            LifeTimeMs = lifetimeMs;
            DamageModifiers = damageModifiers;
        }

        public IProjectileModule Copy() => new FireProjectileDefinition(Speed, Damage, DamageRange, Acceleration, IsGuided, SlowingFactor, SlowingDuration, LifeTimeMs, DamageModifiers);

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Speed != 0)
                sb.AppendLine($"Speed: {Speed}");
            if (LifeTimeMs != 0)
                sb.AppendLine($"Lifetime: {LifeTimeMs}");
            if (Damage != 0)
                sb.AppendLine($"Damage: {Damage}");
            if (DamageRange != 0)
                sb.AppendLine($"Damage Range: {DamageRange}");
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
