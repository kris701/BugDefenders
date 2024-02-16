using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Textures
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "TargetType")]
    [JsonDerivedType(typeof(TurretAnimationDefinition), typeDiscriminator: "Turret")]
    public interface IAnimationDefinition
    {
        public Guid Target { get; set; }
    }
}
