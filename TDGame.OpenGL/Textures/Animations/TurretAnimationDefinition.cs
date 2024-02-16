using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Textures.Animations
{
    public class TurretAnimationDefinition : IAnimationDefinition
    {
        public Guid Target { get; set; }
        public Guid OnShoot { get; set; }
        public Guid OnIdle { get; set; }
    }
}
