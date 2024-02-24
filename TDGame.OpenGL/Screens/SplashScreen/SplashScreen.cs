using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.SplashScreen
{
    public partial class SplashScreen : BaseScreen
    {
        private static Guid _id = new Guid("65ac71fc-1863-4a86-bc7b-2e24afe2fba7");
        public TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(5);

        public SplashScreen(UIEngine parent) : base(parent, _id)
        {
            FadeInTime = 1000;
            FadeOutTime = 1000;
            ScaleValue = parent.CurrentUser.UserData.Scale;
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
