﻿using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.AchivementsView;
using BugDefender.OpenGL.Views.Helpers;
using BugDefender.OpenGL.Views.LoadGameView;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BugDefender.OpenGL.Screens.LoadGameView
{
    public partial class LoadGameView : BaseBugDefenderView
    {
        private PageHandler<LoadGameControl> _loadGamePageHandler;

        [MemberNotNull(nameof(_loadGamePageHandler))]
        public override void Initialize()
        {
            BasicMenuPage.GenerateBaseMenu(
                this,
                Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Load a Saved Game",
                $"This user has a total of {Parent.UserManager.CurrentUser.SavedGames.Count} saved games");

            var controlList = new List<LoadGameControl>();
            foreach (var savedGame in Parent.UserManager.CurrentUser.SavedGames)
                controlList.Add(new LoadGameControl(
                    Parent,
                    savedGame,
                    ContinueGameClick,
                    DeleteGameClick));
            _loadGamePageHandler = new PageHandler<LoadGameControl>(this, controlList)
            {
                LeftButtonX = 10,
                LeftButtonY = -50,
                RightButtonX = 850,
                RightButtonY = -50,
                ItemsPrPage = 5,
                X = 500,
                Y = 250,
                Width = 800,
                Height = 725
            };
            AddControl(0, _loadGamePageHandler);

            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) =>
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
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new LoadGameView(Parent)))
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
