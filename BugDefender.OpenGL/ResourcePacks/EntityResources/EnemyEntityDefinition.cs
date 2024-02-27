using System;

namespace TDGame.OpenGL.ResourcePacks.EntityResources
{
    public class EnemyEntityDefinition : IEntityResource
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
        public Guid OnDeath { get; set; }
    }
}
