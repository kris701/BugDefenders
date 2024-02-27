using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseScreen
    {
        private static Guid _id = new Guid("f2320690-8061-4f95-8373-972825f97d83");

        private Texture2D _screen;
        private int _score;
        private TimeSpan _gameTime;
        public GameOverView(UIEngine parent, Texture2D screen, int score, TimeSpan gameTime) : base(parent, _id)
        {
            _screen = screen;
            _score = score;
            _gameTime = gameTime;
            ScaleValue = parent.CurrentUser.UserData.Scale;

            Parent.CurrentUser.HighScores.Add(new ScoreDefinition(
                _score,
                gameTime.ToString("hh\\:mm\\:ss"),
                DateTime.Now.Date.ToShortDateString()
            ));
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
