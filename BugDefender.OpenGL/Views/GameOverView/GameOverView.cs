using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace BugDefender.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseAnimatedView
    {
        private static readonly Guid _id = new Guid("f2320690-8061-4f95-8373-972825f97d83");
        private static string _saveDir = "Saves";

        private readonly Texture2D _screen;
        private readonly int _score;
        private readonly int _credits;
        private readonly TimeSpan _gameTime;
        public GameOverView(GameWindow parent, Texture2D screen, int score, int credits, TimeSpan gameTime) : base(
            parent,
            _id,
            parent.UIResources.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.UIResources.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            _screen = screen;
            _score = score;
            _credits = credits;
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

            Parent.UIResources.PlaySong(ID);

            var saveFile = Path.Combine(_saveDir, $"{Parent.CurrentUser.ID}_save.json");
            if (File.Exists(saveFile))
                File.Delete(saveFile);
        }
    }
}
