using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Helpers;
using static TDGame.OpenGL.Engine.Controls.ButtonControl;
using TDGame.Core.Turrets.Upgrades;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class UpgradePanel : TileControl
    {
        public event ClickedHandler Buy;
        public List<IControl> Children { get; set; }
        private LabelControl _nameLabel;
        private TextboxControl _descriptionTextbox;
        private ButtonControl _buyUpgradeButton;
        private TileControl _cantBuyTile;

        public UpgradePanel(IScreen parent, ClickedHandler buy) : base(parent)
        {
            Buy = buy;
            _nameLabel = new LabelControl(Parent)
            {
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                Height = 35
            };
            _descriptionTextbox = new TextboxControl(Parent)
            {
                Font = BasicFonts.GetFont(10),
                FillColor = BasicTextures.GetBasicRectange(Color.DarkCyan),
                Height = 60
            };
            _buyUpgradeButton = new ButtonControl(Parent, clicked: buy)
            {
                Font = BasicFonts.GetFont(16),
                FillColor = BasicTextures.GetBasicRectange(Color.Red),
                Text = "Buy",
                Height = 35
            };
            _cantBuyTile = new TileControl(Parent)
            {
                FillColor = BasicTextures.GetBasicRectange(Color.DarkGray),
                Alpha = 100
            };
            Children = new List<IControl>() {
                _nameLabel,
                _descriptionTextbox,
                _buyUpgradeButton,
                _cantBuyTile
            };
        }

        public void SetUpgrade(IUpgrade upgrade, bool canUpgrade)
        {
            _nameLabel.Text = $"[{upgrade.Cost}$] {upgrade.Name}";
            _descriptionTextbox.Text = upgrade.GetDescriptionString();
            _buyUpgradeButton.Tag = upgrade;
            IsVisible = true;
            foreach (var child in Children)
                child.IsVisible = true;
            _buyUpgradeButton.IsEnabled = canUpgrade;
            _cantBuyTile.IsVisible = !canUpgrade;
            Initialize();
        }

        public void TurnInvisible()
        {
            IsVisible = false;
            foreach (var child in Children)
                child.IsVisible = false;
        }

        public override void Initialize()
        {
            _nameLabel._x = _x;
            _nameLabel._y = _y;
            _nameLabel._width = _width;
            _descriptionTextbox._x = _x;
            _descriptionTextbox._y = _y + Parent.Scale(35);
            _descriptionTextbox._width = _width;
            _buyUpgradeButton._x = _x;
            _buyUpgradeButton._y = _y + Parent.Scale(35 + 65);
            _buyUpgradeButton._width = _width;
            _cantBuyTile._x = _x;
            _cantBuyTile._y = _y;
            _cantBuyTile._width = _width;
            _cantBuyTile._height = _height;

            foreach (var child in Children)
                child.Initialize();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var child in Children)
                child.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }
    }
}
