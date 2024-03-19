using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.AchivementsView;
using BugDefender.OpenGL.Views.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BugDefender.OpenGL.Screens.AchivementsView
{
    public partial class AchivementsView : BaseBugDefenderView
    {
        private PageHandler<AchivementControl> _achivementPageHandler;

        [MemberNotNull(nameof(_achivementPageHandler))]
        public override void Initialize()
        {
            BasicMenuHelper.GenerateBaseMenu(
                this,
                Parent.TextureController.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Achivements",
                $"You have {Parent.UserManager.CurrentUser.Achivements.Count} unlocked! There are still {ResourceManager.Achivements.GetResources().Count - Parent.UserManager.CurrentUser.Achivements.Count} to go!");

            var ids = ResourceManager.Achivements.GetResources();
            var sorted = new List<AchivementDefinition>();
            foreach (var id in ids)
                sorted.Add(ResourceManager.Achivements.GetResource(id));
            sorted = sorted.OrderByDescending(x => Parent.UserManager.CurrentUser.Achivements.Contains(x.ID)).ToList();

            var controlList = new List<AchivementControl>();
            foreach (var achivement in sorted)
                controlList.Add(new AchivementControl(
                    Parent,
                    achivement,
                    Parent.UserManager.CurrentUser.Achivements.Contains(achivement.ID)));
            _achivementPageHandler = new PageHandler<AchivementControl>(this, controlList)
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
            AddControl(0, _achivementPageHandler);
            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Back", (e) => { SwitchView(new MainMenu.MainMenuView(Parent)); }));
#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new AchivementsView(Parent)))
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
