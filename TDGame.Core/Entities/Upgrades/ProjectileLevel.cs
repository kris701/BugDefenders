using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Entities.Turrets.Upgrades
{
    public class ProjectileLevel : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float DamageModifier { get; set; }
        public float SplashRangeModifier { get; set; }
        public float TriggerRangeModifier { get; set; }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Description);
            sb.AppendLine("Projectile get:");
            if (DamageModifier != 1)
                sb.AppendLine($"Damage {DamageModifier}x");
            if (SplashRangeModifier != 1)
                sb.AppendLine($"Splash Range {SplashRangeModifier}x");
            if (TriggerRangeModifier != 1)
                sb.AppendLine($"Trigger Range {TriggerRangeModifier}x");

            return sb.ToString();
        }
    }
}
