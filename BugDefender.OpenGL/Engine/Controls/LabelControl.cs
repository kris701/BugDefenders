using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class LabelControl : TileControl
    {
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
        public Color FontColor { get; set; } = Color.Black;
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

        public LabelControl(UIEngine parent) : base(parent)
        {
        }

        internal void UpdateTextPositions()
        {
            var size = Font.MeasureString(Text);
            if (Width == 0)
                Width = size.X;
            if (Height == 0)
                Height = size.Y;
            ReAlign();
            _textWidth = Scale(size.X);
            _textHeight = Scale(size.Y);
            _textX = X + Width / 2 - _textWidth / 2;
            _textY = Y + Height / 2 - _textHeight / 2;
        }

        public override void Initialize()
        {
            UpdateTextPositions();
            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            DrawString(gameTime, spriteBatch);
        }

        internal void DrawString(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Text != "")
                spriteBatch.DrawString(
                    Font,
                    Text,
                    new Vector2(_textX + (_textWidth * ScaleValue) / 2, _textY + (_textHeight * ScaleValue) / 2),
                    new Color(FontColor.R, FontColor.G, FontColor.B, Alpha),
                    Rotation,
                    new Vector2(_textWidth / 2, _textHeight / 2),
                    ScaleValue,
                    SpriteEffects.None,
                    0);
#if TEXTBORDER
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY), new Rectangle((int)_textX, (int)_textY, (int)_textWidth, 1), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY), new Rectangle((int)_textX, (int)_textY, 1, (int)_textHeight), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX + _textWidth, _textY), new Rectangle((int)(_textX + _textWidth), (int)_textY, 1, (int)_textHeight), GetAlphaColor());
            spriteBatch.Draw(BasicTextures.GetBasicRectange(Color.Purple), new Vector2(_textX, _textY + _textHeight), new Rectangle((int)_textX, (int)(_textY + _textHeight), (int)(_textWidth + 1), 1), GetAlphaColor());
#endif
        }

        public override void Update(GameTime gameTime)
        {
            if (_textChanged && Text != "")
            {
                _textChanged = false;
                UpdateTextPositions();
            }
            base.Update(gameTime);
        }
    }
}
