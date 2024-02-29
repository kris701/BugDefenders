using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseAnimatedView
    {
        private static readonly Guid _id = new Guid("9db0a8d6-9ffa-4382-a858-bba0929c0b1f");
        private readonly KeyWatcher _escapeKeyWatcher;
        public PermaBuffsView(UIEngine parent) : base(
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
        }
    }
}
