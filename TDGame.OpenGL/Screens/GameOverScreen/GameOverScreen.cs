using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverScreen : BaseScreen
    {
        private Texture2D _screen;
        private int _score;
        private TimeSpan _gameTime;
        public GameOverScreen(GameEngine parent, Texture2D screen, int score, TimeSpan gameTime) : base(parent)
        {
            _screen = screen;
            _score = score;
            _gameTime = gameTime;
            ScaleValue = parent.Settings.Scale;
            Initialize();
        }
    }
}
