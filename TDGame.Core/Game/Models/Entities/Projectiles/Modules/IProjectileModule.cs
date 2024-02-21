using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.Core.Game.Models.Entities.Projectiles.Modules
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ProjectileModule")]
    [JsonDerivedType(typeof(ExplosiveProjectileDefinition), typeDiscriminator: "Explosive")]
    [JsonDerivedType(typeof(DirectProjectileDefinition), typeDiscriminator: "Direct")]
    public interface IProjectileModule : IModuleInfo<IProjectileModule>
    {
    }
}
