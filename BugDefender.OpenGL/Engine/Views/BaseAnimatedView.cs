using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.ResourcePacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BugDefender.OpenGL.Engine.Views
{
    public class BaseAnimatedView : BaseView
    {
        public enum FadeState { AnimateIn, Hold, AnimateOut, PostHold }
        public FadeState State { get; set; } = FadeState.AnimateIn;

        private readonly AnimatedTileControl _tile;
        private readonly TextureSetDefinition _in;
        private readonly TextureSetDefinition _out;

        private IView? _switchTo;
        public BaseAnimatedView(GameWindow parent, Guid id, TextureSetDefinition inSet, TextureSetDefinition outSet) : base(parent, id)
        {
            _in = inSet;
            _out = outSet;
            _tile = new AnimatedTileControl(parent)
            {
                Width = 1000,
                Height = 1000,
                TileSet = _in.LoadedContents,
                FrameTime = TimeSpan.FromMilliseconds(_in.FrameTime),
                AutoPlay = false
            };
            _tile.OnAnimationDone += (s) =>
            {
                switch (State)
                {
                    case FadeState.AnimateIn:
                        State = FadeState.Hold;
                        _tile.IsVisible = false;
                        break;
                    case FadeState.AnimateOut:
                        State = FadeState.PostHold;
                        break;
                }
            };
            _tile.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            if (_tile.IsVisible)
                _tile.Draw(gameTime, spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_tile.IsVisible)
                _tile.Update(gameTime);
            if (State == FadeState.PostHold)
                if (_switchTo != null)
                    Parent.CurrentScreen = _switchTo;
        }

        public override void SwitchView(IView screen)
        {
            if (_switchTo == null)
            {
                _tile.TileSet = _out.LoadedContents;
                _tile.FrameTime = TimeSpan.FromMilliseconds(_out.FrameTime);
                _tile.Frame = 0;
                _tile.IsVisible = true;
                _tile.Initialize();
                _switchTo = screen;
                State = FadeState.AnimateOut;
            }
        }
    }
}
