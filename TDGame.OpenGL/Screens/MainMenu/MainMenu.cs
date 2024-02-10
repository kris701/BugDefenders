using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public MainMenu(GameEngine parent) : base(parent)
        {
            ScaleValue = parent.Scale;
            Initialize();
        }
    }
}
