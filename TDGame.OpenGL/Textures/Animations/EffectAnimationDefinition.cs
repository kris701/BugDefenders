using System;

namespace TDGame.OpenGL.Textures.Animations
{
    public class EffectAnimationDefinition : IAnimationDefinition
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
