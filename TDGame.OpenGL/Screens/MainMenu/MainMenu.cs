using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL.Screens.MainMenu
{
    public partial class MainMenu : BaseScreen
    {
        public MainMenu(GameEngine parent) : base(parent)
        {
            ScaleValue = parent.Settings.Scale;
            Initialize();
        }
    }
}
