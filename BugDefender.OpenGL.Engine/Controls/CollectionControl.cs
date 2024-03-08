using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public abstract class CollectionControl : BaseControl
    {
        public List<IControl> Children { get; set; } = new List<IControl>();
        private bool _visibilityChanged = true;

        public override void Initialize()
        {
            base.Initialize();
            foreach (var child in Children)
            {
                child.OffsetFrom(this);
                child.Initialize();
            }
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (IsVisible != _visibilityChanged)
            {
                _visibilityChanged = IsVisible;
                foreach (var child in Children)
                    child.IsVisible = IsVisible;
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateVisibility();
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
