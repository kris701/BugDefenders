using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Controls;
using BugDefender.OpenGL.Views;
using System;

namespace BugDefender.OpenGL.Screens.SplashScreen
{
    public partial class SplashScreenView : BaseBugDefenderView
    {
        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                FillColor = Parent.TextureController.GetTexture(new Guid("a23f20ed-299a-4f94-b398-3dd00ff63bd5")),
                Width = IWindow.BaseScreenSize.X,
                Height = IWindow.BaseScreenSize.Y
            });
            base.Initialize();
        }
    }
}
