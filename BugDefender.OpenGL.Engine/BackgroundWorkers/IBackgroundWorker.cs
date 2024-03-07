using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.BackgroundWorkers
{
    public interface IBackgroundWorker
    {
        public Guid ID { get; }

        public void Initialize(); // Constructor level initialization
        public void Update(GameTime gameTime); // Update each frame
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch); // Draw each frame
    }
}
