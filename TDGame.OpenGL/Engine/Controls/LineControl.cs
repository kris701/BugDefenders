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
    public class LineControl : BaseControl
    {
        public float Thickness { get; set; } = 2;
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
                _x2 = Parent.Scale(value);
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
                _y2 = Parent.Scale(value);
            }
        }

        public LineControl(IScreen parent) : base(parent)
        {
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Stroke, new Vector2(X, Y), new Vector2(X2, Y2), Thickness, Alpha);
        }
    }
}
