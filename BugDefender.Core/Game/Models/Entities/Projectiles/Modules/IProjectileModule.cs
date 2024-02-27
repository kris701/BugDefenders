using System.Text.Json.Serialization;

namespace TDGame.Core.Game.Models.Entities.Projectiles.Modules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ProjectileModule")]
    [JsonDerivedType(typeof(ExplosiveProjectileDefinition), typeDiscriminator: "Explosive")]
    [JsonDerivedType(typeof(DirectProjectileDefinition), typeDiscriminator: "Direct")]
    public interface IProjectileModule : IModuleInfo<IProjectileModule>
    {
    }
}
