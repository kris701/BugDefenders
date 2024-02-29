﻿using BugDefender.Core.Game.Helpers;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Screens.MainMenu
{
    public partial class MainMenuView : BaseAnimatedView
    {
        public static readonly Guid _id = new Guid("9c52281c-3202-4b22-bfc9-dfc187fdbeb3");
        private readonly KeysWatcher _cheatsInputWatcher;
        private readonly KeyWatcher _escapeInputWatcher;
        public MainMenuView(UIEngine parent) : base(
            parent,
            _id,
            parent.UIResources.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.UIResources.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            Parent.UIResources.PlaySong(ID);
            _cheatsInputWatcher = new KeysWatcher(new List<Keys>() { Keys.LeftAlt, Keys.Enter }, OpenCheatsMenu);
            _escapeInputWatcher = new KeyWatcher(Keys.Escape, CloseCheatsMenu);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _cheatsInputWatcher.Update(keyState);
            _escapeInputWatcher.Update(keyState);
        }

        private void OpenCheatsMenu()
        {
            _cheatsInput.IsVisible = true;
        }

        private void CloseCheatsMenu()
        {
            _cheatsInput.IsVisible = false;
        }

        private void OnEnterCheat(TextInputControl control)
        {
            var preCount = CheatsHelper.Cheats.Count;
            CheatsHelper.AddCheat(control.Text);
            if (CheatsHelper.Cheats.Count == preCount)
                Parent.UIResources.PlaySoundEffectOnce(new Guid("130c17d8-7cab-4fc0-8256-18092609f8d5"));
            else
                Parent.UIResources.PlaySoundEffectOnce(new Guid("aebfa031-8a3c-46c1-82dd-13a39d3caf36"));
            control.Text = "";
        }
    }
}