using BugDefender.OpenGL.Engine.Controls.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugDefender.OpenGL.Engine.Controls
{
    public class AnimatedTileControl : TileControl
    {
        public List<Texture2D> TileSet
        {
            get => AnimatedElement.TileSet;
            set
            {
                AnimatedElement.TileSet = value;
                AnimatedElement.Finished = false;
            }
        }
        public int Frame
        {
            get => AnimatedElement.Frame;
            set => AnimatedElement.Frame = value;
        }
        public bool AutoPlay
        {
            get => AnimatedElement.AutoPlay;
            set => AnimatedElement.AutoPlay = value;
        }
        public TimeSpan FrameTime
        {
            get => AnimatedElement.FrameTime;
            set => AnimatedElement.FrameTime = value;
        }
        public AnimatedTileElement AnimatedElement;

        public AnimatedTileControl()
        {
            AnimatedElement = new AnimatedTileElement(this);
        }

        public override void Initialize()
        {
            AnimatedElement.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            AnimatedElement.Update(gameTime);
            base.Update(gameTime);
        }
    }
}
