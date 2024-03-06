using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Buffs;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views.Helpers;
using BugDefender.OpenGL.Views.PermaBuffsView;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseAnimatedView
    {
        private readonly PageHandler<PermaBuffControl> _buffPageHandler = new PageHandler<PermaBuffControl>()
        {
            LeftButtonX = 500,
            LeftButtonY = 110,
            RightButtonX = 1350,
            RightButtonY = 110,
            ItemsPrPage = 5,
            X = 500,
            Y = 250,
        };
        public override void Initialize()
        {
            BasicMenuPage.GenerateBaseMenu(
                this,
                Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Permanent Buffs",
                $"Currently {Parent.UserManager.CurrentUser.Buffs.Count} buffs are applied. You have {Parent.UserManager.CurrentUser.Credits} credits.");

            var ids = ResourceManager.Buffs.GetResources();
            var sorted = new List<BuffDefinition>();
            foreach (var id in ids)
                sorted.Add(ResourceManager.Buffs.GetResource(id));
            sorted = sorted.OrderByDescending(x => !Parent.UserManager.CurrentUser.Buffs.Contains(x.ID)).ThenByDescending(x => x.IsValid(Parent.UserManager.CurrentUser)).ToList();

            var controlList = new List<PermaBuffControl>();
            foreach (var buff in sorted)
            {
                var newButton = new PermaBuffControl(
                    Parent,
                    buff,
                    (x) =>
                    {
                        if (x.Tag is BuffDefinition buffDef)
                        {
                            if (Parent.UserManager.CurrentUser.Credits >= buffDef.Cost)
                            {
                                Parent.UserManager.AddBuffUpgrade(buffDef.ID);
                                SwitchView(new PermaBuffsView(Parent));
                            }
                        }
                    },
                    buff.IsValid(Parent.UserManager.CurrentUser))
                {
                    Tag = buff
                };
                controlList.Add(newButton);
            }
            _buffPageHandler.Initialize(controlList, this);

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
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });
#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new PermaBuffsView(Parent)))
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
