using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Views.Helpers
{
    public static class BasicMenuPage
    {
        public static void GenerateBaseMenu(IView view, Texture2D background, string title, string subtitle)
        {
            view.AddControl(0, new TileControl()
            {
                FillColor = background,
                Width = GameWindow.BaseScreenSize.X,
                Height = GameWindow.BaseScreenSize.Y
            });
            view.AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = title,
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            view.AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = subtitle,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });
        }
    }
}
