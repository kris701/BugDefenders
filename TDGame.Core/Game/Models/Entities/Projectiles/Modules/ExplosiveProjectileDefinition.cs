using System.Text;
using TDGame.Core.Game.Models.Entities.Turrets;
using TDGame.Core.Resources;

namespace TDGame.Core.Game.Models.Entities.Projectiles.Modules
{
    public class ExplosiveProjectileDefinition : IProjectileModule, ISpeedAttribute, ISlowingAttribute, IDamageAttribute
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

        public IProjectileModule Copy()
        {
            return new ExplosiveProjectileDefinition()
            {
                Speed = Speed,
                Damage = Damage,
                SplashRange = SplashRange,
                TriggerRange = TriggerRange,
                Acceleration = Acceleration,
                IsGuided = IsGuided,
                SlowingFactor = SlowingFactor,
                SlowingDuration = SlowingDuration,
                DamageModifiers = DamageModifiers
            };
        }

        public string GetDescriptionString()
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
