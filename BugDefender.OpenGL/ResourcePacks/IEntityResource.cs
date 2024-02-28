using BugDefender.OpenGL.ResourcePacks.EntityResources;
using System;
using System.Text.Json.Serialization;

namespace BugDefender.OpenGL.ResourcePacks
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "TargetType")]
    [JsonDerivedType(typeof(TurretEntityDefinition), typeDiscriminator: "Turret")]
    [JsonDerivedType(typeof(EffectEntityDefinition), typeDiscriminator: "Effect")]
    [JsonDerivedType(typeof(EnemyEntityDefinition), typeDiscriminator: "Enemy")]
    [JsonDerivedType(typeof(ProjectileEntityDefinition), typeDiscriminator: "Projectile")]
    public interface IEntityResource
    {
        public Guid Target { get; set; }
    }
}
