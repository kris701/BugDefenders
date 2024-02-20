using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TDGame.OpenGL.Engine.Input;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.AchivementsView
{
    public partial class AchivementsView : BaseScreen
    {
        private KeyWatcher _escapeKeyWatcher;
        public AchivementsView(UIEngine parent) : base(parent)
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
