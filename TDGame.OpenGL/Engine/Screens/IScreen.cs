using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TDGame.Core.Maps;

namespace TDGame.OpenGL.Engine.Screens
{
    public enum FadeState { FadeIn, Hold, FadeOut, PostHold }
    public interface IScreen
    {
        public TDGame Parent { get; set; }
        public int FadeInTime { get; }
        public int FadeOutTime { get; }
        public FadeState State { get; set; }

        public float ScaleValue { get; set; }
        public int Scale(int value);
        public double Scale(double value);
        public float Scale(float value);
        public WayPoint GetRelativePosition(MouseState from);

        public void AddControl(int layer, IControl control);

        public void Initialize();
        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
