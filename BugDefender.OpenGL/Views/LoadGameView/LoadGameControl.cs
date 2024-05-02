using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Helpers;
using Microsoft.Xna.Framework;
using MonoGame.OpenGL.Formatter.Controls;
using System;
using static MonoGame.OpenGL.Formatter.Controls.ButtonControl;

namespace BugDefender.OpenGL.Views.LoadGameView
{
    public class LoadGameControl : CollectionControl
    {
        public LoadGameControl(BugDefenderGameWindow parent, ISavedGame saveGame, ClickedHandler clickedContinue, ClickedHandler removeClick)
        {
            Height = 140;
            Width = 900;
            Children.Add(new TileControl()
            {
                X = 0,
                Y = 0,
                Width = 900,
                Height = 140,
                FillColor = parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"))
            });
            Children.Add(new LabelControl()
            {
                X = 20,
                Y = 20,
                Width = 800,
                Height = 40,
                Font = parent.Fonts.GetFont(FontSizes.Ptx12),
                Text = $"{saveGame}",
                FontColor = Color.White
            });
            string continueText = "Continue";
            bool isEnabled = true;
            if (saveGame is CampaignSavedGame campaign)
            {
                isEnabled = !campaign.IsCompleted;
                if (campaign.IsCompleted)
                    continueText = "Completed!";
            }
            Children.Add(new ButtonControl(parent, clickedContinue)
            {
                X = 20,
                Y = 70,
                Width = 400,
                Height = 40,
                Font = parent.Fonts.GetFont(FontSizes.Ptx12),
                Text = continueText,
                FontColor = Color.White,
                FillColor = parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = parent.Textures.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Tag = saveGame,
                IsEnabled = isEnabled
            });
            Children.Add(new ButtonControl(parent, removeClick)
            {
                X = 480,
                Y = 70,
                Width = 400,
                Height = 40,
                Font = parent.Fonts.GetFont(FontSizes.Ptx12),
                Text = "Delete",
                FontColor = Color.Red,
                FillColor = parent.Textures.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.Textures.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                Tag = saveGame
            });
        }
    }
}
