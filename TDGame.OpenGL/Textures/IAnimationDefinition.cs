using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TDGame.OpenGL.Textures.Animations;

namespace TDGame.OpenGL.Textures
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "TargetType")]
    [JsonDerivedType(typeof(TurretAnimationDefinition), typeDiscriminator: "Turret")]
    [JsonDerivedType(typeof(EffectAnimationDefinition), typeDiscriminator: "Effect")]
    [JsonDerivedType(typeof(EnemyAnimationDefinition), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(ProjectileAnimationDefinition), typeDiscriminator: "Projectile")]
    public interface IAnimationDefinition
    {
        public Guid Target { get; set; }
    }
}
