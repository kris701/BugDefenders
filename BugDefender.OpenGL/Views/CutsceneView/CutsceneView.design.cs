using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Screens.GameOverScreen;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.AchivementsView;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BugDefender.OpenGL.Screens.CutsceneView
{
    public partial class CutsceneView : BaseBugDefenderView
    {
        public LabelControl _leftName;
        public TextboxControl _middleText;
        public TileControl _leftSpeaker;
        public LabelControl _rightName;
        public TileControl _rightSpeaker;

        [MemberNotNull(nameof(_leftName), nameof(_rightName), nameof(_middleText),
            nameof(_leftSpeaker), nameof(_rightSpeaker))]
        public override void Initialize()
        {
            AddControl(0, new TileControl()
            {
                X = 0,
                Y = 0,
                Width = 1920,
                Height = 1080,
                FillColor = Parent.TextureController.GetTexture(_cutscene.ID)
            });
            AddControl(0, new TileControl()
            {
                X = 30,
                Y = 30,
                Width = 1920 - 60,
                Height = 350,
                FillColor = Parent.TextureController.GetTexture(new Guid("6344ec42-5f76-4c44-9e03-11dd4d69c1d8"))
            });

            _leftName = new LabelControl()
            {
                X = 50,
                Y = 50 + 256 + 10,
                Width = 256,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                Text = _cutscene.Conversation[_conversationIndex].From,
                IsVisible = true
            };
            AddControl(0, _leftName);
            _leftSpeaker = new TileControl()
            {
                X = 50,
                Y = 50,
                Width = 256,
                Height = 256,
                FillColor = Parent.TextureController.GetTexture(_cutscene.Conversation[_conversationIndex].SpeakerID)
            };
            AddControl(0, _leftSpeaker);

            AddControl(0, _leftName);
            _middleText = new TextboxControl()
            {
                X = 50 + 256 + 10,
                Y = 50,
                Width = 1920 - 256 - 256 - 20 - 100,
                Height = 256,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
                Text = _cutscene.Conversation[_conversationIndex].Text,
                IsVisible = true
            };
            AddControl(0, _middleText);

            _rightName = new LabelControl()
            {
                X = 1920 - 256 - 50,
                Y = 50 + 256 + 10,
                Width = 256,
                Height = 50,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                IsVisible = false
            };
            AddControl(0, _rightName);
            _rightSpeaker = new TileControl()
            {
                X = 1920 - 256 - 50,
                Y = 50,
                Width = 256,
                Height = 256,
                IsVisible = false
            };
            AddControl(0, _rightSpeaker);

            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) =>
            {
                _onConversationOver.Invoke(this, _savedGame);
            })
            {
                X = 50,
                Y = 980,
                Width = 200,
                Height = 50,
                Text = "Skip All",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) =>
            {
                SkipConversation();
            })
            {
                Y = 980,
                X = 1670,
                Width = 200,
                Height = 50,    
                Text = "Continue",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });


#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new CutsceneView(Parent, _savedGame, _cutscene, _onConversationOver)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = BasicFonts.GetFont(10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif

            base.Initialize();
        }
    }
}
