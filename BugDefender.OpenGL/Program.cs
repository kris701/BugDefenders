using System;
using BugDefender.OpenGL.Screens.SplashScreen;

namespace BugDefender.OpenGL
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
