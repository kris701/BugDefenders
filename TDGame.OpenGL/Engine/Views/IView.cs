using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TDGame.OpenGL.Engine.Views
{
    public enum FadeState { FadeIn, Hold, FadeOut, PostHold }
    public interface IView : IScalable
    {
        public Guid ID { get; set; }
        public int FadeInTime { get; }
        public int FadeOutTime { get; }
        public FadeState State { get; set; }

        public void ClearLayer(int layer);
        public void AddControl(int layer, IControl control);
        public void RemoveControl(int layer, IControl control);

        public void Initialize();
        public void Update(GameTime gameTime);
        public void OnUpdate(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
