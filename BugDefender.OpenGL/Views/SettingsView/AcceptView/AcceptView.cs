using MonoGame.OpenGL.Formatter.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.SettingsView.AcceptView
{
    public partial class AcceptView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("937b4268-87fc-4f72-a180-e53ebd47a18d");
        private readonly SettingsDefinition _newSettings;
        private readonly SettingsDefinition _oldSettings;
        private readonly KeyWatcher _escapeKeyWatcher;
        private TimeSpan _waitFor = TimeSpan.FromSeconds(10);
        private int _secsLeft = 10;
        public AcceptView(BugDefenderGameWindow parent, SettingsDefinition oldSettings, SettingsDefinition newSettings) : base(parent, _id)
        {
            _oldSettings = oldSettings;
            _newSettings = newSettings;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, Cancel);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);

            _waitFor -= gameTime.ElapsedGameTime;
            if (_waitFor.Seconds != _secsLeft)
            {
                _secsLeft = _waitFor.Seconds;
                _timeLeftLabel.Text = $"{_secsLeft} seconds left";
            }
            if (_waitFor <= TimeSpan.Zero)
                Cancel();
        }

        private void Accept()
        {
            Parent.UserManager.CurrentUser.UserData = _newSettings;
            Parent.ApplySettings();
            Parent.UserManager.SaveUser();
            SwitchView(new MainMenu.MainMenuView(Parent));
        }

        private void Cancel()
        {
            Parent.UserManager.CurrentUser.UserData = _oldSettings;
            Parent.ApplySettings();
            Parent.UserManager.SaveUser();
            SwitchView(new SettingsView(Parent));
        }
    }
}
