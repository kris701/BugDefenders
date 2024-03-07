﻿using BugDefender.Core.Game;
using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace BugDefender.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("f2320690-8061-4f95-8373-972825f97d83");
        private static readonly string _saveDir = "Saves";

        private readonly Texture2D _screen;
        private readonly int _score;
        private readonly int _credits;
        private readonly TimeSpan _gameTime;
        private readonly GameResult _gameResult;
        public GameOverView(BugDefenderGameWindow parent, Texture2D screen, int score, int credits, TimeSpan gameTime, GameResult result) : base(
            parent,
            _id,
            parent.TextureController.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.TextureController.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            _screen = screen;
            _score = score;
            _credits = credits;
            _gameTime = gameTime;
            _gameResult = result;

            Parent.UserManager.CurrentUser.HighScores.Add(new ScoreDefinition(
                _score,
                gameTime.ToString("hh\\:mm\\:ss"),
                DateTime.Now.Date.ToShortDateString()
            ));
            if (Parent.UserManager.CurrentUser.HighScores.Count > 10)
            {
                var smallest = int.MaxValue;
                ScoreDefinition? smallestDef = null;
                foreach (var scoreDef in Parent.UserManager.CurrentUser.HighScores)
                {
                    if (scoreDef.Score < smallest)
                    {
                        smallestDef = scoreDef;
                        smallest = scoreDef.Score;
                    }
                }
                if (smallestDef != null)
                    Parent.UserManager.CurrentUser.HighScores.Remove(smallestDef);
            }
            Parent.UserManager.SaveUser();

            Initialize();

            Parent.AudioController.PlaySong(ID);

            var saveFile = Path.Combine(_saveDir, $"{Parent.UserManager.CurrentUser.ID}_save.json");
            if (File.Exists(saveFile))
                File.Delete(saveFile);
        }
    }
}
