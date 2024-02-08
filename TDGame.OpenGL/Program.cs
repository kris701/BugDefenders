using MonoGame.FormEngine.Templates.Engines.PlainEngine;
using Project1.Helpers;
using Project1.Screens.MainMenu;
using System;

namespace Project1
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
#if DEBUG
            using (var game = new PlainEngine((x) => new MainMenu(x)))
            {
                game.Device.PreferredBackBufferWidth = 1000;
                game.Device.PreferredBackBufferHeight = 1000;
                game.Device.ApplyChanges();
                game.OnInitialize += () => { BasicFonts.Initialize(game.Content); };
                game.Run();
            }
#else
            using (var game = new PrimaryEngine((x) => new GenericSplashScreen(x)))
                game.Run();
#endif
        }
    }
}
