﻿using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.HighScoresView
{
    public partial class HighScoresView : BaseView
    {
        private static readonly Guid _id = new Guid("9f04f94c-75e6-413c-ba40-5582e78d4baa");

        private readonly KeyWatcher _escapeKeyWatcher;
        public HighScoresView(UIEngine parent) : base(parent, _id)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }
    }
}
