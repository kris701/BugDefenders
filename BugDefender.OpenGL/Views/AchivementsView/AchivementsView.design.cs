using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models;
using BugDefender.OpenGL.Engine;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;
using BugDefender.OpenGL.Views.AchivementsView;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugDefender.OpenGL.Screens.AchivementsView
{
    public partial class AchivementsView : BaseAnimatedView
    {
        private readonly int _selectionsPrPage = 5;
        private readonly List<List<AchivementControl>> _achivementsPages = new List<List<AchivementControl>>();
        private int _currentAchivementsPage = 0;
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
                Text = "Achivements",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"You have {Parent.CurrentUser.Achivements.Count} unlocked! There are still {ResourceManager.Achivements.GetResources().Count - Parent.CurrentUser.Achivements.Count} to go!",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            int count = 1;
            int page = 0;
            int offset = 0;
            _achivementsPages.Add(new List<AchivementControl>());
            var ids = ResourceManager.Achivements.GetResources();
            var sorted = new List<AchivementDefinition>();
            foreach(var id in ids)
                sorted.Add(ResourceManager.Achivements.GetResource(id));
            sorted = sorted.OrderByDescending(x => Parent.CurrentUser.Achivements.Contains(x.ID)).ToList();

            foreach (var achivement in sorted)
            {
                var unlocked = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048"));
                if (!Parent.CurrentUser.Achivements.Contains(achivement.ID))
                    unlocked = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8"));
                if (count++ % (_selectionsPrPage + 1) == 0)
                {
                    page++;
                    _achivementsPages.Add(new List<AchivementControl>());
                    offset = 0;
                }
                var newButton = new AchivementControl(Parent, achivement)
                {
                    FillColor = unlocked,
                    X = 100,
                    Y = 270 + offset++ * 120,
                    IsVisible = false
                };
                _achivementsPages[page].Add(newButton);
                AddControl(2, newButton);
            }

            UpdateAchivementSelectionPages();

            if (count == 1)
            {
                AddControl(0, new LabelControl(Parent)
                {
                    HorizontalAlignment = Alignment.Middle,
                    Y = 500,
                    Height = 80,
                    Width = 700,
                    Text = $"You have unlocked no achivements yet",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White
                });
            }
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentAchivementsPage--;
                if (_currentAchivementsPage < 0)
                    _currentAchivementsPage = 0;
                if (_currentAchivementsPage >= _achivementsPages.Count)
                    _currentAchivementsPage = _achivementsPages.Count - 1;
                UpdateAchivementSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $"<",
                X = 70,
                Y = 210,
                Height = 50,
                Width = 50,
                IsVisible = _achivementsPages.Count > 1
            });
            AddControl(1, new ButtonControl(Parent, clicked: (s) =>
            {
                _currentAchivementsPage++;
                if (_currentAchivementsPage < 0)
                    _currentAchivementsPage = 0;
                if (_currentAchivementsPage >= _achivementsPages.Count)
                    _currentAchivementsPage = _achivementsPages.Count - 1;
                UpdateAchivementSelectionPages();
            })
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("d86347e3-3834-4161-9bbe-0d761d1d27ae")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("2c220d3f-5e7a-44ec-b4da-459f104c1e4a")),
                FontColor = Color.White,
                Font = BasicFonts.GetFont(16),
                Text = $">",
                X = 875,
                Y = 210,
                Height = 50,
                Width = 50,
                IsVisible = _achivementsPages.Count > 1
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
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new AchivementsView(Parent)))
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
