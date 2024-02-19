using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Reflection;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreen : BaseScreen
    {
        private KeyWatcher _escapeKeyWatcher;
        private int _hash = 1;
        public UsersScreen(UIEngine parent) : base(parent)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenu(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
            var allUsers = Parent.UserManager.GetAllUsers();
            var newHash = 1;
            foreach (var user in allUsers)
                newHash *= user.ID.GetHashCode();
            if (newHash != _hash)
            {
                _hash = newHash;
                UpdateUsersList();
            }
        }

        private void AddUserButton_Click(ButtonControl sender)
        {
            if (_nameInputBox.Text != "")
            {
                Parent.CreateNewUser(_nameInputBox.Text);
                _nameInputBox.Text = "";
            }
            CheckValidUserInput();
        }

        private void RemoveUserButton_Click(ButtonControl sender)
        {
            if (sender.Tag is UserDefinition<SettingsDefinition> user)
            {
                if (Parent.CurrentUser.ID == user.ID)
                {
                    var allUsers = Parent.UserManager.GetAllUsers();
                    if (allUsers.Count == 1)
                        return;
                    Parent.ChangeUser(allUsers.First(x => x.ID != Parent.CurrentUser.ID));
                }
                Parent.UserManager.RemoveUser(user);
                SwitchView(new UsersScreen(Parent));
                CheckValidUserInput();
            }
        }

        private void ChangeUserButton_Click(ButtonControl sender)
        {
            if (sender.Tag is UserDefinition<SettingsDefinition> user)
            {
                if (Parent.CurrentUser.ID != user.ID)
                {
                    Parent.ChangeUser(user);
                    SwitchView(new UsersScreen(Parent));
                }
            }
        }

        private void CheckValidUserInput()
        {
            if (Parent.UserManager.GetAllUsers().Count > 10)
            {
                _nameInputBox.IsEnabled = false;
                _acceptButton.IsEnabled = false;
            }
            else
            {
                _nameInputBox.IsEnabled = true;
                _acceptButton.IsEnabled = true;
            }
        }
    }
}
