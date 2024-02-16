using System;

namespace TDGame.OpenGL.Textures.Animations
{
    public class ProjectileAnimationDefinition : IAnimationDefinition
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
