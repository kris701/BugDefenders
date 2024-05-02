using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.OpenGL.Formatter.Controls;
using MonoGame.OpenGL.Formatter.Input;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Screens.ChallengeView
{
    public partial class ChallengeView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("7fe5d5b5-3be0-4bc9-a27c-08448042b881");
        private readonly KeyWatcher _escapeKeyWatcher;
        private static readonly int _challengeCount = 5;

        private readonly List<Guid> _remainingChallenges;
        public ChallengeView(BugDefenderGameWindow parent) : base(parent, _id)
        {

            _remainingChallenges = new List<Guid>();
            DateTime a = DateTime.MinValue;
            DateTime now = DateTime.Now;
            TimeSpan ts = now - a;
            int hashValue = Math.Abs(ts.Days);
            if (Parent.UserManager.CurrentUser.ChallengeDaySeed != hashValue)
            {
                Parent.UserManager.CurrentUser.ChallengeDaySeed = hashValue;
                Parent.UserManager.CurrentUser.CompletedChallenges.Clear();
            }
            var challenges = ResourceManager.Challenges.GetResources();
            var rnd = new Random(hashValue);
            while (_remainingChallenges.Count < _challengeCount)
            {
                var target = rnd.Next(0, challenges.Count);
                var id = challenges[target];
                if (!_remainingChallenges.Contains(id))
                    _remainingChallenges.Add(id);
            }

            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);

            DateTime a = DateTime.Today.AddDays(1);
            DateTime now = DateTime.Now;
            TimeSpan ts = a - now;
            _waitLabel.Text = $"{ts.ToString("hh\\:mm\\:ss")}";
        }

        private void StartButton_Click(ButtonControl sender)
        {
            if (sender.Tag is ChallengeDefinition challenge)
                Parent.GameManager.NewGame(new ChallengeSavedGame("Latest Challenge", challenge.ID, DateTime.Now, new GameContext(challenge.MapID, challenge.GameStyleID)));
        }
    }
}
