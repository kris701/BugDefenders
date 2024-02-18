using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TDGame.Core.Resources;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.PermaBuffsView
{
    public partial class PermaBuffsView : BaseScreen
    {
        private int _upgradeViewCount = 10;
        private List<ButtonControl> _upgradeButtons = new List<ButtonControl>();
        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureManager.GetTexture(new Guid("1ba73c85-76b6-4fa1-9cd6-680598c6163b")),
                Width = 1000,
                Height = 1000
            });


            AddControl(0, new LabelControl(this)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 700,
                Text = "Permanent Buffs",
                Font = BasicFonts.GetFont(48),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
            });
            AddControl(0, new LabelControl(this)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 175,
                Height = 35,
                Width = 700,
                Text = $"Currently {Parent.CurrentUser.Buffs.Count} buffs are applied",
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
            });

            var buffs = ResourceManager.Buffs.GetResources();
            int count = 0;
            foreach(var id in buffs)
            {
                if (count >= buffs.Count || count >= _upgradeViewCount)
                    break;
                var buff = ResourceManager.Buffs.GetResource(id);
                if (Parent.CurrentUser.Buffs.Contains(id))
                    continue;

                AddControl(0, new TextboxControl(this)
                {
                    HorizontalAlignment = Engine.Alignment.Middle,
                    Y = 250 + count * 140 + 5,
                    Width = 600,
                    Height = 100,
                    Text = buff.GetDescriptionString(),
                    Font = BasicFonts.GetFont(16),
                    FillColor = BasicTextures.GetBasicRectange(Color.DarkCyan)
                });
                AddControl(0, new ButtonControl(this, clicked: (x) =>
                {
                    Parent.UserManager.AddBuffUpgrade(Parent.CurrentUser, id);
                    SwitchView(new PermaBuffsView(Parent));
                })
                {
                    IsEnabled = buff.IsValid(Parent.CurrentUser),
                    HorizontalAlignment = Engine.Alignment.Middle,
                    Y = 350 + count * 140 + 5,
                    Width = 600,
                    Height = 35,
                    Text = "Claim",
                    Font = BasicFonts.GetFont(16),
                    FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                    FillClickedColor = BasicTextures.GetClickedTexture(),
                });
                count++;
            }

            AddControl(0, new ButtonControl(this, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                X = 800,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
            });
#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new PermaBuffsView(Parent)))
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
