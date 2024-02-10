using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Screens;

namespace Project1.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public MainMenu(TDGame.OpenGL.TDGame parent) : base(parent)
        {
            ScaleValue = parent.Scale;
            Initialize();
        }
    }
}
