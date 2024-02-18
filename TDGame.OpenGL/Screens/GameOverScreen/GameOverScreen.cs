using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverScreen : BaseScreen
    {
        private Texture2D _screen;
        private int _score;
        private TimeSpan _gameTime;
        public GameOverScreen(UIEngine parent, Texture2D screen, int score, TimeSpan gameTime) : base(parent)
        {
            _screen = screen;
            _score = score;
            _gameTime = gameTime;
            ScaleValue = parent.CurrentUser.UserData.Scale;

            Parent.CurrentUser.HighScores.Add(new Core.Users.Models.ScoreDefinition()
            {
                Score = _score,
                GameTime = gameTime.ToString("hh\\:mm\\:ss"),
                Date = DateTime.Now.Date.ToShortDateString(),
            });
            if (Parent.CurrentUser.HighScores.Count > 10)
            {
                var smallest = int.MaxValue;
                ScoreDefinition? smallestDef = null;
                foreach (var scoreDef in Parent.CurrentUser.HighScores)
                {
                    if (scoreDef.Score < smallest)
                    {
                        smallestDef = scoreDef;
                        smallest = scoreDef.Score;
                    }
                }
                if (smallestDef != null)
                    Parent.CurrentUser.HighScores.Remove(smallestDef);
            }
            Parent.UserManager.SaveUser(Parent.CurrentUser);

            Initialize();
        }
    }
}
