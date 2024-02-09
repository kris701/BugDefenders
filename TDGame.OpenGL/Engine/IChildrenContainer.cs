using System.Collections.Generic;

namespace TDGame.OpenGL.Engine
{
    public interface IChildrenContainer : IControl
    {
        public List<IControl> Children { get; set; }
    }
}
