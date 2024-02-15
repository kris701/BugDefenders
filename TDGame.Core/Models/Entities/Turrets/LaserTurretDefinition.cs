using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.Core.Resources;

namespace TDGame.Core.Models.Entities.Turrets
{
    public class LaserTurretDefinition : ITurretType
    {
        public float Range { get; set; }
        public float Damage { get; set; }
        public int Cooldown { get; set; }
        public float SlowingFactor { get; set; }
        public int SlowingDuration { get; set; }
        public List<DamageModifier> DamageModifiers { get; set; }

        [JsonIgnore]
        public TimeSpan CoolingFor { get; set; }

        public ITurretType Copy()
        {
            return new LaserTurretDefinition()
            {
                Range = Range,
                Damage = Damage,
                Cooldown = Cooldown,
                SlowingFactor = SlowingFactor,
                SlowingDuration = SlowingDuration,
                DamageModifiers = DamageModifiers
            };
        }

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Laser");
            if (Range != 0)
                sb.AppendLine($"Range: {Range}");
            if (Damage != 0)
                sb.AppendLine($"Damage: {Damage}");
            sb.AppendLine($"Cooldown: {Cooldown}");
            if (SlowingFactor != 1)
                sb.AppendLine($"Slowing Factor: {SlowingFactor}");
            if (SlowingDuration != 0)
                sb.AppendLine($"Slowing Duration: {SlowingDuration}");
            sb.AppendLine();
            foreach (var modifier in DamageModifiers)
                sb.AppendLine($"{ResourceManager.EnemyTypes.GetResource(modifier.EnemyType).Name}: {modifier.Modifier}x");

            return sb.ToString();
        }
    }
}
