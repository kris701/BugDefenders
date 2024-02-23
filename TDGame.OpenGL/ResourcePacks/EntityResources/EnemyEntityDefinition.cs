using System;

namespace TDGame.OpenGL.Textures.Animations
{
    public class EnemyEntityDefinition : IEntityResource
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
