using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.AchivementsView
{
    public partial class AchivementsView : BaseView
    {
        private static readonly Guid _id = new Guid("14b01cd7-8a0d-40ad-961e-562a915448d1");
        private readonly KeyWatcher _escapeKeyWatcher;
        public AchivementsView(UIEngine parent) : base(parent, _id)
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

        private void UpdateAchivementSelectionPages()
        {
            foreach (var buttons in _achivementsPages)
                foreach (var control in buttons)
                    control.IsVisible = false;

            foreach (var control in _achivementsPages[_currentAchivementsPage])
                control.IsVisible = true;
        }
    }
}
