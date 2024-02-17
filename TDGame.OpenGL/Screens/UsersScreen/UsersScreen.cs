using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreen : BaseScreen
    {
        private int _hash = 1;
        public UsersScreen(UIEngine parent) : base(parent)
        {
            ScaleValue = parent.Settings.Scale;
            Initialize();
        }

        public override void OnUpdate(GameTime gameTime)
        {
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
                Parent.CreateNewUser(_nameInputBox.Text);
        }

        private void RemoveUserButton_Click(ButtonControl sender)
        {
            if (sender.Tag is UserDefinition user)
            {
                if (Parent.CurrentUser.ID == user.ID)
                {
                    var allUsers = Parent.UserManager.GetAllUsers();
                    if (allUsers.Count == 1)
                        return;
                    Parent.ChangeUser(allUsers[0]);
                }
                Parent.DeleteUser(user);
                SwitchView(new UsersScreen(Parent));
            }
        }

        private void ChangeUserButton_Click(ButtonControl sender)
        {
            if (sender.Tag is UserDefinition user)
            {
                if (Parent.CurrentUser.ID != user.ID)
                {
                    foreach (var button in _usersButtons)
                        button.FillColor = BasicTextures.GetBasicRectange(Color.LightGray);
                    sender.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
                    Parent.ChangeUser(user);
                    SwitchView(new UsersScreen(Parent));
                }
            }
        }
    }
}
