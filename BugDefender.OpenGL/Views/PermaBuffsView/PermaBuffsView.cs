using MonoGame.OpenGL.Formatter.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("9db0a8d6-9ffa-4382-a858-bba0929c0b1f");
        private readonly KeyWatcher _escapeKeyWatcher;
        public PermaBuffsView(BugDefenderGameWindow parent) : base(parent, _id)
        {
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
