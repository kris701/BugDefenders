using System;
using TDGame.OpenGL.Screens.SplashScreen;

namespace TDGame.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new UIEngine((g) => new SplashScreenView(g)))
                game.Run();
        }
    }
}
