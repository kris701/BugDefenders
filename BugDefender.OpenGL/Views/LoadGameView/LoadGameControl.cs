using BugDefender.Core.Users.Models.SavedGames;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BugDefender.OpenGL.Engine.Controls.ButtonControl;

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
                FillColor = parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"))
            });
            var typeStr = "";
            switch (saveGame)
            {
                case SurvivalSavedGame: typeStr = "(Survival) "; break;
                case ChallengeSavedGame: typeStr = "(Challenge) "; break;
                case CampainSavedGame: typeStr = "(Campain) "; break;
            }
            Children.Add(new LabelControl()
            {
                X = 20,
                Y = 20,
                Width = 400,
                Height = 40,
                Font = BasicFonts.GetFont(12),
                Text = $"{typeStr}{saveGame.Name}",
                FontColor = Color.White
            });
            Children.Add(new LabelControl()
            {
                X = 480,
                Y = 20,
                Width = 400,
                Height = 40,
                Font = BasicFonts.GetFont(12),
                Text = saveGame.Date.ToString(),
                FontColor = Color.White
            });
            string continueText = "Continue";
            bool isEnabled = true;
            if (saveGame is CampainSavedGame campain) {
                isEnabled = !campain.IsCompleted;
                if (campain.IsCompleted)
                    continueText = "Completed!";
            }
            Children.Add(new ButtonControl(parent, clickedContinue)
            {
                X = 20,
                Y = 70,
                Width = 400,
                Height = 40,
                Font = BasicFonts.GetFont(12),
                Text = continueText,
                FontColor = Color.White,
                FillColor = parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = parent.TextureController.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                Tag = saveGame,
                IsEnabled = isEnabled
            });
            Children.Add(new ButtonControl(parent, removeClick)
            {
                X = 480,
                Y = 70,
                Width = 400,
                Height = 40,
                Font = BasicFonts.GetFont(12),
                Text = "Delete",
                FontColor = Color.Red,
                FillColor = parent.TextureController.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = parent.TextureController.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                Tag = saveGame
            });
        }
    }
}
