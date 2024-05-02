using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Buffs;
using BugDefender.OpenGL.Controls;
using MonoGame.OpenGL.Formatter.Helpers;
using BugDefender.OpenGL.Views;
using BugDefender.OpenGL.Views.Helpers;
using BugDefender.OpenGL.Views.PermaBuffsView;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BugDefender.OpenGL.Helpers;

namespace BugDefender.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseBugDefenderView
    {
        private PageHandler<PermaBuffControl> _buffPageHandler;

        [MemberNotNull(nameof(_buffPageHandler))]
        public override void Initialize()
        {
            BasicMenuHelper.GenerateBaseMenu(
                this,
                Parent.Textures.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                "Permanent Buffs",
                $"Currently {Parent.UserManager.CurrentUser.Buffs.Count} buffs are applied. You have {Parent.UserManager.CurrentUser.Credits} credits.",
                Parent.Fonts);

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
            _buffPageHandler = new PageHandler<PermaBuffControl>(this, controlList)
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
            AddControl(0, _buffPageHandler);

            AddControl(0, BasicMenuHelper.GetCancelButton(Parent, "Back", (e) => { SwitchView(new MainMenu.MainMenuView(Parent)); }));
#if DEBUG
            AddControl(0, new BugDefenderButtonControl(Parent, clicked: (x) => SwitchView(new PermaBuffsView(Parent)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = Parent.Fonts.GetFont(FontSizes.Ptx10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif
            base.Initialize();
        }
    }
}
