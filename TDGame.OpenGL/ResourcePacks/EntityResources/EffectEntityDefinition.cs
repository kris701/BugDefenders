using System;

namespace TDGame.OpenGL.Textures.Animations
{
    public class EffectEntityDefinition : IEntityResource
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
