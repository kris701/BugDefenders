using BugDefender.Core.Game.Models;
using MonoGame.OpenGL.Formatter.Textures;
using System;

namespace BugDefender.OpenGL.Views.GameView
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
            Size = textureSetDefinition.GetLoadedContent()[0].Width;
        }
    }
}
