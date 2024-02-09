using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TDGame.OpenGL.Engine.Screens
{
    public delegate void SwitchEvent();
    public delegate void UpdatedEvent(GameTime passed);
    public enum FadeState { FadeIn, Hold, FadeOut, PostHold }
    public interface IScreen : ILookup
    {
        public event SwitchEvent EnteredView;
        public event SwitchEvent ExitedView;
        public event UpdatedEvent OnUpdate;
        public IEngine Parent { get; set; }
        public int FadeInTime { get; }
        public int FadeOutTime { get; }
        public FadeState State { get; set; }
        public void Initialize();
        public void LoadContent(ContentManager content);
        public void Refresh();
        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
