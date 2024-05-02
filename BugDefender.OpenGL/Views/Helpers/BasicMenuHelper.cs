using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.OpenGL.Formatter;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Fonts;
using MonoGame.OpenGL.Formatter.Views;
using System;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.Helpers
{
    public static class BasicMenuHelper
    {
        public static void GenerateBaseMenu(IView view, Texture2D background, string title, string subtitle, FontController fonts)
        {
            view.AddControl(0, new TileControl()
            {
                FillColor = background,
                Width = IWindow.BaseScreenSize.X,
                Height = IWindow.BaseScreenSize.Y
            });
            view.AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = title,
                FontColor = Color.White,
                Font = fonts.GetFont(FontSizes.Ptx48)
            });
            view.AddControl(0, new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = subtitle,
                Font = fonts.GetFont(FontSizes.Ptx16),
                FontColor = Color.White
            });
        }

        public static BugDefenderButtonControl GetAcceptButton(BugDefenderGameWindow parent, string text, ClickedHandler click)
        {
            return new BugDefenderButtonControl(parent, click)
            {
                Y = 980,
                X = 1670,
                Width = 200,
                Height = 50,
                Text = text,
                Font = parent.Fonts.GetFont(FontSizes.Ptx24),
                FontColor = Color.White,
                FillColor = parent.Textures.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = parent.Textures.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
            };
        }

        public static BugDefenderButtonControl GetCancelButton(BugDefenderGameWindow parent, string text, ClickedHandler click)
        {
            return new BugDefenderButtonControl(parent, click)
            {
                X = 50,
                Y = 980,
                Width = 200,
                Height = 50,
                Text = text,
                Font = parent.Fonts.GetFont(FontSizes.Ptx24),
                FontColor = Color.White,
                FillColor = parent.Textures.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = parent.Textures.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
            };
        }
    }
}
