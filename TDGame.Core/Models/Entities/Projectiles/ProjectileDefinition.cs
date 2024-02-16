using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Models;
using TDGame.Core.Models.Entities.Turrets;
using TDGame.Core.Resources;
using static TDGame.Core.Models.Entities.Enemies.EnemyDefinition;

namespace TDGame.Core.Models.Entities.Projectiles
{
    public class ProjectileDefinition : IDefinition
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Size { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float SplashRange { get; set; }
        public float TriggerRange { get; set; }
        public double Acceleration { get; set; }
        public bool IsGuided { get; set; }
        public bool IsExplosive { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        public ProjectileDefinition Copy()
        {
            return new ProjectileDefinition() {
                ID = ID,
                Name = Name,
                Description = Description,
                Size = Size,
                Speed = Speed,
                Damage = Damage,
                SplashRange = SplashRange,
                TriggerRange = TriggerRange,
                Acceleration = Acceleration,
                IsGuided = IsGuided,
                IsExplosive = IsExplosive,
                SlowingFactor = SlowingFactor,
                SlowingDuration = SlowingDuration,
                DamageModifiers = DamageModifiers
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {Name}");
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
