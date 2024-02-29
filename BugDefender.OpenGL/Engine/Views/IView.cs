using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BugDefender.OpenGL.Engine.Views
{
    public interface IView : IScalable
    {
        public Guid ID { get; set; }

        public void ClearLayer(int layer);
        public void AddControl(int layer, IControl control);
        public void RemoveControl(int layer, IControl control);

        public void Initialize();
        public void Update(GameTime gameTime);
        public void OnUpdate(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public void SwitchView(IView screen);
    }
}
