using BugDefender.Core.Game;
using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BugDefender.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("f2320690-8061-4f95-8373-972825f97d83");
        private static readonly string _saveDir = "Saves";

        private readonly Texture2D _screen;
        private readonly int _credits;
        private readonly GameResult _gameResult;
        private readonly float _difficulty;
        private readonly GameContext _context;
        private readonly KeyWatcher _escapeKeyWatcher;
        private readonly List<string> _linesToShow;
        private int _lineIndex = 0;
        private TimeSpan _passed = TimeSpan.FromSeconds(1);
        public GameOverView(BugDefenderGameWindow parent, Texture2D screen, GameContext context, int credits, GameResult result, float difficulty) : base(parent, _id)
        {
            _screen = screen;
            _context = context;
            _credits = credits;
            _gameResult = result;
            _difficulty = difficulty;
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });

            Parent.UserManager.CurrentUser.HighScores.Add(new ScoreDefinition(
                context.Score,
                context.GameTime.ToString("hh\\:mm\\:ss"),
                DateTime.Now.Date.ToShortDateString(),
                difficulty
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

            _linesToShow = new List<string>();
            _linesToShow.Add($"Final Score: {context.Score}");
            _linesToShow.Add($"Game Difficulty: {Math.Round(_difficulty, 2)}");
            _linesToShow.Add($"Total Gametime: {context.GameTime.ToString("hh\\:mm\\:ss")}");
            _linesToShow.Add($"Game gives you {_credits} credits");
            _linesToShow.Add($" ");
            _linesToShow.Add($"Game Stats:");
            _linesToShow.Add($"Total Kills: {context.Stats.TotalKills}");
            _linesToShow.Add($"Total Turrets Placed: {context.Stats.TotalTurretsPlaced}");
            _linesToShow.Add($"Total Turrets Sold: {context.Stats.TotalTurretsSold}");
            _linesToShow.Add($"Total Money Earned: {context.Stats.TotalMoneyEarned}");
            _linesToShow.Add($"Total Waves Started: {context.Stats.TotalWavesStarted}");
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);

            if (_lineIndex < _linesToShow.Count)
            {
                _passed -= gameTime.ElapsedGameTime;
                if (_passed <= TimeSpan.Zero)
                {
                    _passed = TimeSpan.FromMilliseconds(500);
                    _lineIndex++;
                    var sb = new StringBuilder();
                    for (int i = 0; i < _lineIndex; i++)
                        sb.AppendLine(_linesToShow[i]);

                    _statsTextBox.Text = sb.ToString();
                    _statsTextBox.Initialize();
                    Parent.AudioController.PlaySoundEffectOnce(new Guid("3cca7fa9-014a-4ffa-8b95-00e775aa37c4"));
                }
            }
        }
    }
}
