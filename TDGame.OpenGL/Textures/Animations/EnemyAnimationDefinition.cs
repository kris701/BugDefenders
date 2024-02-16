using System;

namespace TDGame.OpenGL.Textures.Animations
{
    public class EnemyAnimationDefinition : IAnimationDefinition
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
