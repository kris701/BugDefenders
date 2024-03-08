using BugDefender.OpenGL.Engine.Views;
using System;

namespace BugDefender.OpenGL.Views
{
    public class BaseBugDefenderView : BaseAnimatedView
    {
        public new BugDefenderGameWindow Parent { get; set; }
        public BaseBugDefenderView(BugDefenderGameWindow parent, Guid id) : base(
            parent,
            id,
            parent.TextureController.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.TextureController.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            Parent = parent;
        }
    }
}
