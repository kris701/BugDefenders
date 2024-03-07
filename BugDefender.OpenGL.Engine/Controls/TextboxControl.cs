using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
                if (size.X > Width - Margin * 2)
                {
                    var newLabel = new LabelControl()
                    {
                        Font = Font,
                        Text = currentString,
                        FontColor = FontColor
                    };
                    newLabel.X = X;
                    newLabel.Y = Y + size.Y * lines.Count + Margin;
                    newLabel.Width = Width;
                    newLabel.Height = size.Y;
                    if (newLabel.Y + newLabel.Height > Y + Height - Margin)
                        break;
                    lines.Add(newLabel);
                    currentString = "";
                }
            }
            if (currentString != "")
            {
                var size = Font.MeasureString(currentString);
                var newLabel = new LabelControl()
                {
                    Font = Font,
                    Text = currentString,
                    FontColor = FontColor
                };
                newLabel.X = X;
                newLabel.Y = Y + size.Y * lines.Count + Margin;
                newLabel.Width = Width;
                newLabel.Height = size.Y;
                if (newLabel.Y + newLabel.Height > Y + Height - Margin)
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
