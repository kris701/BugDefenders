using System;

namespace TDGame.OpenGL.ResourcePacks.EntityResources
{
    public class TurretEntityDefinition : IEntityResource
    {
        public Guid Target { get; set; }
        public Guid OnShoot { get; set; }
        public Guid OnIdle { get; set; }
    }
}
