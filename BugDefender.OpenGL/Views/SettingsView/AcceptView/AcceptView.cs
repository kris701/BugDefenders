using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Settings;
using System.Runtime;
using BugDefender.OpenGL.Engine.Controls;

namespace BugDefender.OpenGL.Screens.SettingsView.AcceptView
{
    public partial class AcceptView : BaseView
    {
        private static readonly Guid _id = new Guid("937b4268-87fc-4f72-a180-e53ebd47a18d");
        private readonly SettingsDefinition _newSettings;
        private readonly SettingsDefinition _oldSettings;
        private readonly KeyWatcher _escapeKeyWatcher;
        private TimeSpan _waitFor = TimeSpan.FromSeconds(10);
        private int _secsLeft = 10;
        public AcceptView(UIEngine parent, SettingsDefinition oldSettings, SettingsDefinition newSettings) : base(parent, _id)
        {
            _oldSettings = oldSettings;
            _newSettings = newSettings;
            ScaleValue = parent.CurrentUser.UserData.Scale;
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
            Parent.CurrentUser.UserData = _newSettings;
            Parent.ApplySettings();
            Parent.UserManager.SaveUser(Parent.CurrentUser);
            SwitchView(new MainMenu.MainMenuView(Parent));
        }

        private void Cancel()
        {
            Parent.CurrentUser.UserData = _oldSettings;
            Parent.ApplySettings();
            Parent.UserManager.SaveUser(Parent.CurrentUser);
            SwitchView(new SettingsView(Parent));
        }
    }
}
