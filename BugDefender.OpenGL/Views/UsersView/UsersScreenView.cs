using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace BugDefender.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreenView : BaseAnimatedView
    {
        private static readonly Guid _id = new Guid("0dd63d81-e49c-44d2-868e-7db6fb4634d7");
        private readonly KeyWatcher _escapeKeyWatcher;
        private bool _update = false;
        public UsersScreenView(UIEngine parent) : base(
            parent,
            _id,
            parent.UIResources.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.UIResources.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
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
            Parent.CreateNewUser(text);
            _update = true;
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
                    Parent.ChangeUser(allUsers.First(x => x.ID != user.ID));
                    Parent.UserManager.RemoveUser(user);
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
                if (Parent.CurrentUser.ID != user.ID)
                {
                    Parent.ChangeUser(user);
                    SwitchView(new UsersScreenView(Parent));
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
