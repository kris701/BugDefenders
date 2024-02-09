using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class LabelControl : BaseControl
    {
        public string Text { get; set; } = "";
        public Color FontColor { get; set; } = Color.Black;
        public SpriteFont Font { get; set; } = BasicFonts.GetFont(8);

        private int _textX = 0;
        private int _textY = 0;

        public LabelControl()
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            if (Text != "")
                spriteBatch.DrawString(Font, Text, new Vector2(_textX, _textY), FontColor);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            if (Text != "")
            {
                var size = Font.MeasureString(Text);
                if (Width == 0)
                    Width = (int)size.X;
                if (Height == 0)
                    Height = (int)size.Y;
                _textX = X + (Width - (int)size.X) / 2;
                _textY = Y + (Height - (int)size.Y) / 2;
            }
        }
    }
}
