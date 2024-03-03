using BugDefender.Core.Resources;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace BugDefender.OpenGL.Screens.ChallengeView
{
    public partial class ChallengeView : BaseAnimatedView
    {
        private static readonly Guid _id = new Guid("7fe5d5b5-3be0-4bc9-a27c-08448042b881");
        private readonly KeyWatcher _escapeKeyWatcher;
        private static readonly string _saveDir = "Saves";
        private static readonly int _challengeCount = 5;

        private readonly List<Guid> _remainingChallenges;
        public ChallengeView(GameWindow parent) : base(
            parent,
            _id,
            parent.UIResources.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.UIResources.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;

            _remainingChallenges = new List<Guid>();
            DateTime a = DateTime.MinValue;
            DateTime now = DateTime.Now;
            TimeSpan ts = now - a;
            int hashValue = Math.Abs(ts.Days);
            if (Parent.CurrentUser.ChallengeDaySeed != hashValue)
            {
                Parent.CurrentUser.ChallengeDaySeed = hashValue;
                Parent.CurrentUser.CompletedChallenges.Clear();
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
            _waitLabel.Text = $"{_remainingChallenges.Count} challenges remaining for today. {ts.ToString("hh\\:mm\\:ss")} until reroll";
        }

        private void StartButton_Click(ButtonControl sender)
        {
            if (sender.Tag is Guid challengeId)
            {
                var saveFile = Path.Combine(_saveDir, $"{Parent.CurrentUser.ID}_save.json");
                if (File.Exists(saveFile))
                    File.Delete(saveFile);
                var challenge = ResourceManager.Challenges.GetResource(challengeId);
                SwitchView(new GameScreen.GameScreen(Parent, challenge));
            }
        }
    }
}
