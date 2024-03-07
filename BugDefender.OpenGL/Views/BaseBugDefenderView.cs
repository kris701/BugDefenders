using BugDefender.OpenGL.Engine.Textures;
using BugDefender.OpenGL.Engine.Views;
using System;

namespace BugDefender.OpenGL.Views
{
    public class BaseBugDefenderView : BaseAnimatedView
    {
        public new BugDefenderGameWindow Parent { get; set; }
        public BaseBugDefenderView(BugDefenderGameWindow parent, Guid id, TextureSetDefinition inSet, TextureSetDefinition outSet) : base(parent, id, inSet, outSet)
        {
            Parent = parent;
        }
    }
}
