using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Settings;

namespace TDGame.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreenView : BaseView
    {
        private static readonly Guid _id = new Guid("0dd63d81-e49c-44d2-868e-7db6fb4634d7");
        private readonly KeyWatcher _escapeKeyWatcher;
        private bool _update = false;
        public UsersScreenView(UIEngine parent) : base(parent, _id)
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
                Parent.CreateNewUser(_nameInputBox.Text);
                _nameInputBox.Text = "";
                _update = true;
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
                    SwitchView(new UsersScreenView(Parent));
                }
                Parent.UserManager.RemoveUser(user);
                _update = true;
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
