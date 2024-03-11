using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BugDefender.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreenView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("0dd63d81-e49c-44d2-868e-7db6fb4634d7");
        private readonly KeyWatcher _escapeKeyWatcher;
        private bool _update = false;
        public UsersScreenView(BugDefenderGameWindow parent) : base(parent, _id)
        {
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
            if (_update)
            {
                UpdateUsersList();
                _update = false;
            }
        }

        private void AddUserButton_Click(ButtonControl sender)
        {
            if (_nameInputBox.Text != "")
            {
                AddUser(_nameInputBox.Text);
                _nameInputBox.Text = "";
            }
            CheckValidUserInput();
        }

        private void AddUserInputTextBox_Enter(TextInputControl sender)
        {
            if (_nameInputBox.Text != "")
            {
                AddUser(_nameInputBox.Text);
                _nameInputBox.Text = "";
            }
            CheckValidUserInput();
        }

        private void AddUser(string text)
        {
            Parent.UserManager.AddNewUser(text);
            _update = true;
        }

        private void RemoveUserButton_Click(ButtonControl sender)
        {
            if (sender.Tag is UserDefinition<SettingsDefinition> user)
            {
                if (Parent.UserManager.CurrentUser.ID == user.ID)
                {
                    if (Parent.UserManager.Users.Count == 1)
                        return;
                    Parent.UserManager.SwitchUser(Parent.UserManager.Users.First(x => x.ID != user.ID));
                    Parent.ApplySettings();
                    SwitchView(new UsersScreenView(Parent));
                }
                else
                {
                    Parent.UserManager.RemoveUser(user);
                    _update = true;
                    CheckValidUserInput();
                }
            }
        }

        private void ChangeUserButton_Click(ButtonControl sender)
        {
            if (sender.Tag is UserDefinition<SettingsDefinition> user)
            {
                if (Parent.UserManager.CurrentUser.ID != user.ID)
                {
                    Parent.UserManager.SwitchUser(user);
                    Parent.ApplySettings();
                    SwitchView(new UsersScreenView(Parent));
                }
            }
        }

        private void CheckValidUserInput()
        {
            if (Parent.UserManager.Users.Count > 10)
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
