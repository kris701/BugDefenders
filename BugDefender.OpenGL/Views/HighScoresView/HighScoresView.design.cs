using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Screens.HighScoresView
{
    public partial class HighScoresView : BaseBugDefenderView
    {
        private static readonly int _showCount = 13;

        public override void Initialize()
        {
            BasicMenuPage.GenerateBaseMenu(
                this,
                Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "High Scores",
                "Play the game and get your highscore onto the list!");

            var allScores = new List<ScoreDefinition>();
            foreach (var user in Parent.UserManager.Users)
                allScores.AddRange(user.HighScores);
            allScores = allScores.OrderByDescending(x => x.Score).ToList();
            allScores = allScores.Take(_showCount).ToList();

            int count = 0;
            foreach (var score in allScores)
            {
                AddControl(0, new LabelControl()
                {
                    HorizontalAlignment = HorizontalAlignment.Middle,
                    Y = 215 + (count++ * 60 + 5),
                    Height = 60,
                    Width = 1200,
                    Text = $"User: {Parent.UserManager.Users.First(x => x.HighScores.Contains(score)).Name}, Score: {score.Score}, Game Time: {score.GameTime}, Date: {score.Date}, Difficulty: {Math.Round(score.DifficultyRating, 2)}",
                    FontColor = Color.White,
                    Font = BasicFonts.GetFont(12),
                    FillColor = Parent.TextureController.GetTexture(new Guid("61bcf9c3-a78d-4521-8534-5690bdc2d6db")),
                });
            }

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 980,
                X = 1670,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.TextureController.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.TextureController.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
