using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Views
{
    public abstract class BaseFadeView : BaseView
    {
        public enum FadeState { FadeIn, Hold, FadeOut, PostHold }
        public FadeState State { get; set; } = FadeState.FadeIn;

        public int FadeInTime { get; set; } = 200;
        public int FadeOutTime { get; set; } = 200;

        private double fadeTimer = 0;
        private int fadeValue = 255;
        private IView? _switchTo;
        private readonly Texture2D _fillColor = BasicTextures.GetBasicRectange(Color.Black);

        public BaseFadeView(IWindow parent, Guid id) : base(parent, id)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            spriteBatch.FillScreen(_fillColor, IWindow.BaseScreenSize.X, IWindow.BaseScreenSize.Y, fadeValue);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (State)
            {
                case FadeState.FadeIn:
                    fadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    fadeValue = 255 - (int)(fadeTimer / FadeInTime * 255);
                    if (fadeTimer >= FadeInTime)
                    {
                        fadeTimer = 0;
                        State = FadeState.Hold;
                        fadeValue = 0;
                    }
                    break;
                case FadeState.FadeOut:
                    fadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    fadeValue = (int)(fadeTimer / FadeOutTime * 255);
                    if (fadeTimer >= FadeOutTime)
                    {
                        if (_switchTo != null)
                            Parent.CurrentScreen = _switchTo;
                        State = FadeState.PostHold;
                    }
                    break;
            }
        }

        public override void SwitchView(IView screen)
        {
            if (_switchTo == null)
            {
                _switchTo = screen;
                State = FadeState.FadeOut;
            }
        }
    }
}
