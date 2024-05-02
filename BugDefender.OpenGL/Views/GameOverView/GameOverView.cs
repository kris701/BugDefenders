using BugDefender.Core.Users.Models;
using MonoGame.OpenGL.Formatter.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugDefender.OpenGL.Screens.GameOverScreen
{
    public partial class GameOverView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("f2320690-8061-4f95-8373-972825f97d83");

        private readonly Texture2D _screen;
        private readonly StatsDefinition _stats;
        private readonly KeyWatcher _escapeKeyWatcher;
        private readonly List<string> _linesToShow;
        private int _lineIndex = 0;
        private TimeSpan _passed = TimeSpan.FromSeconds(1);
        private readonly string _title;
        public GameOverView(BugDefenderGameWindow parent, Texture2D screen, StatsDefinition stats, string title) : base(parent, _id)
        {
            _screen = screen;
            _stats = stats;
            _title = title;
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });

            Initialize();

            Parent.Audio.PlaySong(ID);

            _linesToShow = new List<string>();
            _linesToShow.Add($"Final Score: {stats.Score}");
            _linesToShow.Add($"Game Difficulty: {Math.Round(stats.Difficulty, 2)}");
            _linesToShow.Add($"Total Gametime: {stats.GameTime.ToString("hh\\:mm\\:ss")}");
            _linesToShow.Add($"Game gives you {stats.Credits} credits");
            _linesToShow.Add($" ");
            _linesToShow.Add($"Game Stats:");
            _linesToShow.Add($"Total Kills: {stats.TotalKills}");
            _linesToShow.Add($"Total Turrets Placed: {stats.TotalTurretsPlaced}");
            _linesToShow.Add($"Total Turrets Sold: {stats.TotalTurretsSold}");
            _linesToShow.Add($"Total Money Earned: {stats.TotalMoneyEarned}");
            _linesToShow.Add($"Total Waves Started: {stats.TotalWavesStarted}");
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
                    Parent.Audio.PlaySoundEffectOnce(new Guid("3cca7fa9-014a-4ffa-8b95-00e775aa37c4"));
                }
            }
        }
    }
}
