using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.OpenGL.Engine.BackgroundWorkers
{
    public interface IBackgroundWorker : IScalable
    {
        public Guid ID { get; }

        public void Initialize(); // Constructor level initialization
        public void Update(GameTime gameTime); // Update each frame
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch); // Draw each frame
    }
}
