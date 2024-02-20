﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.Core.Users.Models;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Screens.AchivementsView
{
    public class AchivementControl : TileControl
    {
        public AchivementDefinition Achivement { get; }

        private TileControl _iconTile;
        private TextboxControl _descriptionTextbox;
        public AchivementControl(UIEngine parent, AchivementDefinition achivement) : base(parent)
        {
            Achivement = achivement;
            Width = 800;
            Height = 120;
        }

        public override void Initialize()
        {
            base.Initialize();
            _iconTile = new TileControl(Parent)
            {
                Width = 75,
                Height = 75,
                FillColor = TextureManager.GetTexture(Achivement.ID)
            };
            _iconTile._x = _x + Scale(20);
            _iconTile._y = _y + Scale(20);
            _iconTile.Initialize();

            _descriptionTextbox = new TextboxControl(Parent)
            {
                Width = 600,
                Height = 100,
                Font = BasicFonts.GetFont(10),
                FontColor = Color.White,
                Text = $"{Achivement.Name}{Environment.NewLine}{Achivement.Description}"
            };
            _descriptionTextbox._x = _x + Scale(10) + Scale(100);
            _descriptionTextbox._y = _y + Scale(10);
            _descriptionTextbox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _descriptionTextbox.Update(gameTime);
            _iconTile.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime, spriteBatch);
            _iconTile.Draw(gameTime, spriteBatch);
            _descriptionTextbox.Draw(gameTime, spriteBatch);
        }
    }
}
