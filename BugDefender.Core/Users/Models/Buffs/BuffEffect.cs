using BugDefender.Core.Game.Models.Entities.Upgrades;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Users.Models.Buffs
{
    [JsonConverter(typeof(JsonStringEnumConverter<BuffEffectTypes>))]
    public enum BuffEffectTypes { Enemy, Projectile, Turret }
    [JsonSerializable(typeof(BuffEffect))]
    public class BuffEffect : UpgradeEffectModel
    {
        public BuffEffectTypes TargetType { get; set; }
        public Guid Target { get; set; }

        public BuffEffect(BuffEffectTypes targetType, Guid target, List<EffectTarget> effects) : base(effects)
        {
            TargetType = targetType;
            Target = target;
        }
    }
}
