using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Projectiles.Modules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ProjectileModule")]
    [JsonDerivedType(typeof(ExplosiveProjectileDefinition), typeDiscriminator: "Explosive")]
    [JsonDerivedType(typeof(DirectProjectileDefinition), typeDiscriminator: "Direct")]
    [JsonDerivedType(typeof(FireProjectileDefinition), typeDiscriminator: "Fire")]
    public interface IProjectileModule : IModuleInfo<IProjectileModule>
    {
    }
}
