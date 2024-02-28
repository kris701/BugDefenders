using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Engine.Controls
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
        public float Margin { get; set; } = 5;
        internal bool _textChanged = true;
        internal List<LabelControl> lines = new List<LabelControl>();

        public TextboxControl(UIEngine parent) : base(parent)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (_textChanged && Text != "")
            {
                lines = new List<LabelControl>();
                var split = Text.Split(Environment.NewLine).ToList();
                split.RemoveAll(x => x == "");
                foreach (var line in split)
                    ProcessString(line);
                _textChanged = false;
            }
            foreach (var line in lines)
                line.Update(gameTime);
            base.Update(gameTime);
        }

        private void ProcessString(string str)
        {
            var currentString = "";
            foreach (var character in str)
            {
                currentString += character;
                var size = Font.MeasureString(currentString);
                if (Scale(size.X) > Width - Scale(Margin * 2))
                {
                    var newLabel = new LabelControl(Parent)
                    {
                        Font = Font,
                        Text = currentString,
                        FontColor = FontColor
                    };
                    newLabel._x = _x;
                    newLabel._y = _y + Scale(size.Y) * lines.Count + Scale(Margin);
                    newLabel._width = _width;
                    newLabel._height = Scale(size.Y);
                    if (newLabel._y + newLabel._height > _y + _height - Margin)
                        break;
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
                    FontColor = FontColor
                };
                newLabel._x = _x;
                newLabel._y = _y + Scale(size.Y) * lines.Count + Scale(Margin);
                newLabel._width = _width;
                newLabel._height = Scale(size.Y);
                if (newLabel._y + newLabel._height > _y + _height - Margin)
                    return;
                lines.Add(newLabel);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            foreach (var line in lines)
                line.Draw(gameTime, spriteBatch);
        }
    }
}
