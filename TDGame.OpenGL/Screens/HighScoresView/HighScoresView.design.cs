using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.HighScoresView
{
    public partial class HighScoresView : BaseScreen
    {
        private static int _showCount = 13;

        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = UIResourceManager.GetTexture(new Guid("fc3e4de1-331e-4a52-baee-e2f3c40962b4")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "High Scores",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Play the game and get your highscore onto the list!",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            var allUsers = Parent.UserManager.GetAllUsers();
            var allScores = new List<ScoreDefinition>();
            foreach (var user in allUsers)
                allScores.AddRange(user.HighScores);
            allScores = allScores.OrderByDescending(x => x.Score).ToList();
            allScores = allScores.Take(_showCount).ToList();

            int count = 0;
            foreach (var score in allScores)
            {
                AddControl(0, new LabelControl(Parent)
                {
                    HorizontalAlignment = Engine.Alignment.Middle,
                    Y = 215 + (count++ * 50 + 5),
                    Height = 50,
                    Width = 800,
                    Text = $"User: {allUsers.First(x => x.HighScores.Contains(score)).Name}, Score: {score.Score}, Game Time: {score.GameTime}, Date: {score.Date}",
                    FontColor = Color.White,
                    Font = BasicFonts.GetFont(10),
                    FillColor = UIResourceManager.GetTexture(new Guid("61bcf9c3-a78d-4521-8534-5690bdc2d6db")),
                });
            }

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new HighScoresView(Parent)))
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
