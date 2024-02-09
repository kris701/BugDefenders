using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Turrets
{
    public class TurretLevel
    {
        public string Name { get; set; }
        public int RequiresTurretLevel { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public double RangeModifier { get; set; }
        public double DamageModifier { get; set; }

        public TurretLevel(string name, int requiresTurretLevel, string description, int cost, double rangeModifier, double damageModifier)
        {
            Name = name;
            RequiresTurretLevel = requiresTurretLevel;
            Description = description;
            Cost = cost;
            RangeModifier = rangeModifier;
            DamageModifier = damageModifier;
        }
    }
}
