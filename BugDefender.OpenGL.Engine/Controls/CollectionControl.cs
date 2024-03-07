using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public abstract class CollectionControl : BaseControl
    {
        public List<IControl> Children { get; set; } = new List<IControl>();

        public override void Initialize()
        {
            base.Initialize();
            foreach (var child in Children)
            {
                child.OffsetFrom(this);
                child.Initialize();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (var child in Children)
                child.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }
    }
}
