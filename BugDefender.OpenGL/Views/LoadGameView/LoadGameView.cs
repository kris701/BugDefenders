using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.LoadGameView
{
    public partial class LoadGameView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("14b01cd7-8a0d-40ad-961e-562a915448d1");
        private readonly KeyWatcher _escapeKeyWatcher;
        public LoadGameView(BugDefenderGameWindow parent) : base(parent, _id)
        {
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }

        private void ContinueGameClick(ButtonControl sender)
        {
            if (sender.Tag is ISavedGame savedGame)
            {
                Parent.GameManager.NewGame(savedGame);
            }
        }

        private void DeleteGameClick(ButtonControl sender)
        {
            if (sender.Tag is ISavedGame savedGame)
            {
                Parent.UserManager.RemoveGame(savedGame);
                SwitchView(new LoadGameView(Parent));
            }
        }
    }
}
