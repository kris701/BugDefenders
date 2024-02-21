using System;
using TDGame.Core.Game.Models;
using TDGame.Core.Game.Models.Entities.Projectiles;

namespace TDGame.OpenGL.Screens.GameScreen
{
    public class EffectEntity : BasePositionModel, IIdentifiable
    {
        public Guid ID { get; set; }
        public TimeSpan LifeTime { get; set; } = TimeSpan.FromSeconds(1);
    }
}
