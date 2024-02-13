using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Entities.Upgrades
{
    public class TurretLevel : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float RangeModifier { get; set; }
        public float DamageModifier { get; set; }
        public float CooldownModifier { get; set; }

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

            return sb.ToString();
        }
    }
}
