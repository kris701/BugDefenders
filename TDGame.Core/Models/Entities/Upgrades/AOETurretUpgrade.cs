using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.Core.Models.Entities.Turrets;

namespace TDGame.Core.Models.Entities.Upgrades
{
    public class AOETurretUpgrade : IUpgrade
    {
        public Guid ID { get; set; }
        public Guid? Requires { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public float RangeModifier { get; set; }
        public float DamageModifier { get; set; }
        public float CooldownModifier { get; set; }
        public float SlowingFactorModifier { get; set; }
        public float SlowingDurationModifier { get; set; }

        public void ApplyUpgrade(TurretInstance on)
        {
            if (on.TurretInfo is AOETurretDefinition item)
            {
                item.Range *= RangeModifier;
                item.Damage *= DamageModifier;
                item.Cooldown = (int)(item.Cooldown * CooldownModifier);
                on.HasUpgrades.Add(ID);
            }
            else
                throw new Exception("Invalid upgrade type for turret!");
        }

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
