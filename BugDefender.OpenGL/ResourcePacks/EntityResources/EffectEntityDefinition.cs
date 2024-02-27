using System;

namespace BugDefender.OpenGL.ResourcePacks.EntityResources
{
    public class EffectEntityDefinition : IEntityResource
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
