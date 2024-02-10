using System;
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

        public ProjectileLevel(string name, string description, int cost, double damageModifier, double splashRangeModifier, double triggerRangeModifier)
        {
            Name = name;
            Description = description;
            Cost = cost;
            DamageModifier = damageModifier;
            SplashRangeModifier = splashRangeModifier;
            TriggerRangeModifier = triggerRangeModifier;
        }

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
    }
}
