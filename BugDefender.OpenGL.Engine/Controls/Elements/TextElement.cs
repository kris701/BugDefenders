using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Engine.Controls.Elements
{
    public class TextElement
    {
        public IControl Parent { get; }
        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != _text)
                    _textChanged = true;
                _text = value;
            }
        }
        public Color FontColor { get; set; } = Color.White;
        private SpriteFont _font = BasicFonts.GetFont(8);
        public SpriteFont Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                _textChanged = true;
            }
        }

        internal float _textX = 0;
        internal float _textY = 0;
        internal float _textWidth = 0;
        internal float _textHeight = 0;
        internal bool _textChanged = true;

        public TextElement(IControl parent)
        {
            Parent = parent;
        }

        private void UpdateTextPositions()
        {
            var size = Font.MeasureString(Text);
            _textWidth = size.X;
            _textHeight = size.Y;
            _textX = Parent.X + Parent.Width / 2 - _textWidth / 2;
            _textY = Parent.Y + Parent.Height / 2 - _textHeight / 2;
        }

        public void Initialize()
        {
            UpdateTextPositions();
        }

        public void Update(GameTime gameTime)
        {
            if (_textChanged && Text != "")
            {
                _textChanged = false;
                UpdateTextPositions();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Text != "")
                spriteBatch.DrawString(
                    Font,
                    Text,
                    new Vector2(_textX + _textWidth / 2, _textY + _textHeight / 2),
                    new Color(FontColor.R, FontColor.G, FontColor.B, Parent.Alpha),
                    Parent.Rotation,
                    new Vector2(_textWidth / 2, _textHeight / 2),
                    1f,
                    SpriteEffects.None,
                    0);
#if TEXTBORDER
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY), new Rectangle((int)_textX, (int)_textY, (int)_textWidth, 1), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY), new Rectangle((int)_textX, (int)_textY, 1, (int)_textHeight), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX + _textWidth, _textY), new Rectangle((int)(_textX + _textWidth), (int)_textY, 1, (int)_textHeight), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY + _textHeight), new Rectangle((int)_textX, (int)(_textY + _textHeight), (int)(_textWidth + 1), 1), GetAlphaColor());
#endif
        }
    }
}
