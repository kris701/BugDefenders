using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class LineControl : BaseControl
    {
        public float Thickness { get; set; }
        public Texture2D Stroke { get; set; } = BasicTextures.GetBasicRectange(Color.Red);
        public float X2 { get; set; }
        public float Y2 { get; set; }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(Stroke, new Vector2(X, Y), new Vector2(X2, Y2), Thickness, Alpha);
        }
    }
}
