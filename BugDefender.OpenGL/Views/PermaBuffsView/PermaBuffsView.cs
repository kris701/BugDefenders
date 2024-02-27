﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;

namespace BugDefender.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseView
    {
        private static readonly Guid _id = new Guid("9db0a8d6-9ffa-4382-a858-bba0929c0b1f");
        private readonly KeyWatcher _escapeKeyWatcher;
        public PermaBuffsView(UIEngine parent) : base(parent, _id)
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
