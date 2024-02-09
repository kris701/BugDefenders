using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine
{
    public interface IEngine
    {
        public GraphicsDeviceManager Device { get; }
        public ContentManager Content { get; }
        public IScreen? CurrentScreen { get; }
        public void SwitchView(IScreen? screen);
        public int ScreenWidth();
        public int ScreenHeight();
        public void Exit();
    }
}
