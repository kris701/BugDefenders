using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace BugDefender.OpenGL.Views.AchivementsView
{
    public class AchivementControl : CollectionControl
    {
        public AchivementControl(GameWindow parent, AchivementDefinition achivement, bool isUnlocked)
        {
            Height = 140;
            Width = 900;
            var backgroundTile = new TileControl()
            {
                X = 0,
                Y = 0,
                Width = 900,
                Height = 140,
            };
            if (isUnlocked)
            {
                if (parent.UserManager.CurrentUser.Achivements.Contains(achivement.ID))
                    backgroundTile.FillColor = parent.UIResources.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                else
                    backgroundTile.FillColor = parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            }
            else
                backgroundTile.FillColor = parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8"));
            Children.Add(backgroundTile);
            Children.Add(new TileControl()
            {
                Y = 30,
                X = 40,
                Width = 75,
                Height = 75,
                FillColor = parent.UIResources.GetTexture(achivement.ID)
            });
            Children.Add(new LabelControl()
            {
                X = 140,
                Y = 20,
                Width = 350,
                Height = 25,
                Text = achivement.Name,
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White,
            });
            Children.Add(new TextboxControl()
            {
                X = 140,
                Y = 50,
                Width = 350,
                Height = 70,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                Text = achivement.Description
            });
            var sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in achivement.Criterias)
                sb.AppendLine(req.ToString());
            Children.Add(new TextboxControl()
            {
                X = 500,
                Y = 20,
                Width = 350,
                Height = 100,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                Text = sb.ToString()
            });
        }
    }
}
