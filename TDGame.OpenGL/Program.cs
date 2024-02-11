using System;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Screens.MainMenu;
using TDGame.OpenGL.Screens.SplashScreen;

namespace TDGame.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameEngine((g) => new SplashScreen(g)))
                game.Run();
        }
    }
}
