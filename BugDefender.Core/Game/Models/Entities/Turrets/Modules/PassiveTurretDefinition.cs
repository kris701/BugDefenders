﻿using BugDefender.Core.Game.Models.Entities.Upgrades;
using System.Text;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Turrets.Modules
{
    public class PassiveTurretDefinition : UpgradeEffectModel, ITurretModule, IRangeAttribute
    {
        public float Range { get; set; }
        [JsonIgnore]
        public HashSet<TurretInstance> Affected = new HashSet<TurretInstance>();

        public PassiveTurretDefinition(float range, List<EffectTarget> effects) : base(effects)
        {
            if (effects.Any(x => x.Value != null))
                throw new Exception("Passive turrets can only have modifiers!");
            Range = range;
        }

        public new ITurretModule Copy() => new PassiveTurretDefinition(Range, base.Copy().Effects);

        public string GetDescriptionString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Type: Passive");
            sb.AppendLine($"Range: {Range}");
            sb.AppendLine("Gives:");
            foreach (var effect in Effects)
            {
                if (effect.Value != null)
                    sb.AppendLine($"{effect.GetDisplayName()} {effect.Value}");
                else
                    sb.AppendLine($"{effect.GetDisplayName()} {effect.Modifier}x");
            }

            return sb.ToString();
        }
    }
}
