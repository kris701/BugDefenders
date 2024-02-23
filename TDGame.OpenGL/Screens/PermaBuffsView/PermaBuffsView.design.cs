using Microsoft.Xna.Framework;
using System;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseScreen
    {
        private int _upgradeViewCount = 5;
        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = UIResourceManager.GetTexture(new Guid("1ba73c85-76b6-4fa1-9cd6-680598c6163b")),
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
                Text = $"Currently {Parent.CurrentUser.Buffs.Count} buffs are applied",
                Font = BasicFonts.GetFont(16),
                FontColor = Color.White
            });

            var buffs = ResourceManager.Buffs.GetResources();
            int count = 0;
            foreach (var id in buffs)
            {
                if (count >= buffs.Count || count >= _upgradeViewCount)
                    break;
                var buff = ResourceManager.Buffs.GetResource(id);
                if (!buff.IsValid(Parent.CurrentUser) || Parent.CurrentUser.Buffs.Contains(id))
                    continue;

                AddControl(0, new LabelControl(Parent)
                {
                    X = 100,
                    Y = 210 + count * 140 + 5,
                    Width = 400,
                    Height = 50,
                    Text = buff.Name,
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                });
                AddControl(0, new TextboxControl(Parent)
                {
                    X = 100,
                    Y = 260 + count * 140 + 5,
                    Width = 800,
                    Height = 75,
                    Text = buff.Description,
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("61bcf9c3-a78d-4521-8534-5690bdc2d6db")),
                    Margin = 15
                });
                AddControl(0, new ButtonControl(Parent, clicked: (x) =>
                {
                    Parent.UserManager.AddBuffUpgrade(Parent.CurrentUser, id);
                    SwitchView(new PermaBuffsView(Parent));
                })
                {
                    X = 500,
                    Y = 210 + count * 140 + 5,
                    Width = 400,
                    Height = 50,
                    Text = "Claim",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = UIResourceManager.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = UIResourceManager.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                });
                count++;
            }

            if (count == 0)
            {
                AddControl(0, new LabelControl(Parent)
                {
                    HorizontalAlignment = Alignment.Middle,
                    Y = 500,
                    Height = 80,
                    Width = 700,
                    Text = $"No buffs available yet! Keep playing the game to unlock them",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White
                });
            }

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = UIResourceManager.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = UIResourceManager.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
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
