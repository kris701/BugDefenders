﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Turrets.Upgrades
{
    public class ProjectileLevel : IUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public double DamageModifier { get; set; }
        public double SplashRangeModifier { get; set; }
        public double TriggerRangeModifier { get; set; }

        [JsonIgnore]
        public bool HasUpgrade { get; set; }

        public void ApplyUpgrade(ProjectileDefinition on)
        {
            on.Damage = (int)(on.Damage * DamageModifier);
            on.SplashRange = (int)(on.SplashRange * SplashRangeModifier);
            on.TriggerRange = (int)(on.TriggerRange * TriggerRangeModifier);
            HasUpgrade = true;
        }

        public void ApplyUpgrade(TurretDefinition on)
        {
            HasUpgrade = true;
        }

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
