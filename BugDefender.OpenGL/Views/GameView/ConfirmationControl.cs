using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using System;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.GameView
{
    public class ConfirmationControl : CollectionControl
    {
        public ConfirmationControl(BugDefenderGameWindow parent, ClickedHandler yesClick, ClickedHandler noClick)
        {
            Width = 400;
            Height = 200;
            HorizontalAlignment = HorizontalAlignment.Middle;
            VerticalAlignment = VerticalAlignment.Middle;
            Children.Add(new TileControl()
            {
                FillColor = parent.TextureController.GetTexture(new Guid("c20d95f4-517c-4fbd-aa25-115ea05539de")),
                Height = Height,
                Width = Width
            });
            Children.Add(new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Font = BasicFonts.GetFont(16),
                Text = "Are you sure?",
                Height = 40,
                Y = 20
            });
            Children.Add(new LabelControl()
            {
                HorizontalAlignment = HorizontalAlignment.Middle,
                Font = BasicFonts.GetFont(12),
                Text = "Your current game will be saved",
                Height = 30,
                Y = 60
            });
            Children.Add(new ButtonControl(parent, yesClick)
            {
                FillColor = parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Font = BasicFonts.GetFont(12),
                Text = "Yes",
                Height = 30,
                Width = 100,
                X = 50,
                Y = 120
            });
            Children.Add(new ButtonControl(parent, noClick)
            {
                FillColor = parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                Font = BasicFonts.GetFont(12),
                Text = "No",
                Height = 30,
                Width = 100,
                X = 250,
                Y = 120
            });
        }
    }
}
