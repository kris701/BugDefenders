using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Engine.Controls
{
    public class AnimatedTileControl : BaseControl
    {
        public delegate void OnAnimationDoneHandler(AnimatedTileControl parent);

        public OnAnimationDoneHandler? OnAnimationDone;
        public List<Texture2D> TileSet { get; set; } = new List<Texture2D>();
        public int Frame { get; set; } = 0;
        public bool AutoPlay { get; set; } = true;
        public TimeSpan FrameTime { get; set; } = TimeSpan.FromMilliseconds(500);
        private bool _finished = false;
        private TimeSpan _currentFrameTime = TimeSpan.Zero;

        public AnimatedTileControl(IScreen parent) : base(parent)
        {
        }

        public override void Initialize()
        {
            if (TileSet.Count > 0)
            {
                int targetW = TileSet[0].Width;
                int targetH = TileSet[0].Height;
                foreach (var tile in TileSet.Skip(1))
                    if (tile.Width != targetW || tile.Height != targetH)
                        throw new Exception("Animated tileset must have the same size!");

                if (Width == 0)
                    Width = TileSet[0].Width;
                if (Height == 0)
                    Height = TileSet[0].Height;
            }
            base.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            if (Frame > TileSet.Count)
                Frame = 0;

            if (TileSet.Count > 0)
                DrawTile(gameTime, spriteBatch, TileSet[Frame]);
        }

        public override void Update(GameTime gameTime)
        {
            if (TileSet.Count <= 1)
                return;

            if (_finished && !AutoPlay)
                return;

            _currentFrameTime += gameTime.ElapsedGameTime;
            if (_currentFrameTime >= FrameTime)
            {
                Frame++;
                if (Frame >= TileSet.Count)
                {
                    if (AutoPlay)
                        Frame = 0;
                    else
                        _finished = true;
                    if (OnAnimationDone != null)
                        OnAnimationDone.Invoke(this);
                }
                _currentFrameTime = TimeSpan.Zero;
            }

        }
    }
}
