using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class AnimatedButtonControl : ButtonControl
    {
        public delegate void OnAnimationDoneHandler(AnimatedButtonControl parent);
        public OnAnimationDoneHandler? OnAnimationDone;

        public List<Texture2D> TileSet { get; set; } = new List<Texture2D>();
        public int Frame { get; set; } = 0;
        public bool AutoPlay { get; set; } = true;
        public TimeSpan FrameTime { get; set; } = TimeSpan.FromMilliseconds(500);
        private bool _finished = false;
        private TimeSpan _currentFrameTime = TimeSpan.Zero;

        public AnimatedButtonControl(IWindow parent, ClickedHandler? clicked = null) : base(parent, clicked)
        {
        }

        public override void Initialize()
        {
            if (TileSet.Count > 0)
            {
                Frame = 0;
                int targetW = TileSet[0].Width;
                int targetH = TileSet[0].Height;
                foreach (var tile in TileSet.Skip(1))
                    if (tile.Width != targetW || tile.Height != targetH)
                        throw new Exception("Animated tileset must have the same size!");

                if (Width == 0)
                    Width = TileSet[0].Width;
                if (Height == 0)
                    Height = TileSet[0].Height;
                Frame = 0;
                FillColor = TileSet[0];
            }
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (!_finished)
            {
                _currentFrameTime += gameTime.ElapsedGameTime;
                if (_currentFrameTime >= FrameTime)
                {
                    Frame++;
                    if (Frame >= TileSet.Count)
                    {
                        if (AutoPlay)
                            Frame = 0;
                        else
                        {
                            _finished = true;
                            Frame = TileSet.Count - 1;
                        }
                        OnAnimationDone?.Invoke(this);
                    }
                    FillColor = TileSet[Frame];
                    _currentFrameTime = TimeSpan.Zero;
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            if (Frame >= TileSet.Count)
                Frame = 0;

            base.Draw(gameTime, spriteBatch);
        }
    }
}
