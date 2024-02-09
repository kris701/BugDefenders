using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TDGame.OpenGL.Engine
{
    public interface IRenderable
    {
        public void Initialize(); // Constructor level initialization
        public void LoadContent(ContentManager Content); // Loading of textures
        public void Refresh(); // Load positioning and sized of content
        public void Update(GameTime gameTime); // Update each frame
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch); // Draw each frame
    }
}
