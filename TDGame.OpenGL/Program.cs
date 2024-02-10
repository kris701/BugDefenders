using System;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Screens.MainMenu;

namespace TDGame.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
#if DEBUG
            using (var game = new GameEngine((g) => new MainMenu(g)))
            {
                game.Device.PreferredBackBufferWidth = 1000;
                game.Device.PreferredBackBufferHeight = 1000;
                game.Device.ApplyChanges();
                game.Run();
            }
#else
            using (var game = new PrimaryEngine((x) => new GenericSplashScreen(x)))
                game.Run();
#endif
        }
    }
}
