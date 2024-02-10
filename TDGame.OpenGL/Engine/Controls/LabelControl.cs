using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine.Controls
{
    public class LabelControl : TileControl
    {
        private string _text = "";
        public string Text { 
            get { 
                return _text; 
            } set {
                if (value != _text)
                    _textChanged = true;
                _text = value;
            } 
        }
        public Color FontColor { get; set; } = Color.Black;
        private SpriteFont _font = BasicFonts.GetFont(8);
        public SpriteFont Font { 
            get { 
                return _font; 
            } set { 
                _font = value;
                _textChanged = true;
            } 
        }

        internal float _textX = 0;
        internal float _textY = 0;
        internal float _textWidth = 0;
        internal float _textHeight = 0;
        internal bool _textChanged = true;
        private float _magicScaleNumber => (float)Math.Pow(Parent.ScaleValue, -0.85);

        public LabelControl(IScreen parent) : base(parent)
        {
        }

        internal void UpdateTextPositions()
        {
            var size = Font.MeasureString(Text) * _magicScaleNumber;
            if (Width == 0)
                Width = size.X;
            if (Height == 0)
                Height = size.Y;
            ReAlign();
            _textWidth = Parent.Scale(size.X);
            _textHeight = Parent.Scale(size.Y);
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
            base.Draw(gameTime, spriteBatch);
            if (Text != "")
                spriteBatch.DrawString(
                    Font, 
                    Text,
                    new Vector2(_textX + _textWidth / 2, _textY + _textHeight / 2),
                    new Color(FontColor.R, FontColor.G, FontColor.B, Alpha),
                    Rotation,
                    new Vector2(_textWidth / 2, _textHeight / 2),
                    Parent.ScaleValue,
                    SpriteEffects.None,
                    0);
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
