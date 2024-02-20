using System.Text.Json.Serialization;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "BuffType")]
    [JsonDerivedType(typeof(EnemyBuffEffect), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(TurretBuffEffect), typeDiscriminator: "Turret")]
    public interface IBuffEffect
    {
        [JsonIgnore]
        public Guid BuffID { get; set; }
    }
}
