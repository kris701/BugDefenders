using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Textures.Animations
{
    public class ProjectileAnimationDefinition : IAnimationDefinition
    {
        public Guid Target { get; set; }
        public Guid OnCreate { get; set; }
    }
}
