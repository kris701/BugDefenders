using BugDefender.Core.Users.Models.Buffs;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Text;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.PermaBuffsView
{
    public class PermaBuffControl : CollectionControl
    {
        public PermaBuffControl(GameWindow parent, BuffDefinition buff, ClickedHandler click, bool isUnlocked)
        {
            Width = 900;
            Height = 140;
            var backgroundTile = new TileControl()
            {
                X = 0,
                Y = 0,
                Width = 900,
                Height = 140
            };
            Children.Add(backgroundTile);
            Children.Add(new LabelControl()
            {

                X = 45,
                Y = 20,
                Width = 400,
                Height = 25,
                Text = buff.Name,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
            });
            Children.Add(new TextboxControl()
            {
                X = 45,
                Y = 50,
                Width = 400,
                Height = 70,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = buff.Description
            });
            var sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in buff.Criterias)
                sb.AppendLine(req.ToString());
            Children.Add(new TextboxControl()
            {
                X = 470,
                Y = 50,
                Width = 400,
                Height = 70,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = sb.ToString()
            });
            var buyButton = new ButtonControl(parent, click)
            {
                X = 470,
                Y = 20,
                Width = 400,
                Height = 25,
                Font = BasicFonts.GetFont(12),
                FontColor = Color.White,
                Text = $"[{buff.Cost} credits] Buy",
                FillColor = parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = parent.TextureController.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Tag = buff,
                IsEnabled = isUnlocked && parent.UserManager.CurrentUser.Credits >= buff.Cost
            };
            Children.Add(buyButton);
            if (isUnlocked)
            {
                if (parent.UserManager.CurrentUser.Buffs.Contains(buff.ID))
                {
                    backgroundTile.FillColor = parent.TextureController.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                    buyButton.Text = "Owned!";
                }
                else
                    backgroundTile.FillColor = parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
            }
            else
                backgroundTile.FillColor = parent.TextureController.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8"));
        }
    }
}
