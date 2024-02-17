using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDGame.OpenGL.Engine.Screens
{
    public enum FadeState { FadeIn, Hold, FadeOut, PostHold }
    public interface IScreen
    {
        public UIEngine Parent { get; set; }
        public int FadeInTime { get; }
        public int FadeOutTime { get; }
        public FadeState State { get; set; }

        public float ScaleValue { get; set; }
        public int Scale(int value);
        public double Scale(double value);
        public float Scale(float value);
        public int InvScale(int value);
        public float InvScale(float value);

        public void ClearLayer(int layer);
        public void AddControl(int layer, IControl control);
        public void RemoveControl(int layer, IControl control);

        public void Initialize();
        public void Update(GameTime gameTime);
        public void OnUpdate(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
