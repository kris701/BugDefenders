using BugDefender.Core.Game;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
                SwitchView(new GameScreen.GameScreen(Parent, new ChallengeSavedGame("Latest Challenge", challenge.ID, DateTime.Now, new GameContext(challenge.MapID, challenge.GameStyleID)), OnGameOver));
        }

        private void OnGameOver(GameEngine game, ISavedGame gameSave)
        {
            var credits = 0;
            var result = game.Result;
#if RELEASE
            if (CheatsHelper.Cheats.Count == 0)
            {
#endif
            if (gameSave is ChallengeSavedGame c)
            {
                var challenge = ResourceManager.Challenges.GetResource(c.ChallengeID);
                credits += challenge.Reward;
                Parent.UserManager.CurrentUser.CompletedChallenges.Add(c.ChallengeID);
            }

            if (result == GameResult.Success)
            {
                Parent.UserManager.CurrentUser.Stats.Combine(game.Context.Stats);
                credits += (game.Context.Score / 100);
                Parent.UserManager.CurrentUser.Credits += credits;
                Parent.UserManager.CheckAndApplyAchivements();
                Parent.UserManager.SaveUser();
            }
#if RELEASE
            }
#endif
            Parent.UserManager.RemoveGame(gameSave);
            var screen = GameScreenHelper.TakeScreenCap(Parent.GraphicsDevice, Parent);
            SwitchView(new GameOverScreen.GameOverView(Parent, screen, game.Context, credits, game.Result, game.Context.Map.GetDifficultyRating() * game.Context.GameStyle.GetDifficultyRating(), "Game Over!"));
        }
    }
}
