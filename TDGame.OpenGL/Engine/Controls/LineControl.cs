using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class LineControl : BaseControl
    {
        public float Thickness { get; set; } = 2;
        public Texture2D Stroke { get; set; }
        public int X2 { get; set; } = 0;
        public int Y2 { get; set; } = 0;

        public LineControl()
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            spriteBatch.DrawLine(Stroke, new Vector2(X, Y), new Vector2(X2, Y2), Thickness);
        }
    }
}
