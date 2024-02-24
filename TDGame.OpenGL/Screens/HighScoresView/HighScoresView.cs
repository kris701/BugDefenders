using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.HighScoresView
{
    public partial class HighScoresView : BaseScreen
    {
        private static Guid _id = new Guid("9f04f94c-75e6-413c-ba40-5582e78d4baa");

        private KeyWatcher _escapeKeyWatcher;
        public HighScoresView(UIEngine parent) : base(parent, _id)
        {
            ScaleValue = parent.CurrentUser.UserData.Scale;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenu(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }
    }
}
