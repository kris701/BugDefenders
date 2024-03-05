using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine
{
    public enum HorizontalAlignment { None, Left, Middle, Right }
    public enum VerticalAlignment { None, Top, Middle, Bottom }
    public interface IControl
    {
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }

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
        public void OffsetFrom(IControl parent);
    }
}
