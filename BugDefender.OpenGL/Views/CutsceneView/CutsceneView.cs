using BugDefender.Core.Campain.Models;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BugDefender.OpenGL.Screens.CutsceneView
{
    public partial class CutsceneView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("14b01cd7-8a0d-40ad-961e-562a915448d1");
        private readonly KeyWatcher _escapeKeyWatcher;
        private readonly Action<IView, ISavedGame> _onConversationOver;
        private readonly ISavedGame _savedGame;
        public CutsceneView(BugDefenderGameWindow parent, ISavedGame savedGame, CutsceneDefinition cutscene, Action<IView, ISavedGame> onConversationOver) : base(parent, _id)
        {
            _onConversationOver = onConversationOver;
            _savedGame = savedGame;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, () => { _onConversationOver.Invoke(this, _savedGame); });
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }
    }
}
