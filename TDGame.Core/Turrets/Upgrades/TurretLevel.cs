using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Turrets.Upgrades
{
    public class TurretLevel : IUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public double RangeModifier { get; set; }
        public double DamageModifier { get; set; }
        public double CooldownModifier { get; set; }

        [JsonIgnore]
        public bool HasUpgrade { get; set; }

        public TurretLevel(string name, string description, int cost, double rangeModifier, double damageModifier, double cooldownModifier)
        {
            Name = name;
            Description = description;
            Cost = cost;
            RangeModifier = rangeModifier;
            DamageModifier = damageModifier;
            CooldownModifier = cooldownModifier;
        }

        public void ApplyUpgrade(TurretDefinition on)
        {
            on.Range = (int)(on.Range * RangeModifier);
            on.Damage = (int)(on.Damage * DamageModifier);
            on.Cooldown = (int)(on.Cooldown * CooldownModifier);
            HasUpgrade = true;
        }
    }
}
