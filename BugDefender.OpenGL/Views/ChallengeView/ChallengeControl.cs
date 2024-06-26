﻿using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Challenges;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using System;
using System.Text;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.ChallengeView
{
    public class ChallengeControl : CollectionControl
    {
        public ChallengeControl(BugDefenderGameWindow parent, ChallengeDefinition challenge, ClickedHandler click, bool isFinished)
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
                Text = challenge.Name,
                Font = parent.Fonts.GetFont(FontSizes.Ptx16),
                FontColor = Color.White,
            });
            var sb2 = new StringBuilder();
            sb2.AppendLine(challenge.Description);
            sb2.AppendLine($"Map: '{ResourceManager.Maps.GetResource(challenge.MapID).Name}'. Gamestyle: '{ResourceManager.GameStyles.GetResource(challenge.GameStyleID).Name}'");
            Children.Add(new TextboxControl()
            {
                X = 45,
                Y = 50,
                Width = 400,
                Height = 70,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                Text = sb2.ToString()
            });
            var sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in challenge.Criterias)
                sb.AppendLine(req.ToString());
            Children.Add(new TextboxControl()
            {
                X = 470,
                Y = 50,
                Width = 400,
                Height = 70,
                Font = parent.Fonts.GetFont(FontSizes.Ptx10),
                FontColor = Color.White,
                Text = sb.ToString()
            });
            var startButton = new BugDefenderButtonControl(parent, click)
            {
                X = 470,
                Y = 20,
                Width = 400,
                Height = 25,
                Font = parent.Fonts.GetFont(FontSizes.Ptx12),
                FontColor = Color.White,
                Text = $"[Reward: {challenge.Reward} credits] Start!",
                FillColor = parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = parent.Textures.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                IsEnabled = !isFinished,
                Tag = challenge
            };
            Children.Add(startButton);

            if (isFinished)
            {
                backgroundTile.FillColor = parent.Textures.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                startButton.Text = "Completed!";
            }
            else
                backgroundTile.FillColor = parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
        }
    }
}
