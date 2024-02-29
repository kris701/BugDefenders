using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.AchivementsView
{
    public partial class AchivementsView : BaseAnimatedView
    {
        private static readonly Guid _id = new Guid("14b01cd7-8a0d-40ad-961e-562a915448d1");
        private readonly KeyWatcher _escapeKeyWatcher;
        public AchivementsView(UIEngine parent) : base(
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
