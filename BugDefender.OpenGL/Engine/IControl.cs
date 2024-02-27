using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine
{
    public enum Alignment { None, Left, Middle, Right }
    public interface IControl : IScalable
    {
        public Alignment HorizontalAlignment { get; set; }

        public bool IsVisible { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Alpha { get; set; }
        public object? Tag { get; set; }
        public float Rotation { get; set; }
        public Rectangle ViewPort { get; set; }

        public void Initialize(); // Constructor level initialization
        public void Update(GameTime gameTime); // Update each frame
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch); // Draw each frame
    }
}
