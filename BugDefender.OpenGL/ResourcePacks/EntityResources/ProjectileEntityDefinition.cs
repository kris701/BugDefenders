using System;

namespace TDGame.OpenGL.ResourcePacks.EntityResources
{
    public class ProjectileEntityDefinition : IEntityResource
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
        public Guid OnDestroyed { get; set; }
    }
}
