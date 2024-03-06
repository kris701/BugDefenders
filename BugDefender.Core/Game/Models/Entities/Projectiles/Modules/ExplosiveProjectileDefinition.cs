using BugDefender.Core.Game.Models.Entities.Turrets;
using BugDefender.Core.Resources;
using System.Text;

namespace BugDefender.Core.Game.Models.Entities.Projectiles.Modules
{
    public class ExplosiveProjectileDefinition : IProjectileModule, ISpeedAttribute, ISlowingAttribute
    {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float SplashRange { get; set; }
        public float TriggerRange { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        public ExplosiveProjectileDefinition(float speed, float damage, float splashRange, float triggerRange, double acceleration, bool isGuided, float slowingFactor, int slowingDuration, List<DamageModifier> damageModifiers)
        {
            Speed = speed;
            Damage = damage;
            SplashRange = splashRange;
            TriggerRange = triggerRange;
            Acceleration = acceleration;
            IsGuided = isGuided;
            SlowingFactor = slowingFactor;
            SlowingDuration = slowingDuration;
            DamageModifiers = damageModifiers;
        }

        public IProjectileModule Copy() => new ExplosiveProjectileDefinition(Speed, Damage, SplashRange, TriggerRange, Acceleration, IsGuided, SlowingFactor, SlowingDuration, DamageModifiers);

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Speed != 0)
                sb.AppendLine($"Speed: {Speed}");
            if (Damage != 0)
                sb.AppendLine($"Damage: {Damage}");
            if (SplashRange != 0)
                sb.AppendLine($"Splash Range: {SplashRange}");
            if (TriggerRange != 0)
                sb.AppendLine($"Trigger Range: {TriggerRange}");
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
