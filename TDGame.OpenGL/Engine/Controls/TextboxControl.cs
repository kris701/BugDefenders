using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class TextboxControl : TileControl
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
                _text = value;
                _textChanged = true;
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
        internal bool _textChanged = true;
        internal List<LabelControl> lines = new List<LabelControl>();

        public TextboxControl(IScreen parent) : base(parent)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_textChanged && Text != "")
            {
                lines = new List<LabelControl>();
                var currentString = "";
                foreach (var character in Text)
                {
                    currentString += character;
                    var size = Font.MeasureString(currentString);
                    if (Parent.Scale(size.X) > Width - 20)
                    {
                        var newLabel = new LabelControl(Parent)
                        {
                            Font = Font,
                            Text = currentString,
                        };
                        newLabel._x = _x;
                        newLabel._y = _y + size.Y * lines.Count;
                        newLabel._width = _width;
                        newLabel._height = size.Y;
                        lines.Add(newLabel);
                        currentString = "";
                    }
                }
                if (currentString != "")
                {
                    var size = Font.MeasureString(currentString);
                    var newLabel = new LabelControl(Parent)
                    {
                        Font = Font,
                        Text = currentString,
                    };
                    newLabel._x = _x;
                    newLabel._y = _y + size.Y * lines.Count;
                    newLabel._width = _width;
                    newLabel._height = size.Y;
                    lines.Add(newLabel);
                }
                _textChanged = false;
            }
            foreach (var line in lines)
                line.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            foreach (var line in lines)
                line.Draw(gameTime, spriteBatch);
        }
    }
}
