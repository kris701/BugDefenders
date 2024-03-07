using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.OpenGL.Engine.Controls.Elements
{
    public class AnimatedTileElement
    {
        public delegate void OnAnimationDoneHandler(TileControl parent);
        public OnAnimationDoneHandler? OnAnimationDone;

        public TileControl Parent { get; set; }
        public List<Texture2D> TileSet { get; set; } = new List<Texture2D>();
        public int Frame { get; set; } = 0;
        public bool AutoPlay { get; set; } = true;
        public TimeSpan FrameTime { get; set; } = TimeSpan.FromMilliseconds(500);
        public bool Finished { get; set; } = false;
        private TimeSpan _currentFrameTime = TimeSpan.Zero;

        public AnimatedTileElement(TileControl parent)
        {
            Parent = parent;
        }

        public void Initialize()
        {
            if (TileSet.Count > 0)
            {
                Frame = 0;
                int targetW = TileSet[0].Width;
                int targetH = TileSet[0].Height;
                foreach (var tile in TileSet.Skip(1))
                    if (tile.Width != targetW || tile.Height != targetH)
                        throw new Exception("Animated tileset must have the same size!");

                if (Parent.Width == 0)
                    Parent.Width = TileSet[0].Width;
                if (Parent.Height == 0)
                    Parent.Height = TileSet[0].Height;
                Frame = 0;
                Parent.FillColor = TileSet[0];
            }
        }

        public void Update(GameTime gameTime)
        {
            if (TileSet.Count <= 1)
                return;

            if (Finished && !AutoPlay)
                return;
            else
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
                            Finished = true;
                            Frame = TileSet.Count - 1;
                        }
                        OnAnimationDone?.Invoke(Parent);
                    }
                    Parent.FillColor = TileSet[Frame];
                    _currentFrameTime = TimeSpan.Zero;
                }
            }
        }
    }
}
