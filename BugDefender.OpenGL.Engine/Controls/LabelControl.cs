using BugDefender.OpenGL.Engine.Controls.Elements;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class LabelControl : TileControl
    {
        public string Text
        {
            get => _element.Text;
            set => _element.Text = value;
        }
        public SpriteFont Font
        {
            get => _element.Font;
            set => _element.Font = value;
        }
        public Color FontColor
        {
            get => _element.FontColor;
            set => _element.FontColor = value;
        }
        public float TextX => _element._textX;
        public float TextY => _element._textY;
        public float TextWidth => _element._textWidth;
        public float TextHeight => _element._textHeight;

        private readonly TextElement _element;

        public LabelControl()
        {
            _element = new TextElement(this)
            {
                Text = "",
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White
            };
        }

        public override void Initialize()
        {
            _element.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _element.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _element.Draw(gameTime, spriteBatch);
        }
    }
}
