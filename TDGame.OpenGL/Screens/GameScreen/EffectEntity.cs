﻿using Microsoft.Xna.Framework.Graphics;
using System;
using TDGame.Core.Game.Models;
using TDGame.OpenGL.ResourcePacks;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EffectEntity : BasePositionModel, IIdentifiable
    {
        public Guid ID { get; set; }
        public TimeSpan LifeTime { get; set; } = TimeSpan.FromSeconds(1);
        public TextureSetDefinition TextureSetDefinition { get; set; }

        public EffectEntity(Guid iD, TimeSpan lifeTime, TextureSetDefinition textureSetDefinition)
        {
            ID = iD;
            LifeTime = lifeTime;
            TextureSetDefinition = textureSetDefinition;
            Size = textureSetDefinition.LoadedContents[0].Width;
        }
    }
}
