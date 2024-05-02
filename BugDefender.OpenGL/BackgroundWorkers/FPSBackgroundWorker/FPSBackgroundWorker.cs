using MonoGame.OpenGL.Formatter.BackgroundWorkers;
using MonoGame.OpenGL.Formatter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using BugDefender.OpenGL.Helpers;

namespace BugDefender.OpenGL.BackgroundWorkers.FPSBackgroundWorker
{
    public class FPSBackgroundWorker : BaseBackroundWorker
    {
        private readonly BugDefenderGameWindow _parent;
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
                spriteBatch.DrawString(_parent.Fonts.GetFont(FontSizes.Ptx16), $"FPS: {_frames}", new Vector2(0, 0), new Color(255, 0, 0, 255));
        }
    }
}
