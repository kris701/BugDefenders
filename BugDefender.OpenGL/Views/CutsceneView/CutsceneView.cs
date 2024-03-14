using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine.Input;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BugDefender.OpenGL.Screens.CutsceneView
{
    public partial class CutsceneView : BaseBugDefenderView
    {
        private static readonly Guid _id = new Guid("14b01cd7-8a0d-40ad-961e-562a915448d1");
        private readonly KeyWatcher _escapeKeyWatcher;
        private readonly Action<IView, ISavedGame> _onConversationOver;
        private readonly ISavedGame _savedGame;
        private readonly CutsceneDefinition _cutscene;
        private int _conversationIndex = 0;

        public CutsceneView(BugDefenderGameWindow parent, ISavedGame savedGame, CutsceneDefinition cutscene, Action<IView, ISavedGame> onConversationOver) : base(parent, _id)
        {
            _onConversationOver = onConversationOver;
            _cutscene = cutscene;
            _savedGame = savedGame;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, SkipConversation);
        }

        private void SkipConversation()
        {
            _conversationIndex++;
            if (_conversationIndex >= _cutscene.Conversation.Count)
                _onConversationOver.Invoke(this, _savedGame);
            else
            {
                var converation = _cutscene.Conversation[_conversationIndex];
                if (_conversationIndex % 2 == 0)
                {
                    _leftName.Text = converation.From;
                    _leftName.IsVisible = true;
                    _middleText.Text = converation.Text;
                    _leftSpeaker.FillColor = Parent.TextureController.GetTexture(converation.SpeakerID);
                    _leftSpeaker.IsVisible = true;

                    _rightName.IsVisible = false;
                    _rightSpeaker.IsVisible = false;
                }
                else
                {
                    _rightName.Text = converation.From;
                    _rightName.IsVisible = true;
                    _middleText.Text = converation.Text;
                    _rightSpeaker.FillColor = Parent.TextureController.GetTexture(converation.SpeakerID);
                    _rightSpeaker.IsVisible = true;

                    _leftName.IsVisible = false;
                    _leftSpeaker.IsVisible = false;
                }
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);
        }
    }
}
