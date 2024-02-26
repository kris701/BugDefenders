using System.Text.Json.Serialization;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "BuffType")]
    [JsonDerivedType(typeof(EnemyBuffEffect), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(TurretBuffEffect), typeDiscriminator: "Turret")]
    [JsonDerivedType(typeof(ProjectileBuffEffect), typeDiscriminator: "Projectile")]
    public interface IBuffEffect
    {
    }
}
