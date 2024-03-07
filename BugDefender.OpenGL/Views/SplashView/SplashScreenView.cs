﻿using BugDefender.OpenGL.Screens.MainMenu;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.SplashScreen
{
    public partial class SplashScreenView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("65ac71fc-1863-4a86-bc7b-2e24afe2fba7");
        public TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(5);

        public SplashScreenView(BugDefenderGameWindow parent) : base(
            parent,
            _id,
            parent.TextureController.GetTextureSet(new Guid("1c960708-4fd0-4313-8763-8191b6818bb4")),
            parent.TextureController.GetTextureSet(new Guid("9eb83a7f-5244-4ccc-8ef3-e88225ff1c18")))
        {
            Initialize();
            Parent.AudioController.PlaySong(MainMenuView._id);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();

            HoldTime -= gameTime.ElapsedGameTime;
            if (HoldTime <= TimeSpan.Zero || keyState.GetPressedKeyCount() > 0)
                SwitchView(new MainMenu.MainMenuView(Parent));
        }
    }
}
