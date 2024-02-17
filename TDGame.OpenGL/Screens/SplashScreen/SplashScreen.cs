using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.SplashScreen
{
    public partial class SplashScreen : BaseScreen
    {
        public TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(5);

        public SplashScreen(UIEngine parent) : base(parent)
        {
            FadeInTime = 1000;
            FadeOutTime = 1000;
            ScaleValue = parent.Settings.Scale;
            Initialize();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();

            HoldTime -= gameTime.ElapsedGameTime;
            if (HoldTime <= TimeSpan.Zero || keyState.GetPressedKeyCount() > 0)
                SwitchView(new MainMenu.MainMenu(Parent));
        }
    }
}
