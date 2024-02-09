using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class StackPanelControl : BaseChildrenContainer
    {
        public string FillColorName { get; set; } = "";
        public Texture2D FillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public int Margin { get; set; } = 0;

        public StackPanelControl()
        {
        }

        public override void Initialize()
        {
            foreach (var child in Children)
            {
                child.UIChanged += () =>
                {
                    Height = 0;
                    UpdateUI();
                };
                child.Initialize();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(FillColor, new Rectangle(X, Y, Width, Height), Color.White);
            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent(ContentManager content)
        {
            if (FillColorName != "")
                FillColor = content.Load<Texture2D>(FillColorName);

            foreach (var child in Children)
                child.LoadContent(content);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            int yOffset = Y + Margin;
            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    child.X = X + Margin;
                    child.Y = yOffset;
                    child.Width = Width - 2 * Margin;
                    child.Refresh();
                    if (child.Height == 0)
                        child.Height = 10;
                    yOffset += child.Height + Margin;
                }
            }
            if (Height == 0)
                Height = yOffset - Y;
        }
    }
}
