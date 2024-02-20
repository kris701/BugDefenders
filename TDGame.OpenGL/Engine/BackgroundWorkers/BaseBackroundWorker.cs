using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TDGame.OpenGL.Engine.BackgroundWorkers
{
    public abstract class BaseBackroundWorker : BaseScalable, IBackgroundWorker
    {
        public abstract Guid ID { get; }

        public BaseBackroundWorker(UIEngine parent) : base(parent)
        {
        }

        public virtual void Initialize()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
