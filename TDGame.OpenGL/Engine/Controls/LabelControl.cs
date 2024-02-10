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
                _text = value;
                _textChanged = true;
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
        internal bool _textChanged = true;

        public LabelControl(IScreen parent) : base(parent)
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
            _textX = X + (Width - Parent.Scale(size.X)) / 2;
            _textY = Y + (Height - Parent.Scale(size.Y)) / 2;
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
                    new Vector2(_textX, _textY), 
                    new Color(FontColor.R, FontColor.G, FontColor.B, Alpha),
                    0,
                    new Vector2(),
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
