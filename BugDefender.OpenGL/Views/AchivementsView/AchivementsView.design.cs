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
            AddControl(0, new TileControl()
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("f9eb39aa-2164-4125-925d-83a1e94fbe93")),
                Width = GameWindow.BaseScreenSize.X,
                Height = GameWindow.BaseScreenSize.Y
            });

            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Achivements",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });
            AddControl(0, new LabelControl()
            {
                HorizontalAlignment = Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"You have {Parent.CurrentUser.Achivements.Count} unlocked! There are still {ResourceManager.Achivements.GetResources().Count - Parent.CurrentUser.Achivements.Count} to go!",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            int count = 0;
            int page = -1;
            int offset = 0;
            var ids = ResourceManager.Achivements.GetResources();
            var sorted = new List<AchivementDefinition>();
            foreach (var id in ids)
                sorted.Add(ResourceManager.Achivements.GetResource(id));
            sorted = sorted.OrderByDescending(x => Parent.CurrentUser.Achivements.Contains(x.ID)).ToList();

            foreach (var achivement in sorted)
            {
                if (count % _selectionsPrPage == 0)
                {
                    page++;
                    _achivementsPages.Add(new List<AchivementControl>());
                    offset = 0;
                }
                var newButton = new AchivementControl(Parent, achivement, Parent.CurrentUser.Achivements.Contains(achivement.ID))
                {
                    X = 500,
                    Y = 250 + offset++ * 135 + 5,
                };
                _achivementsPages[page].Add(newButton);
                AddControl(2, newButton);
                count++;
            }

            UpdateAchivementSelectionPages();
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
                X = 600,
                Y = 110,
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
                X = 1250,
                Y = 110,
                Height = 50,
                Width = 50,
                IsVisible = _achivementsPages.Count > 1
            });

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
