using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Users.Models.SavedGames;
using MonoGame.OpenGL.Formatter.Input;
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
        private readonly CutsceneDefinition _cutscene;
        private int _conversationIndex = 0;
        private Guid _previousSpeaker = Guid.Empty;
        private string _targetText = "";
        private int _targetTextIndex = 0;
        private TimeSpan _targetTextUpdateTime = TimeSpan.Zero;
        private readonly ISavedGame _savedGame;
        private readonly Dictionary<Guid, string> _speakers;

        public CutsceneView(BugDefenderGameWindow parent, Dictionary<Guid, string> speakers, CutsceneDefinition cutscene, ISavedGame savedGame) : base(parent, _id)
        {
            _cutscene = cutscene;
            _savedGame = savedGame;
            _speakers = speakers;
            Initialize();
            _escapeKeyWatcher = new KeyWatcher(Keys.Escape, SkipConversation);
        }

        private void SkipConversation()
        {
            if (_targetTextIndex != _targetText.Length)
            {
                _targetTextIndex = _targetText.Length - 1;
                return;
            }

            _conversationIndex++;
            if (_conversationIndex >= _cutscene.Conversation.Count)
                Parent.GameManager.NewGame(_savedGame);
            else
            {
                var converation = _cutscene.Conversation[_conversationIndex];
                _middleText.Text = "";
                _targetText = converation.Text;
                _targetTextIndex = 0;

                if (_previousSpeaker != converation.SpeakerID)
                {
                    if (_rightName.IsVisible == true)
                    {
                        _leftName.Text = _speakers[converation.SpeakerID];
                        _leftName.IsVisible = true;
                        _leftSpeaker.FillColor = Parent.Textures.GetTexture(converation.SpeakerID);
                        _leftSpeaker.IsVisible = true;

                        _rightName.IsVisible = false;
                        _rightSpeaker.IsVisible = false;
                    }
                    else
                    {
                        _rightName.Text = _speakers[converation.SpeakerID];
                        _rightName.IsVisible = true;
                        _rightSpeaker.FillColor = Parent.Textures.GetTexture(converation.SpeakerID);
                        _rightSpeaker.IsVisible = true;

                        _leftName.IsVisible = false;
                        _leftSpeaker.IsVisible = false;
                    }
                }
                _previousSpeaker = converation.SpeakerID;
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            _escapeKeyWatcher.Update(keyState);

            if (_targetTextIndex < _targetText.Length)
            {
                _targetTextUpdateTime -= gameTime.ElapsedGameTime;
                if (_targetTextUpdateTime <= TimeSpan.Zero)
                {
                    _targetTextUpdateTime = TimeSpan.FromMilliseconds(30);
                    _targetTextIndex++;
                    _middleText.Text = _targetText.Substring(0, _targetTextIndex);

                    if (_targetTextIndex % 2 == 0)
                        Parent.Audio.PlaySoundEffectOnce(new Guid("a741f37c-ad92-43f7-9a0c-e215d26e6bc7"));
                }
            }
        }
    }
}
