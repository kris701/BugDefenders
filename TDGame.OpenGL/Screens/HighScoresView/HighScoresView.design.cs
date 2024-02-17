using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.HighScoresView
{
    public partial class HighScoresView : BaseScreen
    {
        private static int _showCount = 10;

        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureManager.GetTexture(new Guid("fc3e4de1-331e-4a52-baee-e2f3c40962b4")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(this)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Width = 800,
                Text = "High Scores",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
            });

            var allUsers = Parent.UserManager.GetAllUsers();
            var allScores = new List<ScoreDefinition>();
            foreach(var user in allUsers)
                allScores.AddRange(user.HighScores);
            allScores = allScores.OrderByDescending(x => x.Score).ToList();
            allScores = allScores.Take(10).ToList();

            int count = 0;
            foreach(var score in allScores)
            {
                AddControl(0, new LabelControl(this)
                {
                    HorizontalAlignment = Engine.Alignment.Middle,
                    Y = 200 + (count++ * 35 + 5),
                    Height = 30,
                    Width = 800,
                    Text = $"User: {allUsers.First(x => x.HighScores.Contains(score)).Name}, Score: {score.Score}, Game Time: {score.GameTime}, Date: {score.Date}",
                    FontColor = Color.White,
                    Font = BasicFonts.GetFont(10),
                    FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                });
            }

            AddControl(0, new ButtonControl(this, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                X = 800,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
            });

#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new HighScoresView(Parent)))
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
