using BugDefender.OpenGL.Screens.SplashScreen;
using System;

namespace BugDefender.OpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using (var mainWindow = new GameWindow((g) => new SplashScreenView(g)))
                    mainWindow.Run();
            }
            catch (Exception e)
            {
                using (var crashWindow = new CrashWindow(e))
                    crashWindow.Run();
            }
        }
    }
}
