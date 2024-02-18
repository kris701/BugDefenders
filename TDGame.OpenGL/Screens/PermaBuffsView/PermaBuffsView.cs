using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseScreen
    {
        public PermaBuffsView(UIEngine parent) : base(parent)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
        }
    }
}
