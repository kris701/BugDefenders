using BugDefender.OpenGL.Engine.BackgroundWorkers;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BugDefender.OpenGL.BackgroundWorkers.FPSBackgroundWorker
{
    public class FPSBackgroundWorker : BaseBackroundWorker
    {
        private readonly BugDefenderGameWindow _parent;
        public override Guid ID { get; } = new Guid("685946a7-a6b4-4a5d-88c4-7b4ba6dee748");
        private TimeSpan _passed = TimeSpan.Zero;
        private int _currentFrames = 0;
        private int _frames = 0;
        public FPSBackgroundWorker(BugDefenderGameWindow parent)
        {
            _parent = parent;
        }

        public override void Update(GameTime gameTime)
        {
            if (_parent.UserManager.CurrentUser.UserData.FPSCounter)
            {
                _currentFrames++;
                _passed += gameTime.ElapsedGameTime;
                if (_passed >= TimeSpan.FromSeconds(1))
                {
                    _passed = TimeSpan.Zero;
                    _frames = _currentFrames;
                    _currentFrames = 0;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_parent.UserManager.CurrentUser.UserData.FPSCounter)
                spriteBatch.DrawString(BasicFonts.GetFont(16), $"FPS: {_frames}", new Vector2(0, 0), new Color(255, 0, 0, 255));
        }
    }
}
