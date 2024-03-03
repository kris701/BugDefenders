using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.Buffs;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views.PermaBuffsView;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseAnimatedView
    {
        private readonly int _selectionsPrPage = 5;
        private readonly List<List<PermaBuffControl>> _upgradePages = new List<List<PermaBuffControl>>();
        private int _currentUpgradePage = 0;
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = 1000,
                Height = 1000
            });


            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Permanent Buffs",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Currently {Parent.CurrentUser.Buffs.Count} buffs are applied. You have {Parent.CurrentUser.Credits} credits.",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            var ids = ResourceManager.Buffs.GetResources();
            var sorted = new List<BuffDefinition>();
            foreach (var id in ids)
                sorted.Add(ResourceManager.Buffs.GetResource(id));
            sorted = sorted.OrderByDescending(x => !Parent.CurrentUser.Buffs.Contains(x.ID)).ThenByDescending(x => x.IsValid(Parent.CurrentUser)).ToList();

            _upgradePages.Add(new List<PermaBuffControl>());
            int page = 0;
            int offset = 0;
            int count = 1;
            foreach (var buff in sorted)
            {
                if (count++ % (_selectionsPrPage + 1) == 0)
                {
                    page++;
                    _upgradePages.Add(new List<PermaBuffControl>());
                    offset = 0;
                }
                var newButton = new PermaBuffControl(
                    Parent,
                    buff,
                    (x) =>
                    {
                        if (x.Tag is BuffDefinition buffDef)
                        {
                            if (Parent.CurrentUser.Credits >= buffDef.Cost)
                            {
                                Parent.UserManager.AddBuffUpgrade(Parent.CurrentUser, buffDef.ID);
                                SwitchView(new PermaBuffsView(Parent));
                            }
                        }
                    },
                    buff.IsValid(Parent.CurrentUser))
                {
                    X = 50,
                    Y = 210 + offset++ * 135 + 5,
                };
                _upgradePages[page].Add(newButton);
                AddControl(2, newButton);
            }

            UpdateUpgradeSelectionPages();
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentUpgradePage--;
                if (_currentUpgradePage < 0)
                    _currentUpgradePage = 0;
                if (_currentUpgradePage >= _upgradePages.Count)
                    _currentUpgradePage = _upgradePages.Count - 1;
                UpdateUpgradeSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $"<",
                X = 70,
                Y = 150,
                Height = 50,
                Width = 50,
                IsVisible = _upgradePages.Count > 1
            });
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentUpgradePage++;
                if (_currentUpgradePage < 0)
                    _currentUpgradePage = 0;
                if (_currentUpgradePage >= _upgradePages.Count)
                    _currentUpgradePage = _upgradePages.Count - 1;
                UpdateUpgradeSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $">",
                X = 875,
                Y = 150,
                Height = 50,
                Width = 50,
                IsVisible = _upgradePages.Count > 1
            });

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 900,
                X = 750,
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
