using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class BorderControl : BaseChildContainer, IChildContainer
    {
        public string BorderColorName { get; set; } = "";
        public Texture2D BorderColor { get; set; } = BasicTextures.GetBasicRectange(Color.Black);
        public int BorderWidth { get; set; } = 1;
        public int Margin { get; set; } = 0;

        public BorderControl()
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            Child.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(BorderColor, new Rectangle(X, Y, Width, BorderWidth), Color.White);
            spriteBatch.Draw(BorderColor, new Rectangle(X, Y, BorderWidth, Height), Color.White);
            spriteBatch.Draw(BorderColor, new Rectangle(X + Width, Y, BorderWidth, Height), Color.White);
            spriteBatch.Draw(BorderColor, new Rectangle(X, Y + Height, Width + BorderWidth, BorderWidth), Color.White);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            if (Width != 0)
                Child.Width = Width - Margin * 2;
            if (Height != 0)
                Child.Height = Height - Margin * 2;
            Child.X = X + Margin;
            Child.Y = Y + Margin;
            Child.Refresh();
            X = Child.X - Margin;
            Y = Child.Y - Margin;
            if (Width == 0)
                Width = Child.Width + Margin * 2;
            if (Height == 0)
                Height = Child.Height + Margin * 2;
        }

        public override void LoadContent(ContentManager content)
        {
            if (BorderColorName != "")
                BorderColor = content.Load<Texture2D>(BorderColorName);

            Child.LoadContent(content);
        }
    }
}
