using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class LineControl : BaseControl
    {
        private float _thickness = 2;
        public float Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = Scale(value);
            }
        }
        public Texture2D Stroke { get; set; } = BasicTextures.GetBasicRectange(Color.Red);
        private float _x2 = 0;
        public float X2
        {
            get
            {
                return _x2;
            }
            set
            {
                _x2 = Scale(value);
            }
        }
        private float _y2 = 0;
        public float Y2
        {
            get
            {
                return _y2;
            }
            set
            {
                _y2 = Scale(value);
            }
        }

        public LineControl(GameWindow parent) : base(parent)
        {
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Stroke, new Vector2(X, Y), new Vector2(X2, Y2), Thickness, Alpha);
        }
    }
}
