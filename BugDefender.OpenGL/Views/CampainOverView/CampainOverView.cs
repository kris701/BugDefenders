using BugDefender.Core.Game;
using BugDefender.Core.Users.Models;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BugDefender.OpenGL.Screens.CampainOverView
{
    public partial class CampainOverView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("e4924a0f-f853-48fb-8fbf-119b47242884");

        private readonly CampainSavedGame _saveGame;
        private readonly bool _won;
        private readonly KeyWatcher _escapeKeyWatcher;

        public CampainOverView(BugDefenderGameWindow parent, CampainSavedGame saveGame, bool won) : base(parent, _id)
        {
            _saveGame = saveGame;
            _won = won;
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { SwitchView(new MainMenu.MainMenuView(Parent)); });

            Initialize();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }
    }
}
